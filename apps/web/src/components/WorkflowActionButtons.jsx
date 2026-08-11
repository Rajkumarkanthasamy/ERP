import { useState } from 'react';
import { Button, Stack } from '@mui/material';
import { apiPost } from '../api/client';
import { useSnackbar } from './SnackbarProvider';

export default function WorkflowActionButtons({
  endpoint,
  actions,
  onComplete,
  size = 'small',
}) {
  const { success, error } = useSnackbar();
  const [running, setRunning] = useState('');

  const run = async (action) => {
    if (action.confirm && !window.confirm(action.confirm)) return;
    let prompted = {};
    if (action.prompt) {
      const value = window.prompt(action.prompt.label, action.prompt.defaultValue || '');
      if (value == null) return;
      if (action.prompt.required && !value.trim()) {
        error(`${action.prompt.fieldLabel || action.prompt.field} is required`);
        return;
      }
      prompted = { [action.prompt.field]: value.trim() };
    }
    setRunning(action.value);
    try {
      await apiPost(endpoint, {
        action: action.value,
        ...(action.body || {}),
        ...prompted,
      });
      success(action.successMessage || `${action.label} completed`);
      await onComplete?.();
    } catch (err) {
      error(err.message || `${action.label} failed`);
    } finally {
      setRunning('');
    }
  };

  return (
    <Stack direction="row" spacing={0.5} justifyContent="flex-end">
      {actions.map((action) => (
        <Button
          key={action.value}
          size={size}
          color={action.color || 'primary'}
          variant={action.variant || 'text'}
          disabled={Boolean(running) || action.disabled}
          onClick={() => run(action)}
        >
          {running === action.value ? 'Working…' : action.label}
        </Button>
      ))}
    </Stack>
  );
}
