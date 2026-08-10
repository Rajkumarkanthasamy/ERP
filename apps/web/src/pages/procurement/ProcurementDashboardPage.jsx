import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Box, Button, Grid, Paper, Stack, Typography } from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import { apiGet } from '../../api/client';
import { formatINR } from '../../utils/format';

const LINKS = [
  { to: '/procurement/kanban', label: 'Kanban Board' },
  { to: '/procurement/pr-generation', label: 'Create PR' },
  { to: '/procurement/pr-approval', label: 'Approve PRs' },
  { to: '/procurement/pr-to-po', label: 'PR → PO' },
  { to: '/procurement/po-approval', label: 'Approve POs' },
  { to: '/procurement/grn', label: 'Create GRN' },
];

export default function ProcurementDashboardPage() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    apiGet('/dashboard')
      .then(setData)
      .catch(() => setData(null))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingBlock />;

  return (
    <Box>
      <PageHeader
        title="Procurement Dashboard"
        subtitle="Track PR → PO → GRN flow with approvals, clubbing, and variance controls."
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Procurement' }]}
      />
      <Grid container spacing={2} sx={{ mb: 3 }}>
        {(data?.metrics || []).map((m, idx) => (
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
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6" sx={{ mb: 1.5 }}>
          Quick actions
        </Typography>
        <Stack direction="row" flexWrap="wrap" useFlexGap spacing={1}>
          {LINKS.map((l) => (
            <Button key={l.to} component={RouterLink} to={l.to} variant="outlined">
              {l.label}
            </Button>
          ))}
        </Stack>
      </Paper>
    </Box>
  );
}
