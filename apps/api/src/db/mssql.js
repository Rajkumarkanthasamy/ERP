import sql from 'mssql';

let pool;
let lastError = null;

function buildConfig() {
  if (process.env.MSSQL_CONNECTION_STRING) {
    return process.env.MSSQL_CONNECTION_STRING;
  }

  const serverRaw = process.env.MSSQL_SERVER || 'localhost\\SQLEXPRESS';
  // Node mssql: named instance as server\instance
  const server = serverRaw.replace(/\\\\/g, '\\');

  return {
    server,
    database: process.env.MSSQL_DATABASE || 'ERP_Database',
    user: process.env.MSSQL_USER || 'sa',
    password: process.env.MSSQL_PASSWORD || '',
    options: {
      encrypt: String(process.env.MSSQL_ENCRYPT || 'false').toLowerCase() === 'true',
      trustServerCertificate:
        String(process.env.MSSQL_TRUST_SERVER_CERTIFICATE || 'true').toLowerCase() !== 'false',
      enableArithAbort: true,
      // SQL Express named instances often need this
      instanceName: server.includes('\\') ? server.split('\\')[1] : undefined,
    },
    pool: {
      max: 10,
      min: 0,
      idleTimeoutMillis: 30000,
    },
    connectionTimeout: 15000,
    requestTimeout: 30000,
  };
}

function normalizeConfig(config) {
  if (typeof config === 'string') return config;
  // When using instanceName, server host should be machine name only
  if (config.options?.instanceName && config.server?.includes('\\')) {
    return {
      ...config,
      server: config.server.split('\\')[0],
    };
  }
  return config;
}

export function isMssqlMode() {
  return String(process.env.DB_CLIENT || 'sqlite').toLowerCase() === 'mssql';
}

export async function getMssqlPool() {
  if (pool?.connected) return pool;
  const config = normalizeConfig(buildConfig());
  try {
    pool = await new sql.ConnectionPool(config).connect();
    lastError = null;
    pool.on('error', (err) => {
      console.error('MSSQL pool error:', err.message);
      lastError = err.message;
    });
    console.log('Connected to SQL Server ERP_Database');
    return pool;
  } catch (err) {
    lastError = err.message;
    pool = null;
    throw err;
  }
}

export async function mssqlQuery(text, params = {}) {
  const p = await getMssqlPool();
  const request = p.request();
  for (const [key, value] of Object.entries(params)) {
    request.input(key, value);
  }
  return request.query(text);
}

export async function mssqlHealth() {
  try {
    const p = await getMssqlPool();
    const result = await p.request().query('SELECT DB_NAME() AS dbName, @@SERVERNAME AS serverName, GETDATE() AS serverTime');
    const row = result.recordset[0] || {};
    return {
      ok: true,
      client: 'mssql',
      database: row.dbName,
      server: row.serverName,
      serverTime: row.serverTime,
      lastError: null,
    };
  } catch (err) {
    return {
      ok: false,
      client: 'mssql',
      error: err.message,
      lastError: lastError || err.message,
      hint:
        'Check MSSQL_SERVER / user / password, SQL Server Browser service, TCP enabled for SQLEXPRESS, and firewall. From this machine you must reach GTKA064W111\\SQLEXPRESS01.',
    };
  }
}

export async function closeMssql() {
  if (pool) {
    await pool.close();
    pool = null;
  }
}

export { sql };
