import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { Box, Button, Card, CardContent, Grid, Stack, Typography } from '@mui/material';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import ApprovalOutlinedIcon from '@mui/icons-material/ApprovalOutlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import { apiGet } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useThemeSettings } from '../../theme/ThemeSettingsContext';

const LINKS = [
  { to: '/procurement/kanban', label: 'Kanban Board', icon: AccountTreeOutlinedIcon },
  { to: '/procurement/pr-generation', label: 'Create PR', icon: ShoppingCartOutlinedIcon },
  { to: '/procurement/pr-approval', label: 'Approve PRs', icon: ApprovalOutlinedIcon },
  { to: '/procurement/pr-to-po', label: 'PR → PO', icon: ShoppingCartOutlinedIcon },
  { to: '/procurement/po-approval', label: 'Approve POs', icon: ApprovalOutlinedIcon },
  { to: '/procurement/grn', label: 'Create GRN', icon: LocalShippingOutlinedIcon },
];

export default function ProcurementDashboardPage() {
  const { colors } = useThemeSettings();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const kpi = colors.kpi || ['#0d47a1', '#1976d2', '#00838f', '#455a64'];

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
            <Card
              className="metric-pop"
              sx={{
                animationDelay: `${idx * 50}ms`,
                bgcolor: kpi[idx % kpi.length],
                color: '#fff',
                border: 'none',
                backgroundImage: `linear-gradient(145deg, ${kpi[idx % kpi.length]}, ${kpi[idx % kpi.length]}cc)`,
              }}
            >
              <CardContent>
                <Typography variant="caption" sx={{ opacity: 0.85, fontWeight: 700 }}>
                  {m.metric}
                </Typography>
                <Typography variant="h4" sx={{ fontFamily: 'var(--font-serif)', mt: 0.5 }}>
                  {m.count}
                </Typography>
                <Typography sx={{ opacity: 0.85 }}>{formatINR(m.amount)}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      <Card className="erp-card-enter">
        <CardContent>
          <Typography variant="h6" sx={{ mb: 1.5 }}>
            Quick actions
          </Typography>
          <Stack direction="row" flexWrap="wrap" useFlexGap spacing={1}>
            {LINKS.map((l) => {
              const Icon = l.icon;
              return (
                <Button
                  key={l.to}
                  component={RouterLink}
                  to={l.to}
                  variant="outlined"
                  startIcon={<Icon />}
                >
                  {l.label}
                </Button>
              );
            })}
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
}
