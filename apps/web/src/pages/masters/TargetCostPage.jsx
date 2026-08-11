import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
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

export default function TargetCostPage() {
  const { success, error } = useSnackbar();
  const { user } = useAuth();
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [itemCode, setItemCode] = useState('');
  const [targetCost, setTargetCost] = useState('');
  const [remarks, setRemarks] = useState('');
  const [busy, setBusy] = useState(false);

  const canDecide =
    Boolean(user?.permissions?.generalManager) ||
    Boolean(user?.permissions?.standardCostUpdate) ||
    Boolean(user?.isGm);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/masters/target-cost');
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

  const propose = async () => {
    if (!itemCode.trim()) {
      error('Item code is required');
      return;
    }
    setBusy(true);
    try {
      await apiPost('/masters/target-cost', {
        itemCode: itemCode.trim(),
        targetCost: Number(targetCost),
        remarks,
      });
      success('Target cost proposed');
      setItemCode('');
      setTargetCost('');
      setRemarks('');
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusy(false);
    }
  };

  const decide = async (id, action) => {
    setBusy(true);
    try {
      await apiPost(`/masters/target-cost/${id}/decide`, { action });
      success(action === 'approve' ? 'Target cost approved' : 'Target cost rejected');
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
        title="Target Cost"
        subtitle="Propose ItemTargetCostHistory updates; GM approval writes ItemMaster.TargetCost."
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Target Cost' }]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <TextField size="small" label="Item code" value={itemCode} onChange={(e) => setItemCode(e.target.value)} />
          <TextField
            size="small"
            type="number"
            label="Target cost"
            value={targetCost}
            onChange={(e) => setTargetCost(e.target.value)}
          />
          <TextField
            size="small"
            label="Remarks"
            value={remarks}
            onChange={(e) => setRemarks(e.target.value)}
            sx={{ flex: 1 }}
          />
          <Button variant="contained" onClick={propose} disabled={busy}>
            Propose
          </Button>
          <Button variant="outlined" onClick={load}>
            Refresh
          </Button>
        </Stack>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No target cost history" description="Requires live SQL Server and ItemTargetCostHistory." />
      ) : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Item</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Target</TableCell>
                <TableCell>By</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>GM</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.id}>
                  <TableCell>{row.itemCode}</TableCell>
                  <TableCell>{row.itemDescription || '—'}</TableCell>
                  <TableCell>{formatINR(row.targetCost)}</TableCell>
                  <TableCell>{row.updatedBy || '—'}</TableCell>
                  <TableCell>{formatDate(row.updatedAt)}</TableCell>
                  <TableCell>
                    <StatusChip
                      status={
                        row.gmApproved == null ? 'Pending' : row.gmApproved ? 'Approved' : 'Rejected'
                      }
                    />
                  </TableCell>
                  <TableCell align="right">
                    {canDecide && row.gmApproved == null && (
                      <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                        <Button size="small" disabled={busy} onClick={() => decide(row.id, 'approve')}>
                          Approve
                        </Button>
                        <Button
                          size="small"
                          color="error"
                          disabled={busy}
                          onClick={() => decide(row.id, 'reject')}
                        >
                          Reject
                        </Button>
                      </Stack>
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
