import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  Checkbox,
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
import { apiGet, apiPost } from '../../api/client';
import { formatDate, formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function GrnPage() {
  const { success, error } = useSnackbar();
  const [grns, setGrns] = useState([]);
  const [openLines, setOpenLines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selected, setSelected] = useState({});
  const [invoiceNo, setInvoiceNo] = useState('');
  const [remarks, setRemarks] = useState('');
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [list, lines] = await Promise.all([apiGet('/grns'), apiGet('/grns/open-po-lines')]);
      setGrns(Array.isArray(list) ? list : []);
      setOpenLines(Array.isArray(lines) ? lines : []);
    } catch (err) {
      error(err.message);
    } finally {
      setLoading(false);
    }
  }, [error]);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = () => {
    const init = {};
    openLines.forEach((l) => {
      init[l.id || `${l.poRef}-${l.itemCode}`] = {
        checked: false,
        qty: l.remainingQty ?? l.requiredQty ?? 0,
        line: l,
      };
    });
    setSelected(init);
    setInvoiceNo('');
    setRemarks('');
    setDialogOpen(true);
  };

  const submit = async () => {
    const lines = Object.values(selected)
      .filter((x) => x.checked)
      .map((x) => ({
        poRef: x.line.poRef,
        poId: x.line.poId || x.line.id,
        vendorCode: x.line.vendorCode,
        projectCode: x.line.projectCode,
        itemCode: x.line.itemCode,
        receivedQty: Number(x.qty),
        remainingQty: Number(x.line.remainingQty ?? x.line.requiredQty ?? 0),
      }));
    if (!lines.length) {
      error('Select at least one PO line');
      return;
    }
    if (!String(invoiceNo || '').trim()) {
      error('Invoice number is required');
      return;
    }
    if (
      lines.some(
        (line) =>
          !line.poId ||
          line.receivedQty <= 0 ||
          line.receivedQty > line.remainingQty
      )
    ) {
      error('Receipt quantity must be positive and cannot exceed the remaining PO quantity');
      return;
    }
    setBusy(true);
    try {
      const byPo = new Map();
      lines.forEach((line) => {
        if (!byPo.has(line.poRef)) byPo.set(line.poRef, []);
        byPo.get(line.poRef).push(line);
      });
      const created = [];
      for (const [poRef, poLines] of byPo) {
        const first = poLines[0];
        const res = await apiPost('/grns', {
          poRef,
          vendorCode: first.vendorCode,
          projectCode: first.projectCode,
          invoiceNo: String(invoiceNo).trim(),
          remarks,
          lines: poLines.map(({ poId, receivedQty }) => ({ poId, receivedQty })),
        });
        created.push(
          res?.legacyGinNumber
            ? `${res.grnNumber} (${res.legacyGinNumber})`
            : res?.grnNumber
        );
      }
      success(`GRN ${created.filter(Boolean).join(', ')} created`);
      setDialogOpen(false);
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
        title="Goods Receipt Note (GRN)"
        subtitle="Receive open PO lines against invoices. Live mode also posts legacy ITWGIN stock."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'GRN' },
        ]}
        actions={[
          { label: 'New GRN', variant: 'contained', onClick: openCreate },
          { label: 'Refresh', variant: 'outlined', onClick: load },
        ]}
      />
      {loading ? (
        <LoadingBlock />
      ) : grns.length === 0 ? (
        <EmptyState title="No GRNs yet" actionLabel="Create GRN" onAction={openCreate} />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>GRN No</TableCell>
                <TableCell>Legacy GIN</TableCell>
                <TableCell>Invoice</TableCell>
                <TableCell>PO</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Amount</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {grns.map((g) => (
                <TableRow key={g.grnNumber || g.id} hover>
                  <TableCell>{g.grnNumber}</TableCell>
                  <TableCell>{g.legacyGinNumber || '—'}</TableCell>
                  <TableCell>{g.invoiceNo || '—'}</TableCell>
                  <TableCell>{g.poRef || '—'}</TableCell>
                  <TableCell>{g.vendorName || g.vendorCode || '—'}</TableCell>
                  <TableCell>{formatDate(g.grnDate || g.receivedDate || g.createdAt)}</TableCell>
                  <TableCell>{formatINR(g.totalAmount)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="md">
        <DialogTitle>Create GRN</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <TextField
                label="Invoice No"
                value={invoiceNo}
                onChange={(e) => setInvoiceNo(e.target.value)}
                required
                fullWidth
                helperText="Required for live GIN posting to Receipt / inventory"
              />
              <TextField label="Remarks" value={remarks} onChange={(e) => setRemarks(e.target.value)} fullWidth />
            </Stack>
            {openLines.length === 0 ? (
              <Typography color="text.secondary">No open PO lines available.</Typography>
            ) : (
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell padding="checkbox" />
                    <TableCell>PO</TableCell>
                    <TableCell>Item</TableCell>
                    <TableCell>Remaining</TableCell>
                    <TableCell>Receive qty</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {openLines.map((l) => {
                    const key = l.id || `${l.poRef}-${l.itemCode}`;
                    const sel = selected[key] || { checked: false, qty: l.remainingQty };
                    return (
                      <TableRow key={key}>
                        <TableCell padding="checkbox">
                          <Checkbox
                            checked={Boolean(sel.checked)}
                            onChange={(e) =>
                              setSelected((prev) => ({
                                ...prev,
                                [key]: { ...sel, checked: e.target.checked, line: l },
                              }))
                            }
                          />
                        </TableCell>
                        <TableCell>{l.poRef}</TableCell>
                        <TableCell>
                          {l.itemCode}
                          <Typography variant="caption" display="block" color="text.secondary">
                            {l.itemDescription}
                          </Typography>
                        </TableCell>
                        <TableCell>{l.remainingQty ?? l.requiredQty}</TableCell>
                        <TableCell>
                          <TextField
                            size="small"
                            type="number"
                            value={sel.qty}
                            onChange={(e) =>
                              setSelected((prev) => ({
                                ...prev,
                                [key]: { ...sel, qty: e.target.value, line: l },
                              }))
                            }
                          />
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            )}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" disabled={busy} onClick={submit}>
            {busy ? 'Saving…' : 'Create GRN'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
