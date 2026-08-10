import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import {
  Alert,
  Box,
  Button,
  Chip,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useAuth } from './AuthContext';
import { BISS_LOGO } from '../layout/navConfig';

const DEMOS = [
  { user: 'admin', pass: 'admin123', role: 'GM / Admin' },
  { user: 'purchase', pass: 'purchase123', role: 'Purchase' },
  { user: 'mh', pass: 'mh123', role: 'MH' },
  { user: 'gm', pass: 'gm123', role: 'GM' },
  { user: 'user', pass: 'user123', role: 'Stores' },
];

export default function LoginPage() {
  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('admin123');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  if (isAuthenticated) return <Navigate to="/" replace />;

  const submit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(username.trim(), password);
      navigate('/', { replace: true });
    } catch (err) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box className="login-shell">
      <Paper
        elevation={0}
        sx={{
          width: '100%',
          maxWidth: 440,
          p: { xs: 3, sm: 4 },
          borderRadius: 3,
          boxShadow: '0 24px 60px rgba(7, 30, 44, 0.35)',
        }}
        className="page-fade"
      >
        <Stack spacing={2.5} alignItems="center">
          <Box
            component="img"
            src={BISS_LOGO}
            alt="BISS"
            sx={{ height: 56, objectFit: 'contain' }}
          />
          <Box textAlign="center">
            <Typography variant="h4" sx={{ fontFamily: 'var(--font-serif)' }}>
              BISS ERP
            </Typography>
            <Typography color="text.secondary">Sign in to ExistERP</Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ width: '100%' }}>
              {error}
            </Alert>
          )}

          <Box component="form" onSubmit={submit} sx={{ width: '100%' }}>
            <Stack spacing={2}>
              <TextField
                label="Username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                autoComplete="username"
                fullWidth
                required
              />
              <TextField
                label="Password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                autoComplete="current-password"
                fullWidth
                required
              />
              <Button type="submit" variant="contained" size="large" disabled={loading}>
                {loading ? 'Signing in…' : 'Sign in'}
              </Button>
            </Stack>
          </Box>

          <Box sx={{ width: '100%' }}>
            <Typography variant="caption" color="text.secondary">
              Demo logins
            </Typography>
            <Stack direction="row" flexWrap="wrap" useFlexGap spacing={1} sx={{ mt: 1 }}>
              {DEMOS.map((d) => (
                <Chip
                  key={d.user}
                  size="small"
                  variant="outlined"
                  label={`${d.user} / ${d.pass}`}
                  onClick={() => {
                    setUsername(d.user);
                    setPassword(d.pass);
                  }}
                  title={d.role}
                />
              ))}
            </Stack>
          </Box>
        </Stack>
      </Paper>
    </Box>
  );
}
