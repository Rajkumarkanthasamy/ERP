import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import StatusChip from '../../components/StatusChip';
import { apiGet, apiPost } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function PoApprovalPage() {
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState(null);
  const [remarks, setRemarks] = useState('');
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/pos/approvals');
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      error(err.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }, [error]);

  useEffect(() => {
    load();
  }, [load]);

  const approve = async (action) => {
    if (!selected) return;
    setBusy(true);
    try {
      await apiPost(`/pos/${encodeURIComponent(selected.poRef)}/approve`, {
        action,
        remarks,
      });
      success(`${selected.poRef} ${action}`);
      setSelected(null);
      setRemarks('');
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusy(false);
    }
  };

  return (
    <Box>
      <PageHeader
        title="PO Approval"
        subtitle="Multi-tier PO approvals (PM / MH / PC / OM / GM)."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PO Approval' },
        ]}
        actions={[{ label: 'Refresh', onClick: load, variant: 'outlined' }]}
      />
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No POs awaiting approval" />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>PO Ref</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Project</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Tier</TableCell>
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
                  <TableCell>
                    <StatusChip status={po.status || 'Pending'} />
                  </TableCell>
                  <TableCell>{po.approvalTier || '—'}</TableCell>
                  <TableCell align="right">
                    <Button size="small" variant="contained" onClick={() => setSelected(po)}>
                      Review
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={Boolean(selected)} onClose={() => setSelected(null)} fullWidth maxWidth="sm">
        <DialogTitle>Approve PO {selected?.poRef}</DialogTitle>
        <DialogContent>
          <Stack spacing={1.5} sx={{ mt: 1 }}>
            <Typography>
              Vendor: {selected?.vendorName || selected?.vendorCode} · Amount:{' '}
              {formatINR(selected?.totalAmount ?? selected?.amount)}
            </Typography>
            <TextField
              label="Remarks"
              value={remarks}
              onChange={(e) => setRemarks(e.target.value)}
              fullWidth
              multiline
              minRows={3}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSelected(null)}>Cancel</Button>
          <Button color="error" disabled={busy} onClick={() => approve('reject')}>
            Reject
          </Button>
          <Button variant="contained" disabled={busy} onClick={() => approve('approve')}>
            Approve
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
