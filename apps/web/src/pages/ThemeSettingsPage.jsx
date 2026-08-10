import {
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  FormControl,
  FormControlLabel,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Switch,
  TextField,
  Typography,
} from '@mui/material'
import PaletteOutlinedIcon from '@mui/icons-material/PaletteOutlined'
import RestartAltIcon from '@mui/icons-material/RestartAlt'
import TextFieldsOutlinedIcon from '@mui/icons-material/TextFieldsOutlined'
import PageHeader from '../components/PageHeader'
import { useThemeSettings } from '../theme/ThemeSettingsContext'
import { COLOR_PRESETS, FONT_FAMILIES, FONT_SIZES } from '../theme/presets'

function ColorField({ label, value, onChange, helper }) {
  return (
    <Stack spacing={0.75}>
      <Stack direction="row" spacing={1.5} alignItems="center">
        <Box
          component="input"
          type="color"
          value={value || '#1565c0'}
          onChange={(e) => onChange(e.target.value)}
          sx={{
            width: 44,
            height: 36,
            border: '1px solid',
            borderColor: 'divider',
            borderRadius: 1,
            p: 0.25,
            bgcolor: 'background.paper',
            cursor: 'pointer',
          }}
        />
        <TextField
          size="small"
          label={label}
          value={value || ''}
          placeholder="Use preset"
          onChange={(e) => onChange(e.target.value)}
          sx={{ flex: 1 }}
        />
      </Stack>
      {helper && (
        <Typography variant="caption" color="text.secondary">
          {helper}
        </Typography>
      )}
    </Stack>
  )
}

export default function ThemeSettingsPage() {
  const { settings, colors, updateSettings, setPreset, resetSettings } = useThemeSettings()
  const presets = Object.values(COLOR_PRESETS)

  return (
    <Box>
      <PageHeader
        title="Theme & Typography"
        subtitle="Customize application colors, font family, and text size. Changes apply across all pages and persist in this browser."
        actions={[
          {
            label: 'Reset defaults',
            startIcon: <RestartAltIcon />,
            variant: 'outlined',
            onClick: resetSettings,
          },
        ]}
      />

      <Grid container spacing={2.5}>
        <Grid size={{ xs: 12, md: 7 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '40ms' }}>
            <CardContent>
              <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 2 }}>
                <PaletteOutlinedIcon color="primary" />
                <Typography variant="h6">Color theme</Typography>
              </Stack>

              <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
                Quick presets
              </Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap sx={{ mb: 2.5 }}>
                {presets.map((preset) => (
                  <Button
                    key={preset.id}
                    size="small"
                    variant={settings.presetId === preset.id && !settings.customPrimary ? 'contained' : 'outlined'}
                    onClick={() => setPreset(preset.id)}
                    startIcon={
                      <Box
                        sx={{
                          width: 14,
                          height: 14,
                          borderRadius: '50%',
                          bgcolor: preset.primary,
                          border: '1px solid rgba(0,0,0,0.12)',
                        }}
                      />
                    }
                  >
                    {preset.label}
                  </Button>
                ))}
              </Stack>

              <Divider sx={{ mb: 2 }} />
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
                Fine-tune colors (optional overrides)
              </Typography>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <ColorField
                    label="Primary override"
                    value={settings.customPrimary}
                    onChange={(v) => updateSettings({ customPrimary: v })}
                    helper={`Active: ${colors.primary}`}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <ColorField
                    label="Secondary override"
                    value={settings.customSecondary}
                    onChange={(v) => updateSettings({ customSecondary: v })}
                    helper={`Active: ${colors.secondary}`}
                  />
                </Grid>
              </Grid>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mt: 2 }}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={settings.rounded !== false}
                      onChange={(e) => updateSettings({ rounded: e.target.checked })}
                    />
                  }
                  label="Rounded corners"
                />
                <FormControlLabel
                  control={
                    <Switch
                      checked={Boolean(settings.dense)}
                      onChange={(e) => updateSettings({ dense: e.target.checked })}
                    />
                  }
                  label="Compact density"
                />
              </Stack>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 5 }}>
          <Card className="erp-card-enter" sx={{ animationDelay: '100ms', mb: 2.5 }}>
            <CardContent>
              <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 2 }}>
                <TextFieldsOutlinedIcon color="primary" />
                <Typography variant="h6">Typography</Typography>
              </Stack>
              <FormControl fullWidth size="small" sx={{ mb: 2.5 }}>
                <InputLabel>Font family</InputLabel>
                <Select
                  label="Font family"
                  value={settings.fontFamilyId}
                  onChange={(e) => updateSettings({ fontFamilyId: e.target.value })}
                >
                  {FONT_FAMILIES.map((f) => (
                    <MenuItem key={f.id} value={f.id} sx={{ fontFamily: f.sans }}>
                      {f.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>

              <FormControl fullWidth size="small">
                <InputLabel>Text size</InputLabel>
                <Select
                  label="Text size"
                  value={settings.fontSizeId}
                  onChange={(e) => updateSettings({ fontSizeId: e.target.value })}
                >
                  {FONT_SIZES.map((s) => (
                    <MenuItem key={s.id} value={s.id}>
                      {s.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 1.5 }}>
                Affects body text, tables, forms, and buttons across the ERP.
              </Typography>
            </CardContent>
          </Card>

          <Card className="erp-card-enter" sx={{ animationDelay: '160ms' }}>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 1 }}>
                Live preview
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Sample content using your current theme.
              </Typography>
              <Stack spacing={1.5}>
                <Typography variant="h4">Dashboard heading</Typography>
                <Typography variant="body1">
                  Purchase requisitions, purchase orders, and GRN workflows use this typography.
                </Typography>
                <Stack direction="row" spacing={1}>
                  <Button variant="contained">Primary action</Button>
                  <Button variant="outlined">Secondary</Button>
                </Stack>
                <Box
                  sx={{
                    mt: 1,
                    p: 2,
                    borderRadius: 2,
                    bgcolor: 'primary.main',
                    color: 'primary.contrastText',
                  }}
                >
                  KPI card sample — Total Sales
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  )
}
