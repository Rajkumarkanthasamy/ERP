import { useEffect, useMemo, useState } from 'react';
import {
  Box,
  Button,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import PageHeader from '../../components/PageHeader';
import { apiGet, apiPost } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

const emptyLine = () => ({
  itemCode: '',
  itemDescription: '',
  quantity: 1,
  uom: 'NOS',
  unitCost: 0,
  specification: '',
  make: '',
});

export default function PrGenerationPage() {
  const { success, error } = useSnackbar();
  const [vendors, setVendors] = useState([]);
  const [projects, setProjects] = useState([]);
  const [items, setItems] = useState([]);
  const [form, setForm] = useState({
    projectCode: '',
    vendorCode: '',
    remarks: '',
    lines: [emptyLine()],
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    Promise.all([
      apiGet('/masters/vendors').catch(() => []),
      apiGet('/masters/projects').catch(() => []),
      apiGet('/masters/items').catch(() => []),
    ]).then(([v, p, i]) => {
      setVendors(Array.isArray(v) ? v : []);
      setProjects(Array.isArray(p) ? p : []);
      setItems(Array.isArray(i) ? i : []);
    });
  }, []);

  const total = useMemo(
    () => form.lines.reduce((s, l) => s + Number(l.quantity || 0) * Number(l.unitCost || 0), 0),
    [form.lines]
  );

  const setLine = (idx, patch) => {
    setForm((prev) => {
      const lines = prev.lines.map((l, i) => (i === idx ? { ...l, ...patch } : l));
      return { ...prev, lines };
    });
  };

  const onItemPick = (idx, itemCode) => {
    const item = items.find((x) => x.itemCode === itemCode);
    setLine(idx, {
      itemCode,
      itemDescription: item?.itemDescription || '',
      uom: item?.uom || 'NOS',
      unitCost: item?.latestPurchasePrice || item?.standardCost || 0,
      specification: item?.specification || '',
      make: item?.make || '',
    });
  };

  const submit = async () => {
    if (!form.projectCode || !form.vendorCode || form.lines.length === 0) {
      error('Project, vendor and at least one line are required');
      return;
    }
    if (
      form.lines.some(
        (line) =>
          !line.itemCode ||
          Number(line.quantity) <= 0 ||
          Number(line.unitCost) < 0
      )
    ) {
      error('Every line requires an item, positive quantity, and valid unit cost');
      return;
    }
    setSaving(true);
    try {
      const vendor = vendors.find((v) => v.vendorCode === form.vendorCode);
      const project = projects.find((p) => p.projectCode === form.projectCode);
      const payload = {
        projectCode: form.projectCode,
        productNo: project?.productNo,
        vendorCode: form.vendorCode,
        vendorName: vendor?.vendorName,
        remarks: form.remarks,
        lines: form.lines.map((l) => ({
          ...l,
          vendorCode: form.vendorCode,
          vendorName: vendor?.vendorName,
          totalCost: Number(l.quantity || 0) * Number(l.unitCost || 0),
        })),
      };
      const res = await apiPost('/prs', payload);
      const createdRows = Array.isArray(res) ? res : res?.created || [];
      const createdNumbers = createdRows.map((row) => row.prNumber).filter(Boolean);
      success(`Created ${createdNumbers.join(', ') || res?.prNumber || 'PR'}`);
      setForm({ projectCode: form.projectCode, vendorCode: form.vendorCode, remarks: '', lines: [emptyLine()] });
    } catch (err) {
      error(err.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <PageHeader
        title="PR Generation"
        subtitle="Create purchase requests with item lines. Amounts auto-total in INR."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PR Generation' },
        ]}
      />
      <Paper sx={{ p: 2.5 }} className="page-fade">
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <TextField
              select
              label="Project"
              required
              fullWidth
              value={form.projectCode}
              onChange={(e) => setForm((p) => ({ ...p, projectCode: e.target.value }))}
            >
              {projects.map((p) => (
                <MenuItem key={p.projectCode} value={p.projectCode}>
                  {p.projectCode} — {p.projectName}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              select
              label="Vendor"
              required
              fullWidth
              value={form.vendorCode}
              onChange={(e) => setForm((p) => ({ ...p, vendorCode: e.target.value }))}
            >
              {vendors.map((v) => (
                <MenuItem key={v.vendorCode} value={v.vendorCode}>
                  {v.vendorCode} — {v.vendorName}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
          <TextField
            label="Remarks"
            value={form.remarks}
            onChange={(e) => setForm((p) => ({ ...p, remarks: e.target.value }))}
            fullWidth
            multiline
            minRows={2}
          />

          <Stack direction="row" justifyContent="space-between" alignItems="center">
            <Typography variant="h6">Lines</Typography>
            <Button
              startIcon={<AddIcon />}
              onClick={() => setForm((p) => ({ ...p, lines: [...p.lines, emptyLine()] }))}
            >
              Add line
            </Button>
          </Stack>

          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Item</TableCell>
                <TableCell>Description</TableCell>
                <TableCell width={100}>Qty</TableCell>
                <TableCell width={90}>UOM</TableCell>
                <TableCell width={120}>Unit cost</TableCell>
                <TableCell width={120}>Line total</TableCell>
                <TableCell width={48} />
              </TableRow>
            </TableHead>
            <TableBody>
              {form.lines.map((line, idx) => (
                <TableRow key={idx}>
                  <TableCell>
                    <TextField
                      select
                      size="small"
                      fullWidth
                      value={line.itemCode}
                      onChange={(e) => onItemPick(idx, e.target.value)}
                    >
                      {items.map((it) => (
                        <MenuItem key={it.itemCode} value={it.itemCode}>
                          {it.itemCode}
                        </MenuItem>
                      ))}
                    </TextField>
                  </TableCell>
                  <TableCell>
                    <TextField
                      size="small"
                      fullWidth
                      value={line.itemDescription}
                      onChange={(e) => setLine(idx, { itemDescription: e.target.value })}
                    />
                  </TableCell>
                  <TableCell>
                    <TextField
                      size="small"
                      type="number"
                      value={line.quantity}
                      onChange={(e) => setLine(idx, { quantity: e.target.value })}
                    />
                  </TableCell>
                  <TableCell>
                    <TextField
                      size="small"
                      value={line.uom}
                      onChange={(e) => setLine(idx, { uom: e.target.value })}
                    />
                  </TableCell>
                  <TableCell>
                    <TextField
                      size="small"
                      type="number"
                      value={line.unitCost}
                      onChange={(e) => setLine(idx, { unitCost: e.target.value })}
                    />
                  </TableCell>
                  <TableCell>{formatINR(Number(line.quantity || 0) * Number(line.unitCost || 0))}</TableCell>
                  <TableCell>
                    <IconButton
                      disabled={form.lines.length === 1}
                      onClick={() =>
                        setForm((p) => ({ ...p, lines: p.lines.filter((_, i) => i !== idx) }))
                      }
                    >
                      <DeleteOutlinedIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

          <Stack direction="row" justifyContent="space-between" alignItems="center">
            <Typography variant="h6">Total: {formatINR(total)}</Typography>
            <Button variant="contained" onClick={submit} disabled={saving}>
              {saving ? 'Creating…' : 'Create PR'}
            </Button>
          </Stack>
        </Stack>
      </Paper>
    </Box>
  );
}
