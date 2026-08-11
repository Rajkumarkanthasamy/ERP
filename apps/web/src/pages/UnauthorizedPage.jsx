import { useNavigate } from 'react-router-dom';
import EmptyState from '../components/EmptyState';

export default function UnauthorizedPage() {
  const navigate = useNavigate();
  return (
    <EmptyState
      title="Access restricted"
      description="Your ExistERP user account does not have permission for this module."
      actionLabel="Return to dashboard"
      onAction={() => navigate('/')}
    />
  );
}
