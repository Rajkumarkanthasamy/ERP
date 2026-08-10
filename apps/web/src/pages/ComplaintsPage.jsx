import ResourcePage from '../components/ResourcePage';

export default function ComplaintsPage() {
  return (
    <ResourcePage
      title="Complaints / Support"
      subtitle="Customer complaints and support tickets."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Complaints' }]}
      endpoints={['/complaints', '/support/tickets']}
      createEndpoint="/complaints"
      createLabel="New complaint"
      allowEdit
      columns={[
        { field: 'complaintNo', header: 'Ticket', getValue: (r) => r.complaintNo || r.ticketNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'subject', header: 'Subject' },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Created', type: 'datetime', getValue: (r) => r.createdAt || r.complaintDate },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'subject', label: 'Subject', required: true },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High'], defaultValue: 'Medium' },
        { name: 'status', label: 'Status', options: ['Open', 'In Progress', 'Resolved', 'Closed'], defaultValue: 'Open' },
        { name: 'description', label: 'Description', multiline: true, minRows: 3 },
      ]}
    />
  );
}
