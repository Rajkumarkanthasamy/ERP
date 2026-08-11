import { mssqlQuery } from '../db/mssql.js';
import fs from 'fs';
import path from 'path';

export async function listComments(entityType, entityRef) {
  const result = await mssqlQuery(
    `
    SELECT CommentID AS id, EntityType AS entityType, EntityRef AS entityRef,
           CommentText AS commentText, CreatedBy AS createdBy, CreatedDate AS createdAt
    FROM ProcurementComment
    WHERE EntityType = @EntityType AND EntityRef = @EntityRef
    ORDER BY CommentID
    `,
    { EntityType: entityType, EntityRef: entityRef }
  );
  return result.recordset;
}

export async function addComment({ entityType, entityRef, commentText, user }) {
  if (!commentText?.trim()) throw new Error('Comment text is required');
  await mssqlQuery(
    `
    INSERT INTO ProcurementComment (EntityType, EntityRef, CommentText, CreatedBy)
    VALUES (@EntityType, @EntityRef, @CommentText, @CreatedBy)
    `,
    {
      EntityType: entityType,
      EntityRef: entityRef,
      CommentText: commentText.trim(),
      CreatedBy: user.displayName || user.username,
    }
  );
  const rows = await listComments(entityType, entityRef);
  return rows[rows.length - 1];
}

export async function listAttachments(entityType, entityRef) {
  const result = await mssqlQuery(
    `
    SELECT AttachmentID AS id, EntityType AS entityType, EntityRef AS entityRef,
           FileName AS fileName, FilePath AS filePath, FileSizeKB AS fileSizeKb,
           UploadedBy AS uploadedBy, UploadedDate AS uploadedAt, Remarks AS remarks
    FROM ProcurementAttachment
    WHERE EntityType = @EntityType AND EntityRef = @EntityRef
    ORDER BY AttachmentID DESC
    `,
    { EntityType: entityType, EntityRef: entityRef }
  );
  return result.recordset;
}

export async function addAttachment({ entityType, entityRef, file, remarks, user }) {
  await mssqlQuery(
    `
    INSERT INTO ProcurementAttachment
      (EntityType, EntityRef, FileName, FilePath, FileSizeKB, UploadedBy, Remarks)
    VALUES
      (@EntityType, @EntityRef, @FileName, @FilePath, @FileSizeKB, @UploadedBy, @Remarks)
    `,
    {
      EntityType: entityType,
      EntityRef: entityRef,
      FileName: file.originalname,
      FilePath: file.filename,
      FileSizeKB: +(file.size / 1024).toFixed(2),
      UploadedBy: user.displayName || user.username,
      Remarks: remarks || null,
    }
  );
  const rows = await listAttachments(entityType, entityRef);
  return rows[0];
}

export async function getAttachmentFile(id) {
  const result = await mssqlQuery(
    `SELECT TOP 1 * FROM ProcurementAttachment WHERE AttachmentID = @ID`,
    { ID: id }
  );
  const row = result.recordset[0];
  if (!row) return null;
  const uploadDir = process.env.UPLOAD_DIR || path.join(process.cwd(), 'uploads');
  const fullPath = path.join(uploadDir, row.FilePath);
  if (!fs.existsSync(fullPath)) return null;
  return {
    row: {
      file_name: row.FileName,
      file_path: row.FilePath,
    },
    fullPath,
  };
}
