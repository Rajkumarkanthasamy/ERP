import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  Divider,
  List,
  ListItem,
  ListItemText,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { apiGet, apiPost } from '../api/client';
import { formatDateTime } from '../utils/format';
import { useSnackbar } from './SnackbarProvider';

/**
 * Comments panel for PR / PO collaboration (SQLite collab + Phase3 MSSQL tables).
 */
export default function CollabPanel({ entityType, entityRef, title = 'Comments' }) {
  const { success, error } = useSnackbar();
  const [comments, setComments] = useState([]);
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    if (!entityType || !entityRef) return;
    setLoading(true);
    try {
      const data = await apiGet(
        `/collab/${encodeURIComponent(entityType)}/${encodeURIComponent(entityRef)}/comments`
      );
      setComments(Array.isArray(data) ? data : []);
    } catch (err) {
      setComments([]);
      if (err.status !== 501) error(err.message);
    } finally {
      setLoading(false);
    }
  }, [entityType, entityRef, error]);

  useEffect(() => {
    load();
  }, [load]);

  const submit = async () => {
    if (!text.trim()) return;
    setBusy(true);
    try {
      await apiPost(
        `/collab/${encodeURIComponent(entityType)}/${encodeURIComponent(entityRef)}/comments`,
        { commentText: text.trim() }
      );
      setText('');
      success('Comment added');
      await load();
    } catch (err) {
      error(err.message);
    } finally {
      setBusy(false);
    }
  };

  if (!entityRef) {
    return (
      <Typography variant="body2" color="text.secondary">
        Select a document to view collaboration comments.
      </Typography>
    );
  }

  return (
    <Box>
      <Typography variant="subtitle1" sx={{ mb: 1 }}>
        {title} — {entityRef}
      </Typography>
      {loading ? (
        <Typography variant="body2" color="text.secondary">
          Loading…
        </Typography>
      ) : (
        <List dense disablePadding>
          {comments.length === 0 && (
            <ListItem>
              <ListItemText secondary="No comments yet." />
            </ListItem>
          )}
          {comments.map((c) => (
            <ListItem key={c.id} alignItems="flex-start" sx={{ px: 0 }}>
              <ListItemText
                primary={c.commentText}
                secondary={`${c.createdBy || '—'} · ${formatDateTime(c.createdAt)}`}
              />
            </ListItem>
          ))}
        </List>
      )}
      <Divider sx={{ my: 1.5 }} />
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1}>
        <TextField
          size="small"
          fullWidth
          multiline
          minRows={2}
          placeholder="Add a comment"
          value={text}
          onChange={(e) => setText(e.target.value)}
        />
        <Button variant="contained" onClick={submit} disabled={busy || !text.trim()}>
          Post
        </Button>
      </Stack>
    </Box>
  );
}
