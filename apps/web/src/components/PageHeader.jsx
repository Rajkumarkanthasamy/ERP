import { Box, Breadcrumbs, Button, Stack, Typography } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

export default function PageHeader({ title, subtitle, actions, crumbs = [] }) {
  return (
    <Box sx={{ mb: 2.5 }} className="page-fade">
      {crumbs.length > 0 && (
        <Breadcrumbs sx={{ mb: 1, fontSize: 13 }}>
          {crumbs.map((c) =>
            c.to ? (
              <Typography
                key={c.label}
                component={RouterLink}
                to={c.to}
                color="text.secondary"
                sx={{ textDecoration: 'none', '&:hover': { color: 'primary.main' } }}
              >
                {c.label}
              </Typography>
            ) : (
              <Typography key={c.label} color="text.primary" fontWeight={600}>
                {c.label}
              </Typography>
            )
          )}
        </Breadcrumbs>
      )}
      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        spacing={1.5}
        alignItems={{ xs: 'stretch', sm: 'center' }}
        justifyContent="space-between"
      >
        <Box>
          <Typography variant="h4" sx={{ letterSpacing: '-0.02em' }}>
            {title}
          </Typography>
          {subtitle && (
            <Typography color="text.secondary" sx={{ mt: 0.5, maxWidth: 720 }}>
              {subtitle}
            </Typography>
          )}
        </Box>
        {actions && (
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
            {Array.isArray(actions)
              ? actions.map((a, i) => {
                  if (!a) return null;
                  const { label, key, ...btnProps } = a;
                  return (
                    <Button key={key || label || i} {...btnProps}>
                      {label}
                    </Button>
                  );
                })
              : actions}
          </Stack>
        )}
      </Stack>
    </Box>
  );
}
