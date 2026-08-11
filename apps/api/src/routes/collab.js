import { Router } from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs';
import { authRequired } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as collabService from '../services/collabService.js';
import * as mssqlCollab from '../services/mssqlCollabService.js';

const uploadDir = process.env.UPLOAD_DIR || path.join(process.cwd(), 'uploads');
fs.mkdirSync(uploadDir, { recursive: true });

const storage = multer.diskStorage({
  destination: (_req, _file, cb) => cb(null, uploadDir),
  filename: (_req, file, cb) => {
    const safe = file.originalname.replace(/[^a-zA-Z0-9._-]/g, '_');
    cb(null, `${Date.now()}-${safe}`);
  },
});
const upload = multer({ storage, limits: { fileSize: 15 * 1024 * 1024 } });

const router = Router();

router.get('/:entityType/:entityRef/comments', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(
        await mssqlCollab.listComments(req.params.entityType, req.params.entityRef)
      );
    }
    res.json(collabService.listComments(req.params.entityType, req.params.entityRef));
  } catch (err) {
    if (/Invalid object name/i.test(err.message)) {
      return res.status(501).json({
        error: 'Run apps/api/sql/Phase3_Schema_Alignment.sql for ProcurementComment',
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
      });
    }
    res.status(500).json({ error: err.message });
  }
});

router.post('/:entityType/:entityRef/comments', authRequired, async (req, res) => {
  try {
    const payload = {
      entityType: req.params.entityType,
      entityRef: req.params.entityRef,
      commentText: req.body?.commentText,
      user: req.user,
    };
    if (isMssqlMode()) return res.status(201).json(await mssqlCollab.addComment(payload));
    res.status(201).json(collabService.addComment(payload));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:entityType/:entityRef/attachments', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(
        await mssqlCollab.listAttachments(req.params.entityType, req.params.entityRef)
      );
    }
    res.json(collabService.listAttachments(req.params.entityType, req.params.entityRef));
  } catch (err) {
    if (/Invalid object name/i.test(err.message)) {
      return res.status(501).json({
        error: 'Run apps/api/sql/Phase3_Schema_Alignment.sql for ProcurementAttachment',
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
      });
    }
    res.status(500).json({ error: err.message });
  }
});

router.post(
  '/:entityType/:entityRef/attachments',
  authRequired,
  upload.single('file'),
  async (req, res) => {
    try {
      if (!req.file) return res.status(400).json({ error: 'File is required' });
      const payload = {
        entityType: req.params.entityType,
        entityRef: req.params.entityRef,
        file: req.file,
        remarks: req.body?.remarks,
        user: req.user,
      };
      if (isMssqlMode()) return res.status(201).json(await mssqlCollab.addAttachment(payload));
      res.status(201).json(collabService.addAttachment(payload));
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  }
);

router.get('/attachments/:id/download', authRequired, async (req, res) => {
  try {
    const file = isMssqlMode()
      ? await mssqlCollab.getAttachmentFile(Number(req.params.id))
      : collabService.getAttachmentFile(Number(req.params.id));
    if (!file) return res.status(404).json({ error: 'Attachment not found' });
    res.download(file.fullPath, file.row.file_name || file.row.FileName);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

export default router;
