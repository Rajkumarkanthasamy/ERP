import { Router } from 'express';
import bcrypt from 'bcryptjs';
import { getDb } from '../db/connection.js';
import { authRequired, signToken } from '../middleware/auth.js';

const router = Router();

router.post('/login', (req, res) => {
  try {
    const { username, password } = req.body || {};
    if (!username || !password) {
      return res.status(400).json({ error: 'Username and password are required' });
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
      },
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/me', authRequired, (req, res) => {
  res.json({ user: req.user });
});

export default router;
