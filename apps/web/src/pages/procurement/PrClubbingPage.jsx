import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Box,
  Button,
  Checkbox,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import { apiGet, apiPost } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function PrClubbingPage() {
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [selected, setSelected] = useState([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await apiGet('/prs', { status: 'Approved' });
      const list = (Array.isArray(data) ? data : []).filter((r) => !r.isClubbed);
      setRows(list);
      setSelected([]);
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

  const total = useMemo(
    () => rows.filter((r) => selected.includes(r.prNumber)).reduce((s, r) => s + Number(r.totalAmount || 0), 0),
    [rows, selected]
  );

  const toggle = (prNumber) => {
    setSelected((prev) => (prev.includes(prNumber) ? prev.filter((x) => x !== prNumber) : [...prev, prNumber]));
  };

  const club = async () => {
    if (selected.length < 2) {
      error('Select at least two PRs');
      return;
    }
    setBusy(true);
    try {
      const res = await apiPost('/prs/club', { prNumbers: selected });
      success(`Clubbed into ${res?.prNumber || 'new PR'}`);
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
        title="PR Clubbing"
        subtitle="Combine approved PRs for the same vendor/project within policy limits."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'PR Clubbing' },
        ]}
        actions={[
          {
            label: busy ? 'Clubbing…' : `Club selected (${selected.length})`,
            variant: 'contained',
            onClick: club,
            disabled: busy || selected.length < 2,
          },
        ]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Typography>
          Selected total: <strong>{formatINR(total)}</strong>
        </Typography>
      </Paper>
      {loading ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="No club-able PRs" description="Approved, non-clubbed PRs will appear here." />
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
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((pr) => (
                <TableRow key={pr.prNumber} hover selected={selected.includes(pr.prNumber)}>
                  <TableCell padding="checkbox">
                    <Checkbox checked={selected.includes(pr.prNumber)} onChange={() => toggle(pr.prNumber)} />
                  </TableCell>
                  <TableCell>{pr.prNumber}</TableCell>
                  <TableCell>{pr.projectCode}</TableCell>
                  <TableCell>{pr.vendorName || pr.vendorCode}</TableCell>
                  <TableCell>{formatINR(pr.totalAmount)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
        <Button variant="outlined" onClick={load}>
          Refresh
        </Button>
      </Stack>
    </Box>
  );
}
