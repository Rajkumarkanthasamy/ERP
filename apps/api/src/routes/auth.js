import { Router } from 'express';
import bcrypt from 'bcryptjs';
import { getDb } from '../db/connection.js';
import { isMssqlMode } from '../db/mssql.js';
import {
  authenticateLegacyUser,
  changeLegacyPassword,
  markLegacyLogout,
} from '../services/mssqlAuthService.js';
import { validateLegacyPasswordPolicy } from '../services/legacyCrypto.js';
import { authRequired, signToken } from '../middleware/auth.js';

const router = Router();

router.post('/login', async (req, res) => {
  try {
    const { username, password } = req.body || {};
    if (!username || !password) {
      return res.status(400).json({ error: 'Username and password are required' });
    }

    if (isMssqlMode()) {
      const result = await authenticateLegacyUser(username, password);
      if (!result.ok) {
        return res.status(401).json({ error: result.error });
      }
      const token = signToken(result.user);
      return res.json({
        token,
        user: {
          id: result.user.id,
          username: result.user.username,
          displayName: result.user.displayName,
          role: result.user.role,
          department: result.user.department,
          email: result.user.email,
          permissions: result.user.permissions,
          erpVersion: result.user.erpVersion,
          passwordExpiryDays: result.user.passwordExpiryDays,
          mustChangePassword: result.user.mustChangePassword,
          canApprovePR: result.user.canApprovePR,
          canGeneratePO: result.user.canGeneratePO,
          canApprovePO: result.user.canApprovePO,
          isPm: result.user.isPm,
          isMh: result.user.isMh,
          isGm: result.user.isGm,
          isOm: result.user.isOm,
          isPc: result.user.isPc,
          source: 'mssql',
        },
      });
    }

    const db = getDb();
    const user = db.prepare('SELECT * FROM users WHERE username = ? AND active = 1').get(username);
    if (!user || !bcrypt.compareSync(password, user.password_hash)) {
      return res.status(401).json({ error: 'Invalid username or password' });
    }
    const token = signToken(user);
    res.json({
      token,
      user: {
        id: user.id,
        username: user.username,
        displayName: user.display_name,
        role: user.role,
        department: user.department,
        canApprovePR: !!user.can_approve_pr,
        canGeneratePO: !!user.can_generate_po,
        canApprovePO: !!user.can_approve_po,
        isPm: !!user.is_pm,
        isMh: !!user.is_mh,
        isGm: !!user.is_gm,
        isOm: !!user.is_om,
        isPc: !!user.is_pc,
        source: 'sqlite',
      },
    });
  } catch (err) {
    res.status(500).json({
      error: err.message,
      hint: isMssqlMode()
        ? 'SQL Server login failed. Verify MSSQL_* env vars and that this PC can reach the SQL Express instance.'
        : undefined,
    });
  }
});

router.get('/me', authRequired, (req, res) => {
  res.json({ user: req.user });
});

router.post('/change-password', authRequired, async (req, res) => {
  try {
    const { currentPassword, newPassword } = req.body || {};
    if (!currentPassword || !newPassword) {
      return res.status(400).json({ error: 'Current and new password are required' });
    }

    if (isMssqlMode()) {
      return res.json(
        await changeLegacyPassword(req.user.username, currentPassword, newPassword)
      );
    }

    if (!validateLegacyPasswordPolicy(newPassword)) {
      return res.status(400).json({
        error:
          'Password must be exactly 12 characters and include uppercase, lowercase, number, and special character',
      });
    }
    const db = getDb();
    const user = db.prepare('SELECT * FROM users WHERE username = ?').get(req.user.username);
    if (!user || !bcrypt.compareSync(currentPassword, user.password_hash)) {
      return res.status(400).json({ error: 'Current password is incorrect' });
    }
    if (currentPassword === newPassword) {
      return res.status(400).json({ error: 'New password must be different from the current password' });
    }
    db.prepare('UPDATE users SET password_hash = ? WHERE id = ?').run(
      bcrypt.hashSync(newPassword, 12),
      user.id
    );
    return res.json({ ok: true });
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.post('/logout', authRequired, async (req, res) => {
  if (isMssqlMode()) await markLegacyLogout(req.user.username);
  res.json({ ok: true });
});

export default router;
