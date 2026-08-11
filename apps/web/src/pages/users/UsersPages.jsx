import { useState } from 'react';
import { Box, Button, Paper, Stack, TextField, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import ResourcePage from '../../components/ResourcePage';
import PageHeader from '../../components/PageHeader';
import { apiPost } from '../../api/client';
import { useSnackbar } from '../../components/SnackbarProvider';
import { useAuth } from '../../auth/AuthContext';

export function UsersPage() {
  return (
    <ResourcePage
      title="Users"
      subtitle="Application users and role flags."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Users' }]}
      endpoints={['/masters/users']}
      createEndpoint="/masters/users"
      allowCreate={false}
      createLabel="Add user"
      columns={[
        { field: 'username', header: 'Username' },
        { field: 'displayName', header: 'Name', getValue: (r) => r.displayName || r.name },
        { field: 'role', header: 'Role' },
        { field: 'department', header: 'Department' },
        {
          field: 'permissions',
          header: 'Permissions',
          getValue: (row) =>
            row.permissions
              ? `${Object.values(row.permissions).filter(Boolean).length} enabled`
              : 'Legacy demo role',
        },
        { field: 'active', header: 'Active', render: (v) => (v === 0 || v === false ? 'No' : 'Yes') },
      ]}
      fields={[
        { name: 'username', label: 'Username', required: true },
        { name: 'displayName', label: 'Display name', required: true },
        { name: 'password', label: 'Password', type: 'password' },
        { name: 'role', label: 'Role', defaultValue: 'User' },
        { name: 'department', label: 'Department' },
      ]}
    />
  );
}

export function ChangePasswordPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { success, error } = useSnackbar();
  const [form, setForm] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' });
  const [busy, setBusy] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (form.newPassword !== form.confirmPassword) {
      error('New passwords do not match');
      return;
    }
    setBusy(true);
    try {
      await apiPost('/auth/change-password', {
        currentPassword: form.currentPassword,
        newPassword: form.newPassword,
      });
      success('Password updated. Sign in with the new password.');
      setForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
      logout();
      navigate('/login', { replace: true });
    } catch (err) {
      error(err.message || 'Unable to change password');
    } finally {
      setBusy(false);
    }
  };

  return (
    <Box>
      <PageHeader
        title="Change Password"
        subtitle={`Update password for ${user?.username || 'current user'}.`}
        crumbs={[{ label: 'Home', to: '/' }, { label: 'Users', to: '/users' }, { label: 'Change Password' }]}
      />
      <Paper sx={{ p: 3, maxWidth: 480 }} className="page-fade">
        <Box component="form" onSubmit={submit}>
          <Stack spacing={2}>
            <TextField
              type="password"
              label="Current password"
              required
              value={form.currentPassword}
              onChange={(e) => setForm((p) => ({ ...p, currentPassword: e.target.value }))}
              fullWidth
            />
            <TextField
              type="password"
              label="New password"
              required
              value={form.newPassword}
              onChange={(e) => setForm((p) => ({ ...p, newPassword: e.target.value }))}
              fullWidth
            />
            <TextField
              type="password"
              label="Confirm new password"
              required
              value={form.confirmPassword}
              onChange={(e) => setForm((p) => ({ ...p, confirmPassword: e.target.value }))}
              fullWidth
            />
            <Button type="submit" variant="contained" disabled={busy}>
              {busy ? 'Saving…' : 'Update password'}
            </Button>
            <Typography variant="caption" color="text.secondary">
              Use exactly 12 characters with uppercase, lowercase, number, and special character.
            </Typography>
          </Stack>
        </Box>
      </Paper>
    </Box>
  );
}
