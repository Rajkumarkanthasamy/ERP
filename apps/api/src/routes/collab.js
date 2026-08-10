import { Router } from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs';
import { authRequired } from '../middleware/auth.js';
import * as collabService from '../services/collabService.js';

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

router.get('/:entityType/:entityRef/comments', authRequired, (req, res) => {
  res.json(collabService.listComments(req.params.entityType, req.params.entityRef));
});

router.post('/:entityType/:entityRef/comments', authRequired, (req, res) => {
  try {
    res.status(201).json(
      collabService.addComment({
        entityType: req.params.entityType,
        entityRef: req.params.entityRef,
        commentText: req.body?.commentText,
        user: req.user,
      })
    );
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:entityType/:entityRef/attachments', authRequired, (req, res) => {
  res.json(collabService.listAttachments(req.params.entityType, req.params.entityRef));
});

router.post(
  '/:entityType/:entityRef/attachments',
  authRequired,
  upload.single('file'),
  (req, res) => {
    try {
      if (!req.file) return res.status(400).json({ error: 'File is required' });
      res.status(201).json(
        collabService.addAttachment({
          entityType: req.params.entityType,
          entityRef: req.params.entityRef,
          file: req.file,
          remarks: req.body?.remarks,
          user: req.user,
        })
      );
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  }
);

router.get('/attachments/:id/download', authRequired, (req, res) => {
  const file = collabService.getAttachmentFile(Number(req.params.id));
  if (!file) return res.status(404).json({ error: 'Attachment not found' });
  res.download(file.fullPath, file.row.file_name);
});

export default router;
