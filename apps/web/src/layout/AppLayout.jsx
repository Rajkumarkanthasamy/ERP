import { useEffect, useMemo, useState } from 'react';
import { Link as RouterLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Avatar,
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
  Typography,
  useMediaQuery,
  Collapse,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import LogoutIcon from '@mui/icons-material/Logout';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import { useTheme } from '@mui/material/styles';
import { useAuth } from '../auth/AuthContext';
import { BISS_LOGO, flattenNav, navSections } from './navConfig';

const DRAWER_WIDTH = 280;

function NavContent({ onNavigate, filter }) {
  const location = useLocation();
  const [openSections, setOpenSections] = useState(() =>
    Object.fromEntries(navSections.map((s) => [s.id, true]))
  );

  const filtered = useMemo(() => {
    const q = filter.trim().toLowerCase();
    if (!q) return navSections;
    return navSections
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
  }, [filter]);

  useEffect(() => {
    if (filter.trim()) {
      setOpenSections(Object.fromEntries(filtered.map((s) => [s.id, true])));
    }
  }, [filter, filtered]);

  return (
    <Box className="nav-reveal" sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Box sx={{ px: 2, py: 2.2, display: 'flex', alignItems: 'center', gap: 1.25 }}>
        <Box
          component="img"
          src={BISS_LOGO}
          alt="BISS"
          sx={{ height: 40, width: 'auto', objectFit: 'contain' }}
        />
        <Box>
          <Typography sx={{ fontFamily: 'var(--font-serif)', fontWeight: 700, lineHeight: 1.1 }}>
            BISS ERP
          </Typography>
          <Typography variant="caption" color="text.secondary">
            ExistERP Suite
          </Typography>
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
                          '&.Mui-selected': {
                            bgcolor: 'rgba(15, 118, 110, 0.12)',
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

export default function AppLayout() {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  const [navFilter, setNavFilter] = useState('');
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const allRoutes = useMemo(() => flattenNav(), []);

  const drawer = (
    <NavContent
      filter={navFilter}
      onNavigate={() => {
        if (!isDesktop) setMobileOpen(false);
      }}
    />
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar
        position="fixed"
        color="inherit"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          ml: { md: `${DRAWER_WIDTH}px` },
          borderBottom: '1px solid',
          borderColor: 'divider',
          bgcolor: 'rgba(255,255,255,0.86)',
          backdropFilter: 'blur(10px)',
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
            placeholder="Search menu…"
            value={navFilter}
            onChange={(e) => setNavFilter(e.target.value)}
            sx={{
              flex: 1,
              maxWidth: 420,
              '& .MuiOutlinedInput-root': { bgcolor: '#fff' },
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
                  />
                ))}
            </Box>
          )}
          <Box sx={{ flex: 1 }} />
          <Chip
            avatar={
              <Avatar sx={{ bgcolor: 'secondary.main', width: 28, height: 28, fontSize: 12 }}>
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
              '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' },
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
        sx={{
          flexGrow: 1,
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          p: { xs: 2, md: 3 },
          mt: 8,
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
