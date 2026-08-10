import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Paper,
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import AddIcon from '@mui/icons-material/Add';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import PageHeader from './PageHeader';
import EmptyState from './EmptyState';
import LoadingBlock from './LoadingBlock';
import StatusChip from './StatusChip';
import { apiDelete, apiGetFirst, apiPost, apiPut, normalizeList } from '../api/client';
import { formatDate, formatDateTime, formatINR } from '../utils/format';
import { useSnackbar } from './SnackbarProvider';

function renderCell(row, col) {
  const raw = typeof col.getValue === 'function' ? col.getValue(row) : row[col.field];
  if (col.type === 'money') return formatINR(raw);
  if (col.type === 'date') return formatDate(raw);
  if (col.type === 'datetime') return formatDateTime(raw);
  if (col.type === 'status') return <StatusChip status={raw} />;
  if (col.render) return col.render(raw, row);
  return raw ?? '—';
}

/**
 * Flexible list + optional create/edit dialog page.
 * Tries multiple API endpoints; shows graceful empty/error states.
 */
export default function ResourcePage({
  title,
  subtitle,
  crumbs,
  endpoints = [],
  createEndpoint,
  updateEndpoint,
  deleteEndpoint,
  columns = [],
  fields = [],
  createLabel = 'Add',
  mapCreateBody,
  mapUpdateBody,
  filters = [],
  transformRows,
  rowKey = 'id',
  allowCreate = true,
  allowEdit = false,
  allowDelete = false,
  extraActions,
  onRowClick,
}) {
  const { success, error } = useSnackbar();
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState('');
  const [filterValues, setFilterValues] = useState({});
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [loadError, setLoadError] = useState('');

  const endpointKey = useMemo(() => (Array.isArray(endpoints) ? endpoints.join('|') : String(endpoints)), [endpoints]);
  const transformRef = useRef(transformRows);
  transformRef.current = transformRows;
  const filterKey = useMemo(() => JSON.stringify(filterValues || {}), [filterValues]);

  const load = useCallback(async () => {
    setLoading(true);
    setLoadError('');
    try {
      const paths = endpointKey.split('|').filter(Boolean);
      const parsedFilters = JSON.parse(filterKey || '{}');
      const data = await apiGetFirst(paths, {
        q: query || undefined,
        ...parsedFilters,
      });
      let list = normalizeList(data);
      if (transformRef.current) list = transformRef.current(list, data);
      setRows(list);
    } catch (err) {
      setRows([]);
      setLoadError(err.message || 'Unable to load data');
    } finally {
      setLoading(false);
    }
  }, [endpointKey, query, filterKey]);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = () => {
    const initial = {};
    fields.forEach((f) => {
      initial[f.name] = f.defaultValue ?? '';
    });
    setEditing(null);
    setForm(initial);
    setDialogOpen(true);
  };

  const openEdit = (row) => {
    const initial = {};
    fields.forEach((f) => {
      initial[f.name] = row[f.name] ?? row[f.source] ?? '';
    });
    setEditing(row);
    setForm(initial);
    setDialogOpen(true);
  };

  const save = async () => {
    setSaving(true);
    try {
      if (editing) {
        const path =
          typeof updateEndpoint === 'function'
            ? updateEndpoint(editing)
            : `${updateEndpoint || endpoints[0]}/${editing[rowKey]}`;
        const body = mapUpdateBody ? mapUpdateBody(form, editing) : form;
        await apiPut(path, body);
        success('Record updated');
      } else {
        const path = createEndpoint || endpoints[0];
        const body = mapCreateBody ? mapCreateBody(form) : form;
        await apiPost(path, body);
        success('Record created');
      }
      setDialogOpen(false);
      await load();
    } catch (err) {
      error(err.message || 'Save failed');
    } finally {
      setSaving(false);
    }
  };

  const remove = async (row) => {
    if (!window.confirm('Delete this record?')) return;
    try {
      const path =
        typeof deleteEndpoint === 'function'
          ? deleteEndpoint(row)
          : `${deleteEndpoint || endpoints[0]}/${row[rowKey]}`;
      await apiDelete(path);
      success('Record deleted');
      await load();
    } catch (err) {
      error(err.message || 'Delete failed');
    }
  };

  const visibleColumns = useMemo(() => columns, [columns]);

  return (
    <Box>
      <PageHeader
        title={title}
        subtitle={subtitle}
        crumbs={crumbs}
        actions={
          <Stack direction="row" spacing={1}>
            {extraActions}
            <Tooltip title="Refresh">
              <IconButton onClick={load}>
                <RefreshIcon />
              </IconButton>
            </Tooltip>
            {allowCreate && (
              <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
                {createLabel}
              </Button>
            )}
          </Stack>
        }
      />

      <Paper sx={{ p: 2, mb: 2 }}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={1.5}>
          <TextField
            size="small"
            label="Search"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            sx={{ minWidth: 220, flex: 1 }}
          />
          {filters.map((f) => (
            <TextField
              key={f.name}
              select={Boolean(f.options)}
              size="small"
              label={f.label}
              value={filterValues[f.name] ?? ''}
              onChange={(e) =>
                setFilterValues((prev) => ({ ...prev, [f.name]: e.target.value }))
              }
              sx={{ minWidth: 160 }}
            >
              {f.options?.map((opt) => (
                <MenuItem key={opt.value ?? opt} value={opt.value ?? opt}>
                  {opt.label ?? opt}
                </MenuItem>
              ))}
            </TextField>
          ))}
          <Button variant="outlined" onClick={load}>
            Apply
          </Button>
        </Stack>
      </Paper>

      {loading ? (
        <LoadingBlock />
      ) : loadError ? (
        <EmptyState
          title="Could not load data"
          description={`${loadError}. The API endpoint may still be under construction.`}
          actionLabel="Retry"
          onAction={load}
        />
      ) : rows.length === 0 ? (
        <EmptyState
          title="No records yet"
          description="Create a record or adjust filters to see results."
          actionLabel={allowCreate ? createLabel : undefined}
          onAction={allowCreate ? openCreate : undefined}
        />
      ) : (
        <TableContainer component={Paper} className="page-fade">
          <Table size="small">
            <TableHead>
              <TableRow>
                {visibleColumns.map((c) => (
                  <TableCell key={c.field || c.header} sx={{ whiteSpace: 'nowrap' }}>
                    {c.header}
                  </TableCell>
                ))}
                {(allowEdit || allowDelete) && <TableCell align="right">Actions</TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row, idx) => (
                <TableRow
                  key={row[rowKey] ?? idx}
                  hover
                  onClick={() => onRowClick?.(row)}
                  sx={{ cursor: onRowClick ? 'pointer' : 'default' }}
                >
                  {visibleColumns.map((c) => (
                    <TableCell key={c.field || c.header}>{renderCell(row, c)}</TableCell>
                  ))}
                  {(allowEdit || allowDelete) && (
                    <TableCell align="right" onClick={(e) => e.stopPropagation()}>
                      {allowEdit && (
                        <IconButton size="small" onClick={() => openEdit(row)}>
                          <EditOutlinedIcon fontSize="small" />
                        </IconButton>
                      )}
                      {allowDelete && (
                        <IconButton size="small" color="error" onClick={() => remove(row)}>
                          <DeleteOutlinedIcon fontSize="small" />
                        </IconButton>
                      )}
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{editing ? `Edit ${title}` : `${createLabel} — ${title}`}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {fields.map((f) => (
              <TextField
                key={f.name}
                label={f.label}
                type={f.type || 'text'}
                select={Boolean(f.options)}
                required={f.required}
                multiline={f.multiline}
                minRows={f.minRows || 1}
                value={form[f.name] ?? ''}
                onChange={(e) => setForm((prev) => ({ ...prev, [f.name]: e.target.value }))}
                fullWidth
              >
                {f.options?.map((opt) => (
                  <MenuItem key={opt.value ?? opt} value={opt.value ?? opt}>
                    {opt.label ?? opt}
                  </MenuItem>
                ))}
              </TextField>
            ))}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={save} disabled={saving}>
            {saving ? 'Saving…' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
