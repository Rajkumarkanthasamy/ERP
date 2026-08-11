import { useCallback, useEffect, useMemo, useState } from 'react';
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
  Typography,
} from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import { apiGet, apiPost } from '../../api/client';
import { useSnackbar } from '../../components/SnackbarProvider';

const TYPE_LABELS = {
  tax: 'Tax',
  cst: 'CST',
  'others-tax': 'Others tax',
  discount: 'Discount',
  excise: 'Excise duty',
  payment: 'Payment terms',
  delivery: 'Delivery terms',
};

export default function AdditionalMastersPage() {
  const { success, error } = useSnackbar();
  const [types, setTypes] = useState(Object.keys(TYPE_LABELS));
  const [type, setType] = useState('tax');
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState('');
  const [percentage, setPercentage] = useState(0);
  const [busy, setBusy] = useState(false);

  const needsPct = useMemo(() => !['payment', 'delivery'].includes(type), [type]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const catalog = await apiGet('/additional-masters/types').catch(() => null);
      if (Array.isArray(catalog) && catalog.length) setTypes(catalog);
      const data = await apiGet(`/additional-masters/${encodeURIComponent(type)}`);
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      error(err.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }, [type, error]);

  useEffect(() => {
    load();
  }, [load]);

  const create = async () => {
    if (!name.trim()) {
      error('Name is required');
      return;
    }
    setBusy(true);
    try {
      await apiPost(`/additional-masters/${encodeURIComponent(type)}`, {
        name: name.trim(),
        percentage: needsPct ? Number(percentage) : undefined,
        status: 'Active',
      });
      success('Master saved');
      setName('');
      setPercentage(0);
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
        title="Additional Masters"
        subtitle="Tax, payment, delivery and related PO term masters (live SQL Server)."
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Additional Masters' }]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} alignItems="flex-start">
          <TextField
            select
            size="small"
            label="Type"
            value={type}
            onChange={(e) => setType(e.target.value)}
            sx={{ minWidth: 200 }}
          >
            {types.map((t) => (
              <MenuItem key={t} value={t}>
                {TYPE_LABELS[t] || t}
              </MenuItem>
            ))}
          </TextField>
          <TextField size="small" label="Name" value={name} onChange={(e) => setName(e.target.value)} sx={{ flex: 1 }} />
          {needsPct && (
            <TextField
              size="small"
              type="number"
              label="%"
              value={percentage}
              onChange={(e) => setPercentage(e.target.value)}
              sx={{ width: 120 }}
            />
          )}
          <Button variant="contained" onClick={create} disabled={busy}>
            Add
          </Button>
          <Button variant="outlined" onClick={load}>
            Refresh
          </Button>
        </Stack>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No rows" description="Nothing found for this master type (requires live SQL mode)." />
      ) : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Code</TableCell>
                <TableCell>Name</TableCell>
                {needsPct && <TableCell>%</TableCell>}
                <TableCell>Status</TableCell>
                <TableCell>Created by</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.code || row.name}>
                  <TableCell>{row.code || '—'}</TableCell>
                  <TableCell>{row.name}</TableCell>
                  {needsPct && <TableCell>{row.percentage ?? '—'}</TableCell>}
                  <TableCell>{row.status || '—'}</TableCell>
                  <TableCell>{row.createdBy || '—'}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
        Currency rates: use PEG / currency endpoints under `/api/additional-masters/currency-rates`.
      </Typography>
    </Box>
  );
}
