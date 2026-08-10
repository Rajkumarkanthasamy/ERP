import InboxOutlinedIcon from '@mui/icons-material/InboxOutlined';
import { Box, Button, Typography } from '@mui/material';

export default function EmptyState({ title = 'No records found', description, actionLabel, onAction }) {
  return (
    <Box
      sx={{
        py: 6,
        px: 2,
        textAlign: 'center',
        color: 'text.secondary',
        border: '1px dashed',
        borderColor: 'divider',
        borderRadius: 2,
        bgcolor: 'rgba(255,255,255,0.55)',
      }}
    >
      <InboxOutlinedIcon sx={{ fontSize: 42, opacity: 0.55, mb: 1 }} />
      <Typography variant="h6" color="text.primary">
        {title}
      </Typography>
      {description && (
        <Typography sx={{ mt: 0.5, mb: 2, maxWidth: 420, mx: 'auto' }}>{description}</Typography>
      )}
      {actionLabel && onAction && (
        <Button variant="contained" onClick={onAction}>
          {actionLabel}
        </Button>
      )}
    </Box>
  );
}
