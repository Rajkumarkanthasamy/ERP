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
import { formatDate } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function ItemCodePage({ mode = 'create' }) {
  const isApproval = mode === 'approval';
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [status, setStatus] = useState(isApproval ? 'Pending' : 'All');
  const [loading, setLoading] = useState(true);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({
    proposedCode: '',
    itemDescription: '',
    specification: '',
    make: '',
    uom: 'NOS',
    remarks: '',
  });
  const [decideRow, setDecideRow] = useState(null);
  const [approvedCode, setApprovedCode] = useState('');
  const [reason, setReason] = useState('');
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/masters/item-codes', { status });
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

  const create = async () => {
    setBusy(true);
    try {
      await apiPost('/masters/item-codes', form);
      success('Item code request submitted');
      setOpen(false);
      setForm({ proposedCode: '', itemDescription: '', specification: '', make: '', uom: 'NOS', remarks: '' });
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusy(false);
    }
  };

  const decide = async (action) => {
    if (!decideRow) return;
    setBusy(true);
    try {
      await apiPost(`/masters/item-codes/${decideRow.id}/decide`, {
        action,
        approvedCode: approvedCode || decideRow.proposedCode,
        reason,
      });
      success(`Request ${action}d`);
      setDecideRow(null);
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
        title={isApproval ? 'Item Code Approval' : 'Item Code Creation'}
        subtitle={
          isApproval
            ? 'Approve or reject proposed item codes before master creation.'
            : 'Request new item codes for engineering / stores masters.'
        }
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: isApproval ? 'Item Code Approval' : 'Item Code Creation' },
        ]}
        actions={
          isApproval
            ? [{ label: 'Refresh', variant: 'outlined', onClick: load }]
            : [
                { label: 'New request', variant: 'contained', onClick: () => setOpen(true) },
                { label: 'Refresh', variant: 'outlined', onClick: load },
              ]
        }
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <TextField select size="small" label="Status" value={status} onChange={(e) => setStatus(e.target.value)} sx={{ minWidth: 180 }}>
          {['All', 'Pending', 'Approved', 'Rejected'].map((s) => (
            <MenuItem key={s} value={s}>
              {s}
            </MenuItem>
          ))}
        </TextField>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No item code requests" />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Request</TableCell>
                <TableCell>Proposed</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Make</TableCell>
                <TableCell>Requested</TableCell>
                <TableCell>Status</TableCell>
                {isApproval && <TableCell align="right">Actions</TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((r) => (
                <TableRow key={r.id} hover>
                  <TableCell>{r.requestNo}</TableCell>
                  <TableCell>{r.proposedCode || '—'}</TableCell>
                  <TableCell>{r.itemDescription}</TableCell>
                  <TableCell>{r.make || '—'}</TableCell>
                  <TableCell>
                    {r.requestedBy}
                    <br />
                    {formatDate(r.requestDate)}
                  </TableCell>
                  <TableCell>
                    <StatusChip status={r.approvalStatus} />
                  </TableCell>
                  {isApproval && (
                    <TableCell align="right">
                      {r.approvalStatus === 'Pending' && (
                        <Button
                          size="small"
                          variant="contained"
                          onClick={() => {
                            setDecideRow(r);
                            setApprovedCode(r.proposedCode || '');
                            setReason('');
                          }}
                        >
                          Decide
                        </Button>
                      )}
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>New item code request</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {[
              ['proposedCode', 'Proposed code'],
              ['itemDescription', 'Description'],
              ['specification', 'Specification'],
              ['make', 'Make'],
              ['uom', 'UOM'],
              ['remarks', 'Remarks'],
            ].map(([name, label]) => (
              <TextField
                key={name}
                label={label}
                required={name === 'itemDescription'}
                value={form[name]}
                onChange={(e) => setForm((p) => ({ ...p, [name]: e.target.value }))}
                fullWidth
                multiline={name === 'remarks' || name === 'specification'}
              />
            ))}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" disabled={busy} onClick={create}>
            Submit
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={Boolean(decideRow)} onClose={() => setDecideRow(null)} fullWidth maxWidth="xs">
        <DialogTitle>Decide {decideRow?.requestNo}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField label="Approved code" value={approvedCode} onChange={(e) => setApprovedCode(e.target.value)} fullWidth />
            <TextField label="Reason" value={reason} onChange={(e) => setReason(e.target.value)} fullWidth multiline minRows={2} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDecideRow(null)}>Cancel</Button>
          <Button color="error" disabled={busy} onClick={() => decide('reject')}>
            Reject
          </Button>
          <Button variant="contained" disabled={busy} onClick={() => decide('approve')}>
            Approve
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
