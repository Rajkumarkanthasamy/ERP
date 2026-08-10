import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Button,
  Grid,
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
import PageHeader from '../components/PageHeader';
import LoadingBlock from '../components/LoadingBlock';
import EmptyState from '../components/EmptyState';
import StatusChip from '../components/StatusChip';
import { apiGet } from '../api/client';
import { formatDate, formatINR } from '../utils/format';
import { useAuth } from '../auth/AuthContext';

export default function DashboardPage() {
  const { user } = useAuth();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let alive = true;
    (async () => {
      try {
        const res = await apiGet('/dashboard');
        if (alive) setData(res);
      } catch (err) {
        if (alive) setError(err.message || 'Failed to load dashboard');
      } finally {
        if (alive) setLoading(false);
      }
    })();
    return () => {
      alive = false;
    };
  }, []);

  if (loading) return <LoadingBlock label="Loading dashboard…" />;
  if (error) {
    return (
      <EmptyState title="Dashboard unavailable" description={error} actionLabel="Reload" onAction={() => window.location.reload()} />
    );
  }

  const metrics = data?.metrics || [];

  return (
    <Box>
      <PageHeader
        title={`Welcome, ${user?.displayName || user?.username || 'User'}`}
        subtitle="Operational overview across procurement and ERP modules."
        actions={[
          { label: 'Procurement', component: RouterLink, to: '/procurement', variant: 'contained' },
          { label: 'PR Approval', component: RouterLink, to: '/procurement/pr-approval', variant: 'outlined' },
        ]}
      />

      <Grid container spacing={2} sx={{ mb: 3 }}>
        {metrics.map((m, idx) => (
          <Grid key={m.metric} size={{ xs: 12, sm: 6, md: 3 }}>
            <Paper
              className="metric-pop"
              sx={{
                p: 2.25,
                height: '100%',
                background: 'linear-gradient(160deg, #ffffff 0%, #eef8f6 100%)',
                animationDelay: `${idx * 60}ms`,
              }}
            >
              <Typography variant="caption" color="text.secondary" fontWeight={700}>
                {m.metric}
              </Typography>
              <Typography variant="h4" sx={{ mt: 0.5, fontFamily: 'var(--font-serif)' }}>
                {m.count ?? 0}
              </Typography>
              <Typography color="text.secondary">{formatINR(m.amount)}</Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, lg: 7 }}>
          <Paper sx={{ p: 2 }}>
            <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 1.5 }}>
              <Typography variant="h6">Aging PRs</Typography>
              <Button component={RouterLink} to="/procurement/pr-approval" size="small">
                Review
              </Button>
            </Stack>
            {(data?.agingPRs || []).length === 0 ? (
              <EmptyState title="No aging PRs" description="Pending purchase requests will appear here." />
            ) : (
              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>PR</TableCell>
                      <TableCell>Project</TableCell>
                      <TableCell>Vendor</TableCell>
                      <TableCell>Amount</TableCell>
                      <TableCell>Age</TableCell>
                      <TableCell>Status</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {data.agingPRs.map((pr) => (
                      <TableRow key={pr.prNumber} hover>
                        <TableCell>{pr.prNumber}</TableCell>
                        <TableCell>{pr.projectCode}</TableCell>
                        <TableCell>{pr.vendorCode}</TableCell>
                        <TableCell>{formatINR(pr.totalAmount)}</TableCell>
                        <TableCell>{pr.ageDays}d</TableCell>
                        <TableCell>
                          <StatusChip status={pr.status} />
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </Paper>
        </Grid>
        <Grid size={{ xs: 12, lg: 5 }}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" sx={{ mb: 1.5 }}>
              Recent activity
            </Typography>
            {(data?.recentActivity || []).length === 0 ? (
              <EmptyState title="No recent activity" />
            ) : (
              <Stack spacing={1.25}>
                {data.recentActivity.map((a, i) => (
                  <Box
                    key={`${a.refNo}-${i}`}
                    sx={{
                      p: 1.25,
                      borderRadius: 2,
                      border: '1px solid',
                      borderColor: 'divider',
                      bgcolor: 'rgba(255,255,255,0.7)',
                    }}
                  >
                    <Stack direction="row" justifyContent="space-between" spacing={1}>
                      <Typography fontWeight={700}>
                        {a.type} · {a.refNo}
                      </Typography>
                      <StatusChip status={a.status} />
                    </Stack>
                    <Typography variant="body2" color="text.secondary">
                      {a.details || '—'}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {a.byUser || 'system'} · {formatDate(a.activityDate)}
                    </Typography>
                  </Box>
                ))}
              </Stack>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
