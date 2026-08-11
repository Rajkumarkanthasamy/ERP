import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  cssVariables: true,
  palette: {
    mode: 'light',
    primary: {
      main: '#0f766e',
      dark: '#0b2a3d',
      light: '#14b8a6',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#0b2a3d',
      contrastText: '#ffffff',
    },
    background: {
      default: '#f3f7f8',
      paper: '#ffffff',
    },
    text: {
      primary: '#123047',
      secondary: '#5b7384',
    },
    success: { main: '#027a48' },
    warning: { main: '#b54708' },
    error: { main: '#b42318' },
    info: { main: '#026aa2' },
    divider: '#d5e3ea',
  },
  typography: {
    fontFamily: "'DM Sans', sans-serif",
    h1: { fontFamily: "'Source Serif 4', Georgia, serif", fontWeight: 700 },
    h2: { fontFamily: "'Source Serif 4', Georgia, serif", fontWeight: 700 },
    h3: { fontFamily: "'Source Serif 4', Georgia, serif", fontWeight: 650 },
    h4: { fontFamily: "'Source Serif 4', Georgia, serif", fontWeight: 650 },
    h5: { fontFamily: "'Source Serif 4', Georgia, serif", fontWeight: 600 },
    h6: { fontFamily: "'DM Sans', sans-serif", fontWeight: 700 },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  shape: { borderRadius: 12 },
  components: {
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: { borderRadius: 10 },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          border: '1px solid var(--biss-border)',
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          borderRight: '1px solid var(--biss-border)',
          background:
            'linear-gradient(180deg, #ffffff 0%, #f7fafb 100%)',
        },
      },
    },
    MuiTableHead: {
      styleOverrides: {
        root: {
          '& .MuiTableCell-head': {
            fontWeight: 700,
            background: '#eef5f6',
            color: '#0b2a3d',
          },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { fontWeight: 600 },
      },
    },
  },
});

export default theme;
