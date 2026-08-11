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
import { apiGet, apiPatch, apiPost } from '../../api/client';
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

function canSendToVendor(user) {
  if (!user) return false;
  const permissions = user.permissions;
  if (permissions && Object.keys(permissions).length > 0) {
    return Boolean(permissions.poTrack || permissions.poWoGenerate);
  }
  return Boolean(user.canGeneratePO);
}

export default function PoStatusViewPage() {
  const { success, error } = useSnackbar();
  const { user } = useAuth();
  const [rows, setRows] = useState([]);
  const [status, setStatus] = useState('All');
  const [q, setQ] = useState('');
  const [loading, setLoading] = useState(true);
  const [busyRef, setBusyRef] = useState('');
  const [lifecycle, setLifecycle] = useState(null);
  const [finalComment, setFinalComment] = useState('');
  const [trackPo, setTrackPo] = useState(null);
  const [finalRemarks, setFinalRemarks] = useState('');
  const [oaDate, setOaDate] = useState('');

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

  const submitLifecycle = async () => {
    if (!lifecycle) return;
    if (!String(finalComment || '').trim()) {
      error('Final comment is required');
      return;
    }
    setBusyRef(lifecycle.po.poRef);
    try {
      await apiPost(`/pos/${encodeURIComponent(lifecycle.po.poRef)}/${lifecycle.action}`, {
        finalComment: String(finalComment).trim(),
      });
      success(`${lifecycle.po.poRef} ${lifecycle.action === 'cancel' ? 'cancelled' : 'closed'}`);
      setLifecycle(null);
      setFinalComment('');
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusyRef('');
    }
  };

  const submitTrack = async () => {
    if (!trackPo) return;
    setBusyRef(trackPo.poRef);
    try {
      await apiPatch(`/pos/${encodeURIComponent(trackPo.poRef)}/track`, {
        finalRemarks: finalRemarks || undefined,
        oaDate: oaDate || undefined,
      });
      success(`${trackPo.poRef} tracking updated`);
      setTrackPo(null);
      setFinalRemarks('');
      setOaDate('');
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusyRef('');
    }
  };

  const allowCancelClose = Boolean(user?.canCancelClosePO || user?.isGm || user?.isOm);
  const allowSend = canSendToVendor(user);
  const allowTrack = allowSend || Boolean(user?.permissions?.purchaseOrder);

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
                    <Stack direction="row" spacing={1} justifyContent="flex-end" flexWrap="wrap">
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
                      {allowSend && po.status === 'PO Generated' && (
                        <Button
                          size="small"
                          variant="outlined"
                          disabled={busyRef === po.poRef}
                          onClick={() => advance(po, 'send')}
                        >
                          Send to vendor
                        </Button>
                      )}
                      {allowTrack &&
                        !['Cancelled', 'Closed', 'Rejected'].includes(po.status) && (
                          <Button
                            size="small"
                            disabled={busyRef === po.poRef}
                            onClick={() => {
                              setTrackPo(po);
                              setFinalRemarks(po.finalRemarks || '');
                              setOaDate(po.oaDate ? String(po.oaDate).slice(0, 10) : '');
                            }}
                          >
                            Track
                          </Button>
                        )}
                      {allowCancelClose &&
                        !['Cancelled', 'Closed'].includes(po.status) && (
                          <>
                            <Button
                              size="small"
                              color="error"
                              disabled={busyRef === po.poRef}
                              onClick={() => {
                                setLifecycle({ po, action: 'cancel' });
                                setFinalComment('');
                              }}
                            >
                              Cancel
                            </Button>
                            <Button
                              size="small"
                              color="warning"
                              disabled={busyRef === po.poRef}
                              onClick={() => {
                                setLifecycle({ po, action: 'close' });
                                setFinalComment('');
                              }}
                            >
                              Close
                            </Button>
                          </>
                        )}
                    </Stack>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={Boolean(lifecycle)} onClose={() => setLifecycle(null)} fullWidth maxWidth="sm">
        <DialogTitle>
          {lifecycle?.action === 'cancel' ? 'Cancel PO' : 'Close PO'} — {lifecycle?.po?.poRef}
        </DialogTitle>
        <DialogContent>
          <TextField
            sx={{ mt: 1 }}
            label="Final comment"
            value={finalComment}
            onChange={(e) => setFinalComment(e.target.value)}
            required
            fullWidth
            multiline
            minRows={3}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setLifecycle(null)}>Back</Button>
          <Button variant="contained" color={lifecycle?.action === 'cancel' ? 'error' : 'warning'} onClick={submitLifecycle}>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={Boolean(trackPo)} onClose={() => setTrackPo(null)} fullWidth maxWidth="sm">
        <DialogTitle>Track PO — {trackPo?.poRef}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="OA date"
              type="date"
              value={oaDate}
              onChange={(e) => setOaDate(e.target.value)}
              InputLabelProps={{ shrink: true }}
              fullWidth
            />
            <TextField
              label="Final remarks"
              value={finalRemarks}
              onChange={(e) => setFinalRemarks(e.target.value)}
              fullWidth
              multiline
              minRows={3}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setTrackPo(null)}>Back</Button>
          <Button variant="contained" onClick={submitTrack}>
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
