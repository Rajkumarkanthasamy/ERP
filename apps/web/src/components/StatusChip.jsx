import { Chip } from '@mui/material';
import { statusTone } from '../utils/format';

const colorMap = {
  success: 'success',
  warning: 'warning',
  error: 'error',
  default: 'default',
  info: 'info',
};

export default function StatusChip({ status, label, size = 'small' }) {
  const tone = statusTone(status || label);
  return (
    <Chip
      size={size}
      label={label || status || '—'}
      color={colorMap[tone] || 'default'}
      variant={tone === 'default' ? 'outlined' : 'filled'}
    />
  );
}
