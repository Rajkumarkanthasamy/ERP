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
import { apiGet } from '../../api/client';
import { formatDate, formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

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
];

export default function PoStatusViewPage() {
  const { error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [status, setStatus] = useState('All');
  const [q, setQ] = useState('');
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/pos/status', { status, q });
      setRows(Array.isArray(data) ? data : []);
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
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}
