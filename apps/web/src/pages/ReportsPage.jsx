import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Box, Button, Grid, Paper, Stack, Typography } from '@mui/material';
import PageHeader from '../components/PageHeader';
import LoadingBlock from '../components/LoadingBlock';
import { apiGet } from '../api/client';
import { formatINR } from '../utils/format';

const REPORT_LINKS = [
  { to: '/procurement/po-status', label: 'PO Status' },
  { to: '/procurement/price-variance', label: 'Price Variance' },
  { to: '/stores/stock-ledger', label: 'Stock Ledger' },
  { to: '/quality/nc', label: 'NC Register' },
  { to: '/sales/enquiries', label: 'Enquiry Register' },
  { to: '/timesheets', label: 'Timesheet' },
];

export default function ReportsPage() {
  const [metrics, setMetrics] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiGet('/dashboard')
      .then((d) => setMetrics(d?.metrics || []))
      .catch(() => setMetrics([]))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Box>
      <PageHeader
        title="Reports Summary"
        subtitle="Operational KPIs and shortcuts into detailed registers."
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Reports' }]}
      />
      {loading ? (
        <LoadingBlock />
      ) : (
        <Grid container spacing={2} sx={{ mb: 3 }}>
          {metrics.map((m, idx) => (
            <Grid key={m.metric} size={{ xs: 12, sm: 6, md: 3 }}>
              <Paper className="metric-pop" sx={{ p: 2, animationDelay: `${idx * 50}ms` }}>
                <Typography variant="caption" color="text.secondary" fontWeight={700}>
                  {m.metric}
                </Typography>
                <Typography variant="h4" sx={{ fontFamily: 'var(--font-serif)' }}>
                  {m.count}
                </Typography>
                <Typography color="text.secondary">{formatINR(m.amount)}</Typography>
              </Paper>
            </Grid>
          ))}
        </Grid>
      )}
      <Paper sx={{ p: 2.5 }}>
        <Typography variant="h6" sx={{ mb: 1.5 }}>
          Report shortcuts
        </Typography>
        <Stack direction="row" flexWrap="wrap" useFlexGap spacing={1}>
          {REPORT_LINKS.map((l) => (
            <Button key={l.to} component={RouterLink} to={l.to} variant="outlined">
              {l.label}
            </Button>
          ))}
        </Stack>
      </Paper>
    </Box>
  );
}
