import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
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
  Tooltip,
  Typography,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import StatusChip from '../../components/StatusChip';
import { useSnackbar } from '../../components/SnackbarProvider';
import { apiGet, apiPost } from '../../api/client';
import { formatDate } from '../../utils/format';

const today = () => new Date().toISOString().slice(0, 10);
const blankLine = () => ({ itemCode: '', itemDescription: '', quantity: 1 });
const documentTypes = [
  'Purchase Order',
  'Work Order',
  'Delivery Challan',
  'Invoice',
  'Gate Pass',
  'Other',
];

function initialForm(entryType) {
  return {
    entryType,
    documentType: entryType === 'Manual' ? 'Manual' : 'Purchase Order',
    documentNo: '',
    vendorName: '',
    invoiceNo: '',
    invoiceDate: today(),
    vehicleNo: '',
    ewayBillNo: '',
    ewayBillDate: today(),
    lrPodNo: '',
    lrPodDate: today(),
    remarks: '',
    lines: [blankLine()],
  };
}

function GateEntryPage({ entryType, title, subtitle, createLabel }) {
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState(() => initialForm(entryType));

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/gate-entries', { type: entryType });
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      error(err.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }, [entryType, error]);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = () => {
    setForm(initialForm(entryType));
    setOpen(true);
  };

  const setLine = (index, patch) => {
    setForm((current) => ({
      ...current,
      lines: current.lines.map((line, lineIndex) =>
        lineIndex === index ? { ...line, ...patch } : line
      ),
    }));
  };

  const submit = async () => {
    if (!form.documentNo.trim() || !form.vendorName.trim()) {
      error('Document number and party / vendor name are required');
      return;
    }
    if (entryType === 'Inward' && !form.invoiceNo.trim()) {
      error('Invoice number is required');
      return;
    }
    if (
      entryType === 'Outward' &&
      (!form.vehicleNo.trim() || !form.lrPodNo.trim() || !form.lrPodDate)
    ) {
      error('Vehicle number, LR / POD number and date are required');
      return;
    }
    if (
      form.lines.some(
        (line) =>
          !line.itemDescription.trim() ||
          !Number.isFinite(Number(line.quantity)) ||
          Number(line.quantity) <= 0
      )
    ) {
      error('Every item requires a description and positive quantity');
      return;
    }

    setSaving(true);
    try {
      const created = await apiPost('/gate-entries', {
        ...form,
        lines: form.lines.map((line) => ({
          ...line,
          quantity: Number(line.quantity),
        })),
      });
      success(`${created.entryNumber} recorded`);
      setOpen(false);
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setSaving(false);
    }
  };

  const crumbs = [
    { label: 'Home', to: '/' },
    { label: 'Gate Entry' },
    { label: title },
  ];

  return (
    <Box>
      <PageHeader
        title={title}
        subtitle={subtitle}
        crumbs={crumbs}
        actions={[
          { label: createLabel, onClick: openCreate, variant: 'contained' },
          { label: 'Refresh', onClick: load, variant: 'outlined' },
        ]}
      />

      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState
          title={`No ${title.toLowerCase()} records`}
          actionLabel={createLabel}
          onAction={openCreate}
        />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Entry No</TableCell>
                <TableCell>Document</TableCell>
                <TableCell>Party</TableCell>
                <TableCell>Items</TableCell>
                <TableCell>Quantity</TableCell>
                {entryType === 'Outward' && <TableCell>Vehicle</TableCell>}
                <TableCell>Date</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.entryNumber} hover>
                  <TableCell>{row.entryNumber}</TableCell>
                  <TableCell>
                    <Typography variant="body2">{row.documentNo || '—'}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {row.documentType || row.purpose || '—'}
                    </Typography>
                  </TableCell>
                  <TableCell>{row.vendorName || '—'}</TableCell>
                  <TableCell>
                    {row.lineCount || row.lines?.length || 0}
                    {row.itemDescription && (
                      <Typography variant="caption" display="block" color="text.secondary">
                        {row.itemDescription}
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>{row.totalQuantity ?? row.quantity ?? 0}</TableCell>
                  {entryType === 'Outward' && <TableCell>{row.vehicleNo || '—'}</TableCell>}
                  <TableCell>{formatDate(row.createdAt)}</TableCell>
                  <TableCell>
                    <StatusChip status={row.status || 'Recorded'} />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={open} onClose={() => !saving && setOpen(false)} fullWidth maxWidth="md">
        <DialogTitle>{createLabel}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              {entryType !== 'Manual' && (
                <TextField
                  select
                  label="Document type"
                  required
                  fullWidth
                  value={form.documentType}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      documentType: event.target.value,
                    }))
                  }
                >
                  {documentTypes.map((option) => (
                    <MenuItem key={option} value={option}>
                      {option}
                    </MenuItem>
                  ))}
                </TextField>
              )}
              <TextField
                label="Document number"
                required
                fullWidth
                value={form.documentNo}
                onChange={(event) =>
                  setForm((current) => ({ ...current, documentNo: event.target.value }))
                }
              />
              <TextField
                label="Party / vendor name"
                required
                fullWidth
                value={form.vendorName}
                onChange={(event) =>
                  setForm((current) => ({ ...current, vendorName: event.target.value }))
                }
              />
            </Stack>

            {entryType === 'Inward' && (
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  label="Invoice number"
                  required
                  fullWidth
                  value={form.invoiceNo}
                  onChange={(event) =>
                    setForm((current) => ({ ...current, invoiceNo: event.target.value }))
                  }
                />
                <TextField
                  label="Invoice date"
                  type="date"
                  fullWidth
                  value={form.invoiceDate}
                  onChange={(event) =>
                    setForm((current) => ({ ...current, invoiceDate: event.target.value }))
                  }
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              </Stack>
            )}

            {entryType === 'Outward' && (
              <>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                  <TextField
                    label="Vehicle number"
                    required
                    fullWidth
                    value={form.vehicleNo}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, vehicleNo: event.target.value }))
                    }
                  />
                  <TextField
                    label="E-way bill number"
                    fullWidth
                    value={form.ewayBillNo}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, ewayBillNo: event.target.value }))
                    }
                  />
                  <TextField
                    label="E-way bill date"
                    type="date"
                    fullWidth
                    value={form.ewayBillDate}
                    onChange={(event) =>
                      setForm((current) => ({
                        ...current,
                        ewayBillDate: event.target.value,
                      }))
                    }
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Stack>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                  <TextField
                    label="LR / POD number"
                    required
                    fullWidth
                    value={form.lrPodNo}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, lrPodNo: event.target.value }))
                    }
                  />
                  <TextField
                    label="LR / POD date"
                    required
                    type="date"
                    fullWidth
                    value={form.lrPodDate}
                    onChange={(event) =>
                      setForm((current) => ({ ...current, lrPodDate: event.target.value }))
                    }
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Stack>
              </>
            )}

            <Stack direction="row" alignItems="center" justifyContent="space-between">
              <Typography variant="h6">Item lines</Typography>
              <Button
                startIcon={<AddIcon />}
                onClick={() =>
                  setForm((current) => ({
                    ...current,
                    lines: [...current.lines, blankLine()],
                  }))
                }
              >
                Add item
              </Button>
            </Stack>

            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell width="25%">Item code</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell width="18%">Quantity</TableCell>
                  <TableCell width={48} />
                </TableRow>
              </TableHead>
              <TableBody>
                {form.lines.map((line, index) => (
                  <TableRow key={index}>
                    <TableCell>
                      <TextField
                        size="small"
                        fullWidth
                        value={line.itemCode}
                        onChange={(event) => setLine(index, { itemCode: event.target.value })}
                      />
                    </TableCell>
                    <TableCell>
                      <TextField
                        size="small"
                        required
                        fullWidth
                        value={line.itemDescription}
                        onChange={(event) =>
                          setLine(index, { itemDescription: event.target.value })
                        }
                      />
                    </TableCell>
                    <TableCell>
                      <TextField
                        size="small"
                        required
                        type="number"
                        fullWidth
                        value={line.quantity}
                        onChange={(event) => setLine(index, { quantity: event.target.value })}
                        slotProps={{ htmlInput: { min: 0.001, step: 'any' } }}
                      />
                    </TableCell>
                    <TableCell>
                      <Tooltip title="Remove item">
                        <span>
                          <IconButton
                            disabled={form.lines.length === 1}
                            onClick={() =>
                              setForm((current) => ({
                                ...current,
                                lines: current.lines.filter(
                                  (_item, lineIndex) => lineIndex !== index
                                ),
                              }))
                            }
                          >
                            <DeleteOutlinedIcon />
                          </IconButton>
                        </span>
                      </Tooltip>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>

            <TextField
              label="Remarks"
              multiline
              minRows={2}
              fullWidth
              value={form.remarks}
              onChange={(event) =>
                setForm((current) => ({ ...current, remarks: event.target.value }))
              }
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)} disabled={saving}>
            Cancel
          </Button>
          <Button variant="contained" onClick={submit} disabled={saving}>
            {saving ? 'Saving…' : 'Record entry'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export function GateInwardPage() {
  return (
    <GateEntryPage
      entryType="Inward"
      title="Gate Inward"
      subtitle="Record invoice-backed materials entering the premises."
      createLabel="New inward"
    />
  );
}

export function GateOutwardPage() {
  return (
    <GateEntryPage
      entryType="Outward"
      title="Gate Outward"
      subtitle="Record document, logistics, and item details leaving the premises."
      createLabel="New outward"
    />
  );
}

export function ManualInwardPage() {
  return (
    <GateEntryPage
      entryType="Manual"
      title="Gate Manual Inward"
      subtitle="Record manual inward material with a reference document."
      createLabel="Manual inward"
    />
  );
}
