import { mssqlQuery } from '../db/mssql.js';

/**
 * Authenticate against the legacy ERP Login table (same as C# AuthDAL).
 * Passwords in the old app are stored plain in LoginPassword.
 */
export async function authenticateLegacyUser(username, password) {
  const result = await mssqlQuery(
    `
    SELECT TOP 1
      LoginId,
      UserName,
      ISNULL(Department, '') AS Department,
      ISNULL(AuthorisedPreson, '') AS AuthorisedPreson,
      ISNULL(Active, 0) AS Active,
      ISNULL(PurchaseManager, 0) AS PurchaseManager,
      ISNULL(POWOGenerate, 0) AS POWOGenerate,
      ISNULL(GeneralManager, 0) AS GeneralManager,
      ISNULL(ManufacturingHead, 0) AS ManufacturingHead,
      ISNULL(ProductionManager, 0) AS ProductionManager
    FROM Login
    WHERE UserName = @UserName
      AND LoginPassword = @Password
    `,
    { UserName: username, Password: password }
  );

  const row = result.recordset[0];
  if (!row) {
    return { ok: false, error: 'Invalid username or password' };
  }
  if (!row.Active) {
    return { ok: false, error: 'This user account is inactive' };
  }

  let role = 'User';
  if (row.GeneralManager) role = 'General Manager';
  else if (row.ManufacturingHead) role = 'Manufacturing Head';
  else if (row.PurchaseManager) role = 'Purchase Manager';
  else if (row.ProductionManager) role = 'Production Manager';
  else if (row.AuthorisedPreson) role = 'Authorised Person';

  const canApprovePR = !!(row.PurchaseManager || row.GeneralManager || row.ManufacturingHead);
  const canGeneratePO = !!(row.POWOGenerate || row.PurchaseManager || row.GeneralManager);
  const canApprovePO = !!(row.GeneralManager || row.ManufacturingHead || row.PurchaseManager);

  return {
    ok: true,
    user: {
      id: row.LoginId,
      username: row.UserName,
      display_name: row.UserName,
      displayName: row.UserName,
      department: row.Department || '',
      role,
      can_approve_pr: canApprovePR ? 1 : 0,
      can_generate_po: canGeneratePO ? 1 : 0,
      can_approve_po: canApprovePO ? 1 : 0,
      is_pm: row.PurchaseManager ? 1 : 0,
      is_mh: row.ManufacturingHead ? 1 : 0,
      is_gm: row.GeneralManager ? 1 : 0,
      is_om: 0,
      is_pc: row.PurchaseManager ? 1 : 0,
      canApprovePR,
      canGeneratePO,
      canApprovePO,
      isPm: !!row.PurchaseManager,
      isMh: !!row.ManufacturingHead,
      isGm: !!row.GeneralManager,
      isOm: false,
      isPc: !!row.PurchaseManager,
    },
  };
}
