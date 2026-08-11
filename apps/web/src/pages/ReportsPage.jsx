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
  Chip,
} from '@mui/material';
import PageHeader from '../components/PageHeader';
import LoadingBlock from '../components/LoadingBlock';
import EmptyState from '../components/EmptyState';
import { apiGet } from '../api/client';
import { useSnackbar } from '../components/SnackbarProvider';

export default function ReportsPage() {
  const { error } = useSnackbar();
  const [catalog, setCatalog] = useState([]);
  const [category, setCategory] = useState('All');
  const [selectedId, setSelectedId] = useState('');
  const [q, setQ] = useState('');
  const [result, setResult] = useState(null);
  const [loadingCatalog, setLoadingCatalog] = useState(true);
  const [loadingRun, setLoadingRun] = useState(false);

  useEffect(() => {
    apiGet('/reports')
      .then((data) => setCatalog(Array.isArray(data) ? data : []))
      .catch((err) => {
        error(err.message);
        setCatalog([]);
      })
      .finally(() => setLoadingCatalog(false));
  }, [error]);

  const categories = useMemo(
    () => ['All', ...Array.from(new Set(catalog.map((r) => r.category))).sort()],
    [catalog]
  );

  const filtered = useMemo(
    () => catalog.filter((r) => category === 'All' || r.category === category),
    [catalog, category]
  );

  const run = useCallback(async () => {
    if (!selectedId) return;
    setLoadingRun(true);
    setResult(null);
    try {
      const data = await apiGet(`/reports/${encodeURIComponent(selectedId)}`, { q });
      setResult(data);
    } catch (err) {
      error(err.message);
      setResult(null);
    } finally {
      setLoadingRun(false);
    }
  }, [selectedId, q, error]);

  const columns = useMemo(() => {
    const first = result?.rows?.[0];
    return first ? Object.keys(first) : [];
  }, [result]);

  return (
    <Box>
      <PageHeader
        title="ERP Reports"
        subtitle="Legacy report catalog with live SQL adapters for mapped reports."
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Reports' }]}
      />
      {loadingCatalog ? (
        <LoadingBlock />
      ) : (
        <Paper sx={{ p: 2, mb: 2 }}>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={1.5}>
            <TextField
              select
              size="small"
              label="Category"
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              sx={{ minWidth: 160 }}
            >
              {categories.map((c) => (
                <MenuItem key={c} value={c}>
                  {c}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              select
              size="small"
              label="Report"
              value={selectedId}
              onChange={(e) => setSelectedId(e.target.value)}
              sx={{ flex: 1, minWidth: 240 }}
            >
              {filtered.map((r) => (
                <MenuItem key={r.id} value={r.id}>
                  {r.name} ({r.status})
                </MenuItem>
              ))}
            </TextField>
            <TextField size="small" label="Filter" value={q} onChange={(e) => setQ(e.target.value)} sx={{ minWidth: 160 }} />
            <Button variant="contained" onClick={run} disabled={!selectedId || loadingRun}>
              Run
            </Button>
          </Stack>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1.5 }}>
            {catalog.filter((r) => r.status === 'live').length} of {catalog.length} reports mapped to live SQL.
            Unmapped reports return 501 until their queries are ported.
          </Typography>
        </Paper>
      )}

      {loadingRun ? (
        <LoadingBlock />
      ) : result ? (
        <Box>
          <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1 }}>
            <Typography variant="h6">{result.report?.name}</Typography>
            <Chip size="small" label={`${result.rowCount} rows`} />
          </Stack>
          {result.rows?.length ? (
            <TableContainer component={Paper} sx={{ maxHeight: 520 }}>
              <Table size="small" stickyHeader>
                <TableHead>
                  <TableRow>
                    {columns.map((col) => (
                      <TableCell key={col}>{col}</TableCell>
                    ))}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {result.rows.map((row, idx) => (
                    <TableRow key={idx}>
                      {columns.map((col) => (
                        <TableCell key={col}>{row[col] == null ? '—' : String(row[col])}</TableCell>
                      ))}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          ) : (
            <EmptyState title="No rows" />
          )}
        </Box>
      ) : (
        !loadingCatalog && (
          <EmptyState title="Select a report" description="Choose a mapped report and click Run." />
        )
      )}
    </Box>
  );
}
