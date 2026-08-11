import { useEffect, useMemo, useState } from 'react';
import { Link as RouterLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Avatar,
  Badge,
  Box,
  Chip,
  Divider,
  Drawer,
  IconButton,
  InputAdornment,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  TextField,
  Tooltip,
  Typography,
  useMediaQuery,
  Collapse,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import LogoutIcon from '@mui/icons-material/Logout';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import NotificationsNoneOutlinedIcon from '@mui/icons-material/NotificationsNoneOutlined';
import CalendarMonthOutlinedIcon from '@mui/icons-material/CalendarMonthOutlined';
import { useTheme } from '@mui/material/styles';
import { useAuth } from '../auth/AuthContext';
import { useThemeSettings } from '../theme/ThemeSettingsContext';
import { BISS_LOGO, canAccessNavItem, flattenNav, navSections } from './navConfig';

const DRAWER_WIDTH = 280;

function NavContent({ onNavigate, filter, permissions }) {
  const location = useLocation();
  const theme = useTheme();
  const [openSections, setOpenSections] = useState(() =>
    Object.fromEntries(navSections.map((s) => [s.id, true]))
  );

  const filtered = useMemo(() => {
    const q = filter.trim().toLowerCase();
    const allowedSections = navSections
      .map((section) => ({
        ...section,
        items: section.items.filter((item) => canAccessNavItem(item, permissions)),
      }))
      .filter((section) => section.items.length > 0);
    if (!q) return allowedSections;
    return allowedSections
      .map((section) => ({
        ...section,
        items: section.items.filter(
          (item) =>
            item.label.toLowerCase().includes(q) ||
            section.label.toLowerCase().includes(q) ||
            item.path.toLowerCase().includes(q)
        ),
      }))
      .filter((section) => section.items.length > 0);
  }, [filter, permissions]);

  useEffect(() => {
    if (filter.trim()) {
      setOpenSections(Object.fromEntries(filtered.map((s) => [s.id, true])));
    }
  }, [filter, filtered]);

  const today = useMemo(
    () =>
      new Date().toLocaleDateString(undefined, {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
        year: 'numeric',
      }),
    []
  );

  return (
    <Box className="nav-reveal" sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Box sx={{ px: 2, py: 2.2, display: 'flex', alignItems: 'center', gap: 1.25 }}>
        <Box
          component="img"
          src={BISS_LOGO}
          alt="BISS"
          sx={{ height: 40, width: 'auto', objectFit: 'contain' }}
        />
        <Box sx={{ minWidth: 0 }}>
          <Typography sx={{ fontFamily: 'var(--font-serif)', fontWeight: 700, lineHeight: 1.1 }}>
            BISS ERP
          </Typography>
          <Typography variant="caption" color="text.secondary" noWrap>
            ExistERP Management
          </Typography>
          <StackDate today={today} />
        </Box>
      </Box>
      <Divider />
      <List dense sx={{ flex: 1, overflow: 'auto', px: 1, py: 1 }}>
        {filtered.map((section) => {
          const Icon = section.icon;
          const open = openSections[section.id] ?? true;
          return (
            <Box key={section.id} sx={{ mb: 0.5 }}>
              <ListItemButton
                onClick={() =>
                  setOpenSections((prev) => ({ ...prev, [section.id]: !prev[section.id] }))
                }
                sx={{ borderRadius: 2 }}
              >
                <ListItemIcon sx={{ minWidth: 36 }}>
                  <Icon fontSize="small" color="primary" />
                </ListItemIcon>
                <ListItemText
                  primary={section.label}
                  primaryTypographyProps={{ fontWeight: 700, fontSize: 13 }}
                />
                {open ? <ExpandLess fontSize="small" /> : <ExpandMore fontSize="small" />}
              </ListItemButton>
              <Collapse in={open} timeout="auto" unmountOnExit>
                <List dense disablePadding>
                  {section.items.map((item) => {
                    const selected =
                      item.path === '/'
                        ? location.pathname === '/'
                        : location.pathname === item.path ||
                          location.pathname.startsWith(`${item.path}/`);
                    return (
                      <ListItemButton
                        key={item.path}
                        component={RouterLink}
                        to={item.path}
                        selected={selected}
                        onClick={onNavigate}
                        sx={{
                          ml: 1,
                          borderRadius: 2,
                          position: 'relative',
                          '&.Mui-selected': {
                            bgcolor: `${theme.palette.primary.main}14`,
                            color: 'primary.dark',
                          },
                        }}
                      >
                        <ListItemText
                          primary={item.label}
                          primaryTypographyProps={{ fontSize: 13 }}
                        />
                      </ListItemButton>
                    );
                  })}
                </List>
              </Collapse>
            </Box>
          );
        })}
      </List>
    </Box>
  );
}

function StackDate({ today }) {
  return (
    <Typography
      variant="caption"
      color="text.secondary"
      sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mt: 0.35 }}
    >
      <CalendarMonthOutlinedIcon sx={{ fontSize: 13 }} />
      {today}
    </Typography>
  );
}

export default function AppLayout() {
  const theme = useTheme();
  const { colors } = useThemeSettings();
  const isDesktop = useMediaQuery(theme.breakpoints.up('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  const [navFilter, setNavFilter] = useState('');
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const allRoutes = useMemo(() => flattenNav(user?.permissions), [user?.permissions]);

  const pageTitle = useMemo(() => {
    const match = allRoutes.find((r) =>
      r.path === '/' ? location.pathname === '/' : location.pathname.startsWith(r.path)
    );
    return match?.label || 'Dashboard';
  }, [allRoutes, location.pathname]);

  const drawer = (
    <NavContent
      filter={navFilter}
      permissions={user?.permissions}
      onNavigate={() => {
        if (!isDesktop) setMobileOpen(false);
      }}
    />
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar
        position="fixed"
        color="inherit"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          ml: { md: `${DRAWER_WIDTH}px` },
          borderBottom: '1px solid',
          borderColor: 'divider',
          bgcolor: 'rgba(255,255,255,0.9)',
          backdropFilter: 'blur(12px)',
        }}
      >
        <Toolbar sx={{ gap: 1.5 }}>
          {!isDesktop && (
            <IconButton edge="start" onClick={() => setMobileOpen(true)}>
              <MenuIcon />
            </IconButton>
          )}
          <TextField
            size="small"
            placeholder="Looking for something…"
            value={navFilter}
            onChange={(e) => setNavFilter(e.target.value)}
            sx={{
              flex: 1,
              maxWidth: 480,
              '& .MuiOutlinedInput-root': {
                bgcolor: colors.background || '#f4f7fb',
                borderRadius: 2.5,
              },
            }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
          {navFilter && (
            <Box sx={{ display: { xs: 'none', lg: 'flex' }, gap: 0.5, flexWrap: 'wrap' }}>
              {allRoutes
                .filter((r) => r.label.toLowerCase().includes(navFilter.toLowerCase()))
                .slice(0, 3)
                .map((r) => (
                  <Chip
                    key={r.path}
                    size="small"
                    label={r.label}
                    onClick={() => navigate(r.path)}
                    variant="outlined"
                    className="chip-pop"
                  />
                ))}
            </Box>
          )}
          <Box sx={{ flex: 1 }} />
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ display: { xs: 'none', lg: 'block' }, mr: 0.5 }}
          >
            {pageTitle}
          </Typography>
          <Tooltip title="Theme & typography">
            <IconButton color="primary" onClick={() => navigate('/settings/theme')}>
              <SettingsOutlinedIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Notifications">
            <IconButton>
              <Badge color="error" variant="dot" overlap="circular">
                <NotificationsNoneOutlinedIcon />
              </Badge>
            </IconButton>
          </Tooltip>
          <Chip
            avatar={
              <Avatar
                sx={{
                  bgcolor: 'secondary.main',
                  width: 28,
                  height: 28,
                  fontSize: 12,
                }}
              >
                {(user?.displayName || user?.username || 'U').slice(0, 1).toUpperCase()}
              </Avatar>
            }
            label={`${user?.displayName || user?.username || 'User'}${user?.role ? ` · ${user.role}` : ''}`}
            variant="outlined"
            sx={{ maxWidth: { xs: 140, sm: 280 } }}
          />
          <IconButton
            color="primary"
            onClick={() => {
              logout();
              navigate('/login');
            }}
            title="Logout"
          >
            <LogoutIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { md: DRAWER_WIDTH }, flexShrink: { md: 0 } }}>
        {isDesktop ? (
          <Drawer
            variant="permanent"
            open
            sx={{
              '& .MuiDrawer-paper': {
                width: DRAWER_WIDTH,
                boxSizing: 'border-box',
                bgcolor: '#ffffff',
              },
            }}
          >
            {drawer}
          </Drawer>
        ) : (
          <Drawer
            variant="temporary"
            open={mobileOpen}
            onClose={() => setMobileOpen(false)}
            ModalProps={{ keepMounted: true }}
            sx={{
              '& .MuiDrawer-paper': {
                width: DRAWER_WIDTH,
                boxSizing: 'border-box',
                transition: 'transform 260ms ease',
              },
            }}
          >
            {drawer}
          </Drawer>
        )}
      </Box>

      <Box
        component="main"
        className="page-fade"
        key={location.pathname}
        sx={{
          flexGrow: 1,
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          p: { xs: 2, md: 3 },
          mt: 8,
          minHeight: '100vh',
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
