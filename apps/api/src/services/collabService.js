import fs from 'fs';
import path from 'path';
import { getDb } from '../db/connection.js';

export function listComments(entityType, entityRef) {
  const db = getDb();
  return db
    .prepare(
      `SELECT id, entity_type AS entityType, entity_ref AS entityRef, comment_text AS commentText,
              created_by AS createdBy, created_at AS createdAt
       FROM procurement_comments
       WHERE entity_type = ? AND entity_ref = ?
       ORDER BY id ASC`
    )
    .all(entityType, entityRef);
}

export function addComment({ entityType, entityRef, commentText, user }) {
  if (!commentText?.trim()) throw new Error('Comment text is required');
  const db = getDb();
  const info = db
    .prepare(
      `INSERT INTO procurement_comments (entity_type, entity_ref, comment_text, created_by)
       VALUES (?, ?, ?, ?)`
    )
    .run(entityType, entityRef, commentText.trim(), user.displayName || user.username);
  return listComments(entityType, entityRef).find((c) => c.id === Number(info.lastInsertRowid));
}

export function listAttachments(entityType, entityRef) {
  const db = getDb();
  return db
    .prepare(
      `SELECT id, entity_type AS entityType, entity_ref AS entityRef, file_name AS fileName,
              file_path AS filePath, file_size_kb AS fileSizeKb, uploaded_by AS uploadedBy,
              uploaded_at AS uploadedAt, remarks
       FROM procurement_attachments
       WHERE entity_type = ? AND entity_ref = ?
       ORDER BY id DESC`
    )
    .all(entityType, entityRef);
}

export function addAttachment({ entityType, entityRef, file, remarks, user }) {
  const db = getDb();
  const info = db
    .prepare(
      `INSERT INTO procurement_attachments
        (entity_type, entity_ref, file_name, file_path, file_size_kb, uploaded_by, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    )
    .run(
      entityType,
      entityRef,
      file.originalname,
      file.filename,
      +(file.size / 1024).toFixed(2),
      user.displayName || user.username,
      remarks || null
    );
  return listAttachments(entityType, entityRef).find((a) => a.id === Number(info.lastInsertRowid));
}

export function getAttachmentFile(id) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM procurement_attachments WHERE id = ?').get(id);
  if (!row) return null;
  const uploadDir = process.env.UPLOAD_DIR || path.join(process.cwd(), 'uploads');
  const fullPath = path.join(uploadDir, row.file_path);
  if (!fs.existsSync(fullPath)) return null;
  return { row, fullPath };
}
