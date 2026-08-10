import { Box, CircularProgress, Typography } from '@mui/material';

export default function LoadingBlock({ label = 'Loading…' }) {
  return (
    <Box sx={{ display: 'grid', placeItems: 'center', py: 8, gap: 1.5 }}>
      <CircularProgress size={36} />
      <Typography color="text.secondary">{label}</Typography>
    </Box>
  );
}
