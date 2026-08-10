import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { ThemeProvider } from '@mui/material/styles';
import {
  COLOR_PRESETS,
  DEFAULT_THEME_SETTINGS,
  FONT_FAMILIES,
  FONT_SIZES,
  STORAGE_KEY,
} from './presets';
import {
  applyCssVars,
  createAppTheme,
  ensureGoogleFont,
  resolveColors,
  resolveFont,
  resolveFontScale,
} from './createAppTheme';

const ThemeSettingsContext = createContext(null);

function loadSettings() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { ...DEFAULT_THEME_SETTINGS };
    return { ...DEFAULT_THEME_SETTINGS, ...JSON.parse(raw) };
  } catch {
    return { ...DEFAULT_THEME_SETTINGS };
  }
}

export function ThemeSettingsProvider({ children }) {
  const [settings, setSettings] = useState(loadSettings);

  const colors = useMemo(() => resolveColors(settings), [settings]);
  const font = useMemo(() => resolveFont(settings), [settings]);
  const scale = useMemo(() => resolveFontScale(settings), [settings]);
  const theme = useMemo(() => createAppTheme(settings), [settings]);

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
    ensureGoogleFont(font);
    applyCssVars(colors, font, scale);
  }, [settings, colors, font, scale]);

  const updateSettings = (patch) => {
    setSettings((prev) => ({ ...prev, ...patch }));
  };

  const setPreset = (presetId) => {
    setSettings((prev) => ({
      ...prev,
      presetId,
      customPrimary: '',
      customSecondary: '',
    }));
  };

  const resetSettings = () => setSettings({ ...DEFAULT_THEME_SETTINGS });

  const value = {
    settings,
    colors,
    font,
    scale,
    presets: Object.values(COLOR_PRESETS),
    fonts: FONT_FAMILIES,
    fontSizes: FONT_SIZES,
    updateSettings,
    setPreset,
    resetSettings,
  };

  return (
    <ThemeSettingsContext.Provider value={value}>
      <ThemeProvider theme={theme}>{children}</ThemeProvider>
    </ThemeSettingsContext.Provider>
  );
}

export function useThemeSettings() {
  const ctx = useContext(ThemeSettingsContext);
  if (!ctx) throw new Error('useThemeSettings must be used within ThemeSettingsProvider');
  return ctx;
}
