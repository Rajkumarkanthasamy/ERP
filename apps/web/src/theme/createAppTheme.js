import { createTheme } from '@mui/material/styles';
import { COLOR_PRESETS, FONT_FAMILIES, FONT_SIZES } from './presets';

export function resolveColors(settings) {
  const preset = COLOR_PRESETS[settings.presetId] || COLOR_PRESETS.oceanBlue;
  return {
    ...preset,
    primary: settings.customPrimary || preset.primary,
    secondary: settings.customSecondary || preset.secondary,
  };
}

export function resolveFont(settings) {
  return FONT_FAMILIES.find((f) => f.id === settings.fontFamilyId) || FONT_FAMILIES[0];
}

export function resolveFontScale(settings) {
  return FONT_SIZES.find((f) => f.id === settings.fontSizeId)?.scale || 1;
}

export function applyCssVars(colors, font, scale) {
  const root = document.documentElement;
  root.style.setProperty('--biss-navy', colors.primaryDark || colors.secondary);
  root.style.setProperty('--biss-teal', colors.primary);
  root.style.setProperty('--biss-teal-bright', colors.accent);
  root.style.setProperty('--biss-sky', colors.background);
  root.style.setProperty('--biss-mist', colors.background);
  root.style.setProperty('--biss-sand', colors.background);
  root.style.setProperty('--biss-ink', colors.text);
  root.style.setProperty('--biss-muted', colors.muted);
  root.style.setProperty('--biss-border', colors.border);
  root.style.setProperty('--biss-danger', colors.error);
  root.style.setProperty('--biss-warning', colors.warning);
  root.style.setProperty('--biss-success', colors.success);
  root.style.setProperty('--font-sans', font.sans);
  root.style.setProperty('--font-serif', font.display);
  root.style.setProperty('--font-scale', String(scale));
  root.style.fontSize = `${16 * scale}px`;
}

export function ensureGoogleFont(font) {
  const id = `biss-font-${font.id}`;
  if (document.getElementById(id)) return;
  const link = document.createElement('link');
  link.id = id;
  link.rel = 'stylesheet';
  link.href = `https://fonts.googleapis.com/css2?${font.google}&display=swap`;
  document.head.appendChild(link);
}

export function createAppTheme(settings) {
  const colors = resolveColors(settings);
  const font = resolveFont(settings);
  const scale = resolveFontScale(settings);
  const radius = settings.rounded === false ? 6 : 12;
  const dense = Boolean(settings.dense);

  return createTheme({
    cssVariables: true,
    palette: {
      mode: 'light',
      primary: {
        main: colors.primary,
        dark: colors.primaryDark || colors.secondary,
        light: colors.accent,
        contrastText: '#ffffff',
      },
      secondary: {
        main: colors.secondary,
        contrastText: '#ffffff',
      },
      background: {
        default: colors.background,
        paper: colors.paper,
      },
      text: {
        primary: colors.text,
        secondary: colors.muted,
      },
      success: { main: colors.success },
      warning: { main: colors.warning },
      error: { main: colors.error },
      info: { main: colors.info },
      divider: colors.border,
    },
    typography: {
      fontFamily: font.sans,
      fontSize: (dense ? 13 : 14) * scale,
      h1: { fontFamily: font.display, fontWeight: 700 },
      h2: { fontFamily: font.display, fontWeight: 700 },
      h3: { fontFamily: font.display, fontWeight: 650 },
      h4: { fontFamily: font.display, fontWeight: 650, letterSpacing: '-0.02em' },
      h5: { fontFamily: font.display, fontWeight: 600 },
      h6: { fontFamily: font.sans, fontWeight: 700 },
      button: { textTransform: 'none', fontWeight: 600 },
    },
    shape: { borderRadius: radius },
    components: {
      MuiButton: {
        defaultProps: { disableElevation: true, size: dense ? 'small' : 'medium' },
        styleOverrides: {
          root: {
            borderRadius: Math.max(radius - 2, 4),
            transition: 'transform 160ms ease, box-shadow 160ms ease, background-color 160ms ease',
            '&:hover': { transform: 'translateY(-1px)' },
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            backgroundImage: 'none',
            border: `1px solid ${colors.border}`,
            boxShadow: '0 8px 24px rgba(16, 42, 67, 0.06)',
            transition: 'box-shadow 200ms ease, transform 200ms ease',
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            border: `1px solid ${colors.border}`,
            boxShadow: '0 8px 24px rgba(16, 42, 67, 0.06)',
            borderRadius: radius,
          },
        },
      },
      MuiDrawer: {
        styleOverrides: {
          paper: {
            borderRight: `1px solid ${colors.border}`,
            background: '#ffffff',
          },
        },
      },
      MuiAppBar: {
        styleOverrides: {
          root: {
            boxShadow: 'none',
            borderBottom: `1px solid ${colors.border}`,
            backgroundColor: 'rgba(255,255,255,0.92)',
            backdropFilter: 'blur(12px)',
          },
        },
      },
      MuiTableHead: {
        styleOverrides: {
          root: {
            '& .MuiTableCell-head': {
              fontWeight: 700,
              background: colors.background,
              color: colors.text,
            },
          },
        },
      },
      MuiTableCell: {
        styleOverrides: {
          root: dense
            ? { paddingTop: 8, paddingBottom: 8 }
            : undefined,
        },
      },
      MuiListItemButton: {
        styleOverrides: {
          root: {
            borderRadius: 10,
            transition: 'background-color 160ms ease, color 160ms ease, transform 160ms ease',
            '&.Mui-selected': {
              backgroundColor: `${colors.primary}18`,
              color: colors.primaryDark || colors.primary,
              borderLeft: `3px solid ${colors.primary}`,
              '&:hover': { backgroundColor: `${colors.primary}24` },
            },
          },
        },
      },
      MuiChip: {
        styleOverrides: {
          root: { fontWeight: 600 },
        },
      },
      MuiCssBaseline: {
        styleOverrides: {
          body: {
            fontFamily: font.sans,
            backgroundColor: colors.background,
          },
        },
      },
    },
  });
}
