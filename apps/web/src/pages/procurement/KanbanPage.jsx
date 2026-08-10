import { useEffect, useState } from 'react';
import {
  Box,
  Button,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import PageHeader from '../../components/PageHeader';
import LoadingBlock from '../../components/LoadingBlock';
import EmptyState from '../../components/EmptyState';
import StatusChip from '../../components/StatusChip';
import { apiGet, apiPost } from '../../api/client';
import { formatINR } from '../../utils/format';
import { useSnackbar } from '../../components/SnackbarProvider';

export default function KanbanPage() {
  const { success, error } = useSnackbar();
  const [columns, setColumns] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [moving, setMoving] = useState(null);

  const load = async () => {
    setLoading(true);
    try {
      const [kanban, statusList] = await Promise.all([
        apiGet('/prs/kanban'),
        apiGet('/prs/statuses').catch(() => []),
      ]);
      setColumns(Array.isArray(kanban) ? kanban : kanban?.columns || []);
      setStatuses(Array.isArray(statusList) ? statusList : []);
    } catch (err) {
      error(err.message);
      setColumns([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const moveCard = async (prNumber, targetStatus) => {
    setMoving(prNumber);
    try {
      await apiPost('/prs/kanban/move', { prNumber, targetStatus });
      success(`Moved ${prNumber} → ${targetStatus}`);
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setMoving(null);
    }
  };

  if (loading) return <LoadingBlock />;

  return (
    <Box>
      <PageHeader
        title="Procurement Kanban"
        subtitle="Drag-free status board for purchase requests."
        crumbs={[
          { label: 'Home', to: '/' },
          { label: 'Procurement', to: '/procurement' },
          { label: 'Kanban' },
        ]}
        actions={[{ label: 'Refresh', onClick: load, variant: 'outlined' }]}
      />
      {columns.length === 0 ? (
        <EmptyState title="No kanban columns" description="Create PRs to populate the board." />
      ) : (
        <Box className="kanban-board page-fade">
          {columns.map((col) => (
            <Box key={col.status || col.title} className="kanban-column">
              <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 1 }}>
                <Typography fontWeight={700}>{col.status || col.title}</Typography>
                <StatusChip status={String((col.cards || col.items || []).length)} label={`${(col.cards || col.items || []).length}`} />
              </Stack>
              {(col.cards || col.items || []).map((card) => (
                <Box key={card.prNumber || card.id} className="kanban-card">
                  <Typography fontWeight={700}>{card.prNumber}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {card.projectCode} · {card.vendorName || card.vendorCode}
                  </Typography>
                  <Typography variant="body2" sx={{ mt: 0.5 }}>
                    {formatINR(card.totalAmount)}
                  </Typography>
                  <TextField
                    select
                    size="small"
                    fullWidth
                    label="Move to"
                    disabled={moving === card.prNumber}
                    value=""
                    onChange={(e) => moveCard(card.prNumber, e.target.value)}
                    sx={{ mt: 1 }}
                  >
                    {(statuses.length ? statuses : columns.map((c) => c.status)).map((s) => (
                      <MenuItem key={s} value={s} disabled={s === (col.status || card.status)}>
                        {s}
                      </MenuItem>
                    ))}
                  </TextField>
                </Box>
              ))}
              {(col.cards || col.items || []).length === 0 && (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
                  Empty
                </Typography>
              )}
            </Box>
          ))}
        </Box>
      )}
      <Button sx={{ mt: 2 }} onClick={load}>
        Refresh board
      </Button>
    </Box>
  );
}
