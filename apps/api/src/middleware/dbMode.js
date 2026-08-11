import { isMssqlMode } from '../db/mssql.js';

export function requireSqliteMode(req, res, next) {
  if (!isMssqlMode()) return next();
  return res.status(501).json({
    error: 'This workflow has not been mapped to the legacy SQL Server schema yet',
    code: 'MSSQL_WORKFLOW_NOT_MAPPED',
    route: `${req.baseUrl}${req.path}`,
    hint: 'Use SQLite demo mode for this scaffold until its ERP_Database mapping is implemented.',
  });
}
