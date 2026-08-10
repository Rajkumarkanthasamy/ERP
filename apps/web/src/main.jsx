import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { CssBaseline } from '@mui/material';
import App from './App.jsx';
import { ThemeSettingsProvider } from './theme/ThemeSettingsContext.jsx';
import { AuthProvider } from './auth/AuthContext.jsx';
import { SnackbarProvider } from './components/SnackbarProvider.jsx';
import './index.css';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <ThemeSettingsProvider>
        <CssBaseline />
        <AuthProvider>
          <SnackbarProvider>
            <App />
          </SnackbarProvider>
        </AuthProvider>
      </ThemeSettingsProvider>
    </BrowserRouter>
  </StrictMode>
);
