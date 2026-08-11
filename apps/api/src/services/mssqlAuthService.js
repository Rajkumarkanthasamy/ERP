import { mssqlQuery } from '../db/mssql.js';
import {
  encryptLegacyPassword,
  legacyPasswordMatches,
  validateLegacyPasswordPolicy,
} from './legacyCrypto.js';

// Database column -> stable permission key used by the React application.
export const LEGACY_PERMISSION_COLUMNS = {
  ProjectMaster: 'projectMaster',
  PartMaster: 'partMaster',
  Reciept: 'receipt',
  Issue: 'issue',
  AdditionalMaster: 'additionalMaster',
  Reports: 'reports',
  Indent: 'indent',
  PO: 'purchaseOrder',
  WO: 'workOrder',
  ProjectBOM: 'projectBom',
  POWOGenerate: 'poWoGenerate',
  ProductMaster: 'productMaster',
  CustomerMaster: 'customerMaster',
  AddUser: 'addUser',
  VendorMaster: 'vendorMaster',
  MaterialLedger: 'materialLedger',
  BOMAuthorise: 'bomAuthorise',
  Quotation: 'quotation',
  Design: 'design',
  ProductBOM: 'productBom',
  PurchaseManager: 'purchaseManager',
  GeneralManager: 'generalManager',
  ProductionManager: 'productionManager',
  ManufacturingHead: 'manufacturingHead',
  ProjectTransfer: 'projectTransfer',
  ProjectDocuments: 'projectDocuments',
  ProjectSSWarranty: 'projectWarranty',
  ProjectStatusUpdate: 'projectStatusUpdate',
  ProjectIntallStatus: 'projectInstallStatus',
  ProjectTransferrequest: 'projectTransferRequest',
  SalesQuote: 'salesQuote',
  SalesProduct: 'salesProduct',
  CustomerVisit: 'customerVisit',
  OpportunityDetails: 'opportunityDetails',
  PegRate: 'pegRate',
  EnquiryRegister: 'enquiryRegister',
  TestingLabQuote: 'testingLabQuote',
  CreateProject: 'createProject',
  ProjectDates: 'projectDates',
  Validation: 'validation',
  VTimeSheet: 'validationTimesheet',
  POTrack: 'poTrack',
  KanBanMaster: 'kanbanMaster',
  KanBanItems: 'kanbanItems',
  KanBanReports: 'kanbanReports',
  KanBanIssue: 'kanbanIssue',
  SecurityCheck: 'securityCheck',
  QualityManagement: 'qualityManagement',
  Copmalint: 'complaint',
  KanBanElectronicsItems: 'kanbanElectronicsItems',
  KanBanBOM: 'kanbanBom',
  KanBanElectronicsIssue: 'kanbanElectronicsIssue',
  KanBanElectronicsLedger: 'kanbanElectronicsLedger',
  QuoteViewItems: 'quoteViewItems',
  KanBanElectronicsItemReturn: 'kanbanElectronicsReturn',
  KanBanElectronicsStockAdjust: 'kanbanElectronicsAdjust',
  ProjectPermission: 'projectPermission',
  KanBanItemReturn: 'kanbanItemReturn',
  KanBanStockAdjust: 'kanbanStockAdjust',
  TimeSheet: 'timesheet',
  TimeSheetReports: 'timesheetReports',
  GMProjectApprove: 'gmProjectApprove',
  MachineUtilization: 'machineUtilization',
  AssetMaster: 'assetMaster',
  StandardCostUpdate: 'standardCostUpdate',
  CityMaster: 'cityMaster',
  FinanceManager: 'financeManager',
  ServiceQuote: 'serviceQuote',
  TestLabManager: 'testLabManager',
  ProjectBarcode: 'projectBarcode',
  Machine_passport: 'machinePassport',
  OperationManager: 'operationManager',
  ProjectClosure: 'projectClosure',
  WARReport: 'warReport',
};

function mapPermissions(row) {
  return Object.fromEntries(
    Object.entries(LEGACY_PERMISSION_COLUMNS).map(([column, key]) => [key, Boolean(row[column])])
  );
}

function deriveRole(row) {
  if (row.GeneralManager) return 'General Manager';
  if (row.OperationManager) return 'Operations Manager';
  if (row.FinanceManager) return 'Finance Manager';
  if (row.ManufacturingHead) return 'Manufacturing Head';
  if (row.PurchaseManager) return 'Purchase Manager';
  if (row.ProductionManager) return 'Production Manager';
  if (row.TestLabManager) return 'Test Lab Manager';
  if (row.AuthorisedPreson) return 'Authorised Person';
  return 'User';
}

/**
 * Authenticate against the legacy Login table using the exact AES format from
 * Code/Cryptography.cs. A plain-text fallback is retained only for old/test DBs.
 */
export async function authenticateLegacyUser(username, password) {
  const result = await mssqlQuery(
    `
    SELECT TOP 1 *,
      DATEDIFF(DAY, PasswordChangeDate, GETDATE()) AS PasswordExpiryDays
    FROM Login
    WHERE LOWER(UserName) = LOWER(@UserName)
    `,
    { UserName: String(username).trim() }
  );

  const row = result.recordset[0];
  if (!row || !legacyPasswordMatches(password, row.LoginPassword)) {
    return { ok: false, error: 'Invalid username or password' };
  }
  if (!row.Active) {
    return { ok: false, error: 'This user account is inactive' };
  }

  const permissions = mapPermissions(row);
  const canApprovePR = Boolean(
    row.PurchaseManager || row.GeneralManager || row.ManufacturingHead || row.OperationManager
  );
  const canGeneratePO = Boolean(row.POWOGenerate || row.PurchaseManager || row.GeneralManager);
  const canApprovePO = Boolean(
    row.GeneralManager ||
      row.OperationManager ||
      row.ManufacturingHead ||
      row.PurchaseManager ||
      row.FinanceManager
  );
  const passwordExpiryDays =
    row.PasswordExpiryDays == null ? null : Number(row.PasswordExpiryDays);

  // Match the legacy active-login tracking without failing authentication when
  // an older schema does not contain Loggedin.
  try {
    await mssqlQuery(`UPDATE Login SET Loggedin = 1 WHERE LoginId = @LoginId`, {
      LoginId: row.LoginId,
    });
  } catch {
    // Schema compatibility: Loggedin was added after early ERP versions.
  }

  return {
    ok: true,
    user: {
      id: row.LoginId,
      username: row.UserName,
      display_name: row.UserName,
      displayName: row.UserName,
      department: row.Department || '',
      email: row.emailid || '',
      role: deriveRole(row),
      permissions,
      authorisedPerson: row.AuthorisedPreson || '',
      authorisedPerson2: row.AuthorisedPerson2 || '',
      erpVersion: row.ERPVersion,
      passwordExpiryDays,
      mustChangePassword: passwordExpiryDays != null && passwordExpiryDays > 90,
      can_approve_pr: canApprovePR ? 1 : 0,
      can_generate_po: canGeneratePO ? 1 : 0,
      can_approve_po: canApprovePO ? 1 : 0,
      is_pm: row.PurchaseManager ? 1 : 0,
      is_mh: row.ManufacturingHead ? 1 : 0,
      is_gm: row.GeneralManager ? 1 : 0,
      is_om: row.OperationManager ? 1 : 0,
      is_pc: row.PurchaseManager ? 1 : 0,
      canApprovePR,
      canGeneratePO,
      canApprovePO,
      isPm: Boolean(row.PurchaseManager),
      isMh: Boolean(row.ManufacturingHead),
      isGm: Boolean(row.GeneralManager),
      isOm: Boolean(row.OperationManager),
      isPc: Boolean(row.PurchaseManager),
    },
  };
}

export async function changeLegacyPassword(username, currentPassword, newPassword) {
  if (!validateLegacyPasswordPolicy(newPassword)) {
    throw new Error(
      'Password must be exactly 12 characters and include uppercase, lowercase, number, and special character'
    );
  }
  if (currentPassword === newPassword) {
    throw new Error('New password must be different from the current password');
  }

  const result = await mssqlQuery(
    `SELECT TOP 1 LoginPassword FROM Login WHERE LOWER(UserName) = LOWER(@UserName)`,
    { UserName: username }
  );
  const row = result.recordset[0];
  if (!row || !legacyPasswordMatches(currentPassword, row.LoginPassword)) {
    throw new Error('Current password is incorrect');
  }

  await mssqlQuery(
    `
    UPDATE Login
    SET LoginPassword = @Password, PasswordChangeDate = GETDATE()
    WHERE LOWER(UserName) = LOWER(@UserName)
    `,
    {
      UserName: username,
      Password: encryptLegacyPassword(newPassword),
    }
  );
  return { ok: true };
}

export async function markLegacyLogout(username) {
  try {
    await mssqlQuery(`UPDATE Login SET Loggedin = 0 WHERE LOWER(UserName) = LOWER(@UserName)`, {
      UserName: username,
    });
  } catch {
    // Compatibility with versions without Loggedin.
  }
}

export async function listLegacyUsers() {
  const result = await mssqlQuery(`
    SELECT TOP 1000 *
    FROM Login
    ORDER BY UserName
  `);
  return result.recordset.map((row) => ({
    id: row.LoginId,
    loginId: row.LoginId,
    username: row.UserName,
    displayName: row.UserName,
    department: row.Department || '',
    email: row.emailid || '',
    role: deriveRole(row),
    active: Boolean(row.Active),
    authorisedPerson: row.AuthorisedPreson || '',
    authorisedPerson2: row.AuthorisedPerson2 || '',
    erpVersion: row.ERPVersion,
    permissions: mapPermissions(row),
  }));
}
