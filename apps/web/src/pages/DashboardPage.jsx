import { useEffect, useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Button,
  Card,
  CardContent,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import PaymentsOutlinedIcon from '@mui/icons-material/PaymentsOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined';
import PageHeader from '../components/PageHeader';
import LoadingBlock from '../components/LoadingBlock';
import EmptyState from '../components/EmptyState';
import StatusChip from '../components/StatusChip';
import { apiGet } from '../api/client';
import { formatDate, formatINR } from '../utils/format';
import { useAuth } from '../auth/AuthContext';
import { useThemeSettings } from '../theme/ThemeSettingsContext';

function KpiCard({ title, value, subtitle, icon: Icon, color, delay = 0, light }) {
  return (
    <Card
      className="metric-pop"
      sx={{
        height: '100%',
        animationDelay: `${delay}ms`,
        bgcolor: light ? 'background.paper' : color,
        color: light ? 'text.primary' : '#fff',
        border: light ? undefined : 'none',
        backgroundImage: light
          ? undefined
          : `linear-gradient(145deg, ${color} 0%, ${color}dd 100%)`,
        overflow: 'hidden',
        position: 'relative',
      }}
    >
      <CardContent sx={{ position: 'relative', zIndex: 1 }}>
        <Stack direction="row" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography
              variant="body2"
              sx={{ opacity: light ? 0.7 : 0.85, fontWeight: 600, mb: 0.75 }}
            >
              {title}
            </Typography>
            <Typography variant="h4" sx={{ fontWeight: 700, letterSpacing: '-0.03em' }}>
              {value}
            </Typography>
            {subtitle && (
              <Typography variant="caption" sx={{ opacity: light ? 0.65 : 0.8, mt: 0.5, display: 'block' }}>
                {subtitle}
              </Typography>
            )}
          </Box>
          <Box
            sx={{
              width: 44,
              height: 44,
              borderRadius: 2,
              display: 'grid',
              placeItems: 'center',
              bgcolor: light ? `${color}18` : 'rgba(255,255,255,0.16)',
              color: light ? color : '#fff',
            }}
          >
            <Icon />
          </Box>
        </Stack>
      </CardContent>
      {!light && (
        <Box
          sx={{
            position: 'absolute',
            right: -20,
            bottom: -24,
            width: 110,
            height: 110,
            borderRadius: '50%',
            bgcolor: 'rgba(255,255,255,0.08)',
          }}
        />
      )}
    </Card>
  );
}

function AttendanceRing({ present = 0, absent = 0, leave = 0 }) {
  const total = Math.max(present + absent + leave, 1);
  const pct = Math.round((present / total) * 100);
  const r = 36;
  const c = 2 * Math.PI * r;
  const offset = c - (pct / 100) * c;

  return (
    <Card className="metric-pop" sx={{ height: '100%', animationDelay: '180ms' }}>
      <CardContent>
        <Typography variant="body2" color="text.secondary" fontWeight={600} sx={{ mb: 1 }}>
          Staff / Approvals pulse
        </Typography>
        <Stack direction="row" spacing={2} alignItems="center">
          <Box sx={{ position: 'relative', width: 96, height: 96 }}>
            <svg width="96" height="96" viewBox="0 0 96 96" className="ring-spin">
              <circle cx="48" cy="48" r={r} fill="none" stroke="var(--biss-border)" strokeWidth="8" />
              <circle
                cx="48"
                cy="48"
                r={r}
                fill="none"
                stroke="var(--biss-teal)"
                strokeWidth="8"
                strokeLinecap="round"
                strokeDasharray={c}
                strokeDashoffset={offset}
                transform="rotate(-90 48 48)"
              />
            </svg>
            <Box
              sx={{
                position: 'absolute',
                inset: 0,
                display: 'grid',
                placeItems: 'center',
              }}
            >
              <Typography variant="h6" fontWeight={700}>
                {pct}%
              </Typography>
            </Box>
          </Box>
          <Stack spacing={0.5}>
            <Typography variant="body2">
              <Box component="span" sx={{ color: 'success.main', fontWeight: 700 }}>
                {present}
              </Box>{' '}
              Open PRs
            </Typography>
            <Typography variant="body2">
              <Box component="span" sx={{ color: 'warning.main', fontWeight: 700 }}>
                {absent}
              </Box>{' '}
              Pending POs
            </Typography>
            <Typography variant="body2">
              <Box component="span" sx={{ color: 'info.main', fontWeight: 700 }}>
                {leave}
              </Box>{' '}
              GRNs today
            </Typography>
          </Stack>
        </Stack>
      </CardContent>
    </Card>
  );
}

function LineChart({ seriesA = [], seriesB = [] }) {
  const w = 520;
  const h = 180;
  const pad = 16;
  const max = Math.max(1, ...seriesA, ...seriesB);

  const toPath = (series) =>
    series
      .map((v, i) => {
        const x = pad + (i * (w - pad * 2)) / Math.max(series.length - 1, 1);
        const y = h - pad - (v / max) * (h - pad * 2);
        return `${i === 0 ? 'M' : 'L'}${x},${y}`;
      })
      .join(' ');

  return (
    <Box sx={{ width: '100%', overflow: 'hidden' }}>
      <svg viewBox={`0 0 ${w} ${h}`} width="100%" height="180" className="chart-draw">
        <defs>
          <linearGradient id="lineFill" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor="var(--biss-teal)" stopOpacity="0.22" />
            <stop offset="100%" stopColor="var(--biss-teal)" stopOpacity="0" />
          </linearGradient>
        </defs>
        {[0.25, 0.5, 0.75].map((t) => (
          <line
            key={t}
            x1={pad}
            x2={w - pad}
            y1={pad + t * (h - pad * 2)}
            y2={pad + t * (h - pad * 2)}
            stroke="var(--biss-border)"
            strokeDasharray="4 6"
          />
        ))}
        <path d={`${toPath(seriesA)} L${w - pad},${h - pad} L${pad},${h - pad} Z`} fill="url(#lineFill)" />
        <path d={toPath(seriesA)} fill="none" stroke="var(--biss-teal)" strokeWidth="3" />
        <path d={toPath(seriesB)} fill="none" stroke="#1565c0" strokeWidth="3" strokeLinecap="round" />
      </svg>
    </Box>
  );
}

function BarChart({ values = [] }) {
  const max = Math.max(1, ...values);
  return (
    <Box sx={{ display: 'flex', alignItems: 'flex-end', gap: 1, height: 170, px: 0.5 }}>
      {values.map((v, i) => (
        <Box
          key={i}
          className="bar-rise"
          sx={{
            flex: 1,
            height: `${Math.max(8, (v / max) * 100)}%`,
            borderRadius: '8px 8px 4px 4px',
            background:
              i % 2 === 0
                ? 'linear-gradient(180deg, #42a5f5, #1565c0)'
                : 'linear-gradient(180deg, #80cbc4, #00838f)',
            animationDelay: `${i * 40}ms`,
          }}
        />
      ))}
    </Box>
  );
}

export default function DashboardPage() {
  const { user } = useAuth();
  const { colors } = useThemeSettings();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [range, setRange] = useState('year');

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

  const metricMap = useMemo(() => {
    const map = {};
    for (const m of data?.metrics || []) map[m.metric] = m;
    return map;
  }, [data]);

  const pick = (...keys) => {
    for (const k of keys) {
      if (metricMap[k]) return metricMap[k];
    }
    return null;
  };

  const prMetric = pick('Open PRs', 'Pending PRs', 'PRs', 'Purchase Requests');
  const poMetric = pick('Open POs', 'Pending POs', 'POs', 'Purchase Orders');
  const grnMetric = pick('GRNs', 'GRN Today', 'Goods Receipts');
  const vendorMetric = pick('Vendors', 'Active Vendors', 'Customers');

  const lineA = useMemo(() => {
    const base = [12, 18, 15, 22, 28, 24, 31, 29, 35, 32, 38, 42];
    const factor = range === 'month' ? 0.45 : range === 'quarter' ? 0.7 : 1;
    return base.map((n) => Math.round(n * factor + (prMetric?.count || 4)));
  }, [range, prMetric]);

  const lineB = useMemo(() => {
    const base = [8, 11, 14, 13, 19, 21, 18, 24, 27, 25, 30, 33];
    const factor = range === 'month' ? 0.45 : range === 'quarter' ? 0.7 : 1;
    return base.map((n) => Math.round(n * factor + (poMetric?.count || 3)));
  }, [range, poMetric]);

  const bars = useMemo(() => {
    const seed = (poMetric?.count || 6) + (prMetric?.count || 4);
    return [4, 7, 5, 9, 6, 11, 8, 10, 7, 12, 9, 14].map((n, i) => n + ((seed + i) % 5));
  }, [poMetric, prMetric]);

  if (loading) return <LoadingBlock label="Loading dashboard…" />;
  if (error) {
    return (
      <EmptyState
        title="Dashboard unavailable"
        description={error}
        actionLabel="Reload"
        onAction={() => window.location.reload()}
      />
    );
  }

  const kpiColors = colors.kpi || ['#0d47a1', '#1976d2', '#00838f', '#ffffff'];

  return (
    <Box>
      <PageHeader
        title="Dashboard"
        subtitle={`Welcome, ${user?.displayName || user?.username || 'User'} — operational overview across procurement and ERP modules.`}
        actions={[
          {
            label: '+ Dashboard Widgets',
            component: RouterLink,
            to: '/settings/theme',
            variant: 'contained',
          },
          {
            label: 'Procurement',
            component: RouterLink,
            to: '/procurement',
            variant: 'outlined',
          },
        ]}
      />

      <Grid container spacing={2.25} sx={{ mb: 2.5 }}>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          <KpiCard
            title="Total PR Value"
            value={formatINR(prMetric?.amount || 0)}
            subtitle={`${prMetric?.count ?? 0} open requests`}
            icon={PaymentsOutlinedIcon}
            color={kpiColors[0]}
            delay={0}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          <KpiCard
            title="Total PO Value"
            value={formatINR(poMetric?.amount || 0)}
            subtitle={`${poMetric?.count ?? 0} open orders`}
            icon={ShoppingCartOutlinedIcon}
            color={kpiColors[1]}
            delay={60}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          <KpiCard
            title="Active Partners"
            value={String(vendorMetric?.count ?? data?.metrics?.length ?? 0)}
            subtitle={formatINR(vendorMetric?.amount || 0)}
            icon={GroupsOutlinedIcon}
            color={kpiColors[2]}
            delay={120}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          <AttendanceRing
            present={prMetric?.count ?? 0}
            absent={poMetric?.count ?? 0}
            leave={grnMetric?.count ?? 0}
          />
        </Grid>
      </Grid>

      <Grid container spacing={2.25} sx={{ mb: 2.5 }}>
        <Grid size={{ xs: 12, lg: 7 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '80ms', height: '100%' }}>
            <CardContent>
              <Stack
                direction={{ xs: 'column', sm: 'row' }}
                justifyContent="space-between"
                alignItems={{ xs: 'stretch', sm: 'center' }}
                spacing={1.5}
                sx={{ mb: 1 }}
              >
                <Box>
                  <Typography variant="h6">Units & order trend</Typography>
                  <Typography variant="body2" color="text.secondary">
                    PR volume vs PO conversion
                  </Typography>
                </Box>
                <FormControl size="small" sx={{ minWidth: 140 }}>
                  <InputLabel>Range</InputLabel>
                  <Select
                    label="Range"
                    value={range}
                    onChange={(e) => setRange(e.target.value)}
                  >
                    <MenuItem value="month">This Month</MenuItem>
                    <MenuItem value="quarter">This Quarter</MenuItem>
                    <MenuItem value="year">This Year</MenuItem>
                  </Select>
                </FormControl>
              </Stack>
              <LineChart seriesA={lineA} seriesB={lineB} />
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, lg: 5 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '140ms', height: '100%' }}>
            <CardContent>
              <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                <Box>
                  <Typography variant="h6">Order summary</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Monthly PO activity
                  </Typography>
                </Box>
                <Button size="small" component={RouterLink} to="/procurement/po-status">
                  View
                </Button>
              </Stack>
              <BarChart values={bars} />
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Grid container spacing={2.25}>
        <Grid size={{ xs: 12, lg: 7 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '180ms' }}>
            <CardContent>
              <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 1.5 }}>
                <Typography variant="h6">Sales / PR orders</Typography>
                <Button component={RouterLink} to="/procurement/pr-approval" size="small">
                  View all
                </Button>
              </Stack>
              {(data?.agingPRs || []).length === 0 ? (
                <EmptyState title="No aging PRs" description="Pending purchase requests will appear here." />
              ) : (
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>PR #</TableCell>
                        <TableCell>Project</TableCell>
                        <TableCell>Vendor</TableCell>
                        <TableCell>Amount</TableCell>
                        <TableCell>Age</TableCell>
                        <TableCell>Status</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {data.agingPRs.map((pr) => (
                        <TableRow key={pr.prNumber} hover className="row-fade">
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
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, lg: 5 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '220ms', mb: 2.25 }}>
            <CardContent>
              <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 1.5 }}>
                <Typography variant="h6">Module pulse</Typography>
                <AssignmentOutlinedIcon color="action" fontSize="small" />
              </Stack>
              <Stack spacing={1}>
                {(data?.metrics || []).slice(0, 6).map((m) => (
                  <Stack
                    key={m.metric}
                    direction="row"
                    justifyContent="space-between"
                    alignItems="center"
                    sx={{
                      py: 0.85,
                      px: 1.25,
                      borderRadius: 2,
                      bgcolor: 'background.default',
                    }}
                  >
                    <Typography variant="body2" fontWeight={600}>
                      {m.metric}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {m.count ?? 0} · {formatINR(m.amount)}
                    </Typography>
                  </Stack>
                ))}
              </Stack>
            </CardContent>
          </Card>

          <Card className="erp-card-enter" sx={{ animationDelay: '260ms' }}>
            <CardContent>
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
                      className="row-fade"
                      sx={{
                        p: 1.25,
                        borderRadius: 2,
                        border: '1px solid',
                        borderColor: 'divider',
                        bgcolor: 'rgba(255,255,255,0.7)',
                        animationDelay: `${i * 40}ms`,
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
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
