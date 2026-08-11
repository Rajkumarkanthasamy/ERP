import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Box,
  Checkbox,
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
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function PrToPoWorkspacePage() {
  const { success, error } = useSnackbar();
  const [ready, setReady] = useState([]);
  const [selected, setSelected] = useState([]);
  const [vendors, setVendors] = useState([]);
  const [vendorCode, setVendorCode] = useState('');
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [prs, v] = await Promise.all([
        apiGet('/pos/ready-prs'),
        apiGet('/masters/vendors').catch(() => []),
      ]);
      setReady(Array.isArray(prs) ? prs : []);
      setVendors(Array.isArray(v) ? v : []);
      setSelected([]);
    } catch (err) {
      error(err.message);
      setReady([]);
    } finally {
      setLoading(false);
    }
  }, [error]);

  useEffect(() => {
    load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!vendorCode) return ready;
    return ready.filter((r) => r.vendorCode === vendorCode);
  }, [ready, vendorCode]);

  const convert = async () => {
    if (!selected.length) {
      error('Select at least one PR');
      return;
    }
    setBusy(true);
    try {
      const res = await apiPost('/pos/convert', { prNumbers: selected });
      const refs = res?.poRefs || (res?.poRef ? [res.poRef] : []);
      success(`Created PO ${refs.join(', ')}`);
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusy(false);
    }
  };

  const toggle = (prNumber) => {
    setSelected((prev) => (prev.includes(prNumber) ? prev.filter((x) => x !== prNumber) : [...prev, prNumber]));
  };

  return (
    <Box>
      <PageHeader
        title="PR → PO Workspace"
        subtitle="Convert approved PR lines into purchase orders."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PR → PO' },
        ]}
        actions={[
          {
            label: busy ? 'Converting…' : 'Convert to PO',
            variant: 'contained',
            onClick: convert,
            disabled: busy || !selected.length,
          },
        ]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <TextField
            select
            size="small"
            label="Filter vendor"
            value={vendorCode}
            onChange={(e) => setVendorCode(e.target.value)}
            sx={{ minWidth: 240 }}
          >
            <MenuItem value="">All vendors</MenuItem>
            {vendors.map((v) => (
              <MenuItem key={v.vendorCode} value={v.vendorCode}>
                {v.vendorName}
              </MenuItem>
            ))}
          </TextField>
          <Typography sx={{ alignSelf: 'center' }}>{selected.length} PR(s) selected</Typography>
        </Stack>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : filtered.length === 0 ? (
        <EmptyState title="No PRs ready for PO" description="Approve PRs first, then convert here." />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell padding="checkbox" />
                <TableCell>PR</TableCell>
                <TableCell>Project</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filtered.map((pr) => (
                <TableRow key={pr.prNumber} hover>
                  <TableCell padding="checkbox">
                    <Checkbox checked={selected.includes(pr.prNumber)} onChange={() => toggle(pr.prNumber)} />
                  </TableCell>
                  <TableCell>{pr.prNumber}</TableCell>
                  <TableCell>{pr.projectCode}</TableCell>
                  <TableCell>{pr.vendorName || pr.vendorCode}</TableCell>
                  <TableCell>{formatINR(pr.totalAmount)}</TableCell>
                  <TableCell>{pr.status}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}
