import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import StatusChip from '../../components/StatusChip';
import { apiGet, apiPost } from '../../api/client';
import { formatDate, formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';
import { useAuth } from '../../auth/AuthContext';

const STATUS_OPTIONS = [
  'All',
  'Created',
  'Pending PM / Dept Approval',
  'Pending MH Approval',
  'Pending Purchase Committee',
  'Pending OM Approval',
  'Pending GM Approval',
  'Ready to Generate',
  'PO Generated',
  'Sent to Vendor',
  'Partially Received',
  'Fully Received',
  'Rejected',
  'Closed',
  'Cancelled',
];

export default function PoStatusViewPage() {
  const { success, error } = useSnackbar();
  const { user } = useAuth();
  const [rows, setRows] = useState([]);
  const [status, setStatus] = useState('All');
  const [q, setQ] = useState('');
  const [loading, setLoading] = useState(true);
  const [busyRef, setBusyRef] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/pos/status', { status, q });
      setRows(Array.isArray(data) ? data : data?.items || []);
    } catch (err) {
      error(err.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }, [status, q, error]);

  useEffect(() => {
    load();
  }, [load]);

  const advance = async (po, step) => {
    setBusyRef(po.poRef);
    try {
      await apiPost(`/pos/${encodeURIComponent(po.poRef)}/approve`, { step });
      success(
        step === 'generate'
          ? `${po.poRef} generated`
          : `${po.poRef} marked as sent to vendor`
      );
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusyRef('');
    }
  };

  return (
    <Box>
      <PageHeader
        title="PO Status View"
        subtitle="Track purchase order lifecycle from creation to receipt."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PO Status' },
        ]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <TextField size="small" label="Search PO / vendor" value={q} onChange={(e) => setQ(e.target.value)} sx={{ flex: 1 }} />
          <TextField select size="small" label="Status" value={status} onChange={(e) => setStatus(e.target.value)} sx={{ minWidth: 220 }}>
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s}>
                {s}
              </MenuItem>
            ))}
          </TextField>
          <Button variant="outlined" onClick={load}>
            Apply
          </Button>
        </Stack>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No purchase orders" />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>PO Ref</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Project</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Prepared</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((po) => (
                <TableRow key={po.poRef} hover>
                  <TableCell>{po.poRef}</TableCell>
                  <TableCell>{po.vendorName || po.vendorCode}</TableCell>
                  <TableCell>{po.projectCode || '—'}</TableCell>
                  <TableCell>{formatINR(po.totalAmount ?? po.amount)}</TableCell>
                  <TableCell>{formatDate(po.preparedDate)}</TableCell>
                  <TableCell>
                    <StatusChip status={po.status} />
                  </TableCell>
                  <TableCell align="right">
                    {user?.canGeneratePO && po.status === 'Ready to Generate' && (
                      <Button
                        size="small"
                        variant="outlined"
                        disabled={busyRef === po.poRef}
                        onClick={() => advance(po, 'generate')}
                      >
                        Generate
                      </Button>
                    )}
                    {user?.canGeneratePO && po.status === 'PO Generated' && (
                      <Button
                        size="small"
                        variant="outlined"
                        disabled={busyRef === po.poRef}
                        onClick={() => advance(po, 'send')}
                      >
                        Send to vendor
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}
