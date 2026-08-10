import { useState } from 'react';
import {
  Box,
  Button,
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
import EmptyState from '../../components/EmptyState';
import StatusChip from '../../components/StatusChip';
import { apiGet } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function PriceVariancePage() {
  const { error } = useSnackbar();
  const [prNumber, setPrNumber] = useState('');
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);

  const load = async () => {
    if (!prNumber.trim()) {
      error('Enter a PR number');
      return;
    }
    setLoading(true);
    try {
      const res = await apiGet(`/pos/variance/${encodeURIComponent(prNumber.trim())}`);
      setData(res);
    } catch (err) {
      setData(null);
      error(err.message);
    } finally {
      setLoading(false);
    }
  };

  const lines = data?.lines || data?.items || (Array.isArray(data) ? data : []);

  return (
    <Box>
      <PageHeader
        title="Price Variance"
        subtitle="Compare PR unit costs vs latest PO / purchase prices."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'Price Variance' },
        ]}
      />
      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <TextField
            size="small"
            label="PR Number"
            value={prNumber}
            onChange={(e) => setPrNumber(e.target.value)}
            sx={{ minWidth: 220 }}
          />
          <Button variant="contained" onClick={load} disabled={loading}>
            {loading ? 'Loading…' : 'Analyze'}
          </Button>
        </Stack>
      </Paper>
      {!data ? (
        <EmptyState title="Enter a PR number" description="Variance analysis will appear here." />
      ) : lines.length === 0 ? (
        <EmptyState title="No variance lines" />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Item</TableCell>
                <TableCell>PR price</TableCell>
                <TableCell>PO / Latest</TableCell>
                <TableCell>Variance %</TableCell>
                <TableCell>Flag</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {lines.map((l, idx) => (
                <TableRow key={l.itemCode || idx}>
                  <TableCell>
                    <Typography fontWeight={600}>{l.itemCode}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {l.itemDescription}
                    </Typography>
                  </TableCell>
                  <TableCell>{formatINR(l.prUnitCost ?? l.unitCost)}</TableCell>
                  <TableCell>{formatINR(l.poUnitPrice ?? l.latestPrice)}</TableCell>
                  <TableCell>{Number(l.variancePct ?? l.variance ?? 0).toFixed(2)}%</TableCell>
                  <TableCell>
                    <StatusChip status={l.flag || (Math.abs(l.variancePct) >= 10 ? 'Alert' : 'Watch')} />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}
