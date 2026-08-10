import jwt from 'jsonwebtoken';

const JWT_SECRET = process.env.JWT_SECRET || 'biss-erp-dev-secret';

export function signToken(user) {
  return jwt.sign(
    {
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
    JWT_SECRET,
    { expiresIn: '12h' }
  );
}

export function authRequired(req, res, next) {
  const header = req.headers.authorization || '';
  const token = header.startsWith('Bearer ') ? header.slice(7) : null;
  if (!token) {
    return res.status(401).json({ error: 'Authentication required' });
  }
  try {
    req.user = jwt.verify(token, JWT_SECRET);
    next();
  } catch {
    return res.status(401).json({ error: 'Invalid or expired token' });
  }
}

export function requirePermission(flag) {
  return (req, res, next) => {
    if (!req.user?.[flag]) {
      return res.status(403).json({ error: `Missing permission: ${flag}` });
    }
    next();
  };
}
