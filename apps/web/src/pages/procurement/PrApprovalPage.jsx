import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
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
import CollabPanel from '../../components/CollabPanel';

const ACTIONS_BY_STATUS = {
  Pending: [
    { action: 'approve', label: 'Approve' },
    { action: 'hold', label: 'Hold', color: 'warning', reasonRequired: true },
    { action: 'reject', label: 'Reject', color: 'error', reasonRequired: true },
  ],
  'On Hold': [
    { action: 'approve', label: 'Approve' },
    { action: 'release', label: 'Release' },
    { action: 'reject', label: 'Reject', color: 'error', reasonRequired: true },
  ],
};

export default function PrApprovalPage() {
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [status, setStatus] = useState('Pending');
  const [loading, setLoading] = useState(true);
  const [dialog, setDialog] = useState(null);
  const [reason, setReason] = useState('');
  const [busy, setBusy] = useState(false);
  const [selectedPr, setSelectedPr] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/prs', { status });
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      error(err.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }, [status, error]);

  useEffect(() => {
    load();
  }, [load]);

  const decide = async () => {
    if (!dialog) return;
    if (dialog.reasonRequired && !reason.trim()) {
      error(`${dialog.label} reason is required`);
      return;
    }
    setBusy(true);
    try {
      await apiPost(`/prs/${encodeURIComponent(dialog.pr.prNumber)}/status`, {
        action: dialog.action,
        reason,
      });
      success(`${dialog.pr.prNumber}: ${dialog.label} completed`);
      setDialog(null);
      setReason('');
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
        title="PR Approval"
        subtitle="Approve, reject, or hold pending purchase requests."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PR Approval' },
        ]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <TextField select size="small" label="Status" value={status} onChange={(e) => setStatus(e.target.value)} sx={{ minWidth: 180 }}>
            {['Pending', 'Approved', 'Rejected', 'On Hold', 'All'].map((s) => (
              <MenuItem key={s} value={s}>
                {s}
              </MenuItem>
            ))}
          </TextField>
          <Button variant="outlined" onClick={load}>
            Refresh
          </Button>
        </Stack>
      </Paper>

      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No PRs" description="Nothing matches the selected status." />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>PR No</TableCell>
                <TableCell>Project</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Requested</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((pr) => (
                <TableRow
                  key={pr.prNumber}
                  hover
                  selected={selectedPr === pr.prNumber}
                  onClick={() => setSelectedPr(pr.prNumber)}
                  sx={{ cursor: 'pointer' }}
                >
                  <TableCell>{pr.prNumber}</TableCell>
                  <TableCell>{pr.projectCode}</TableCell>
                  <TableCell>{pr.vendorName || pr.vendorCode}</TableCell>
                  <TableCell>{formatINR(pr.totalAmount)}</TableCell>
                  <TableCell>{pr.requestedBy || '—'}</TableCell>
                  <TableCell>{formatDate(pr.requestDate)}</TableCell>
                  <TableCell>
                    <StatusChip status={pr.status} />
                  </TableCell>
                  <TableCell align="right" onClick={(e) => e.stopPropagation()}>
                    <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                      {(ACTIONS_BY_STATUS[pr.status] || []).map((action) => (
                        <Button
                          key={action.action}
                          size="small"
                          color={action.color || 'primary'}
                          onClick={() => setDialog({ pr, ...action })}
                        >
                          {action.label}
                        </Button>
                      ))}
                    </Stack>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Paper sx={{ p: 2.5, mt: 2 }}>
        <CollabPanel entityType="PR" entityRef={selectedPr} title="PR collaboration" />
      </Paper>

      <Dialog open={Boolean(dialog)} onClose={() => setDialog(null)} fullWidth maxWidth="xs">
        <DialogTitle>
          {dialog?.label} — {dialog?.pr?.prNumber}
        </DialogTitle>
        <DialogContent>
          <TextField
            label="Reason / remarks"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            fullWidth
            multiline
            minRows={3}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialog(null)}>Cancel</Button>
          <Button variant="contained" onClick={decide} disabled={busy}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
