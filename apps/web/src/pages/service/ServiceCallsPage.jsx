import ResourcePage from '../../components/ResourcePage';

export default function ServiceCallsPage() {
  return (
    <ResourcePage
      title="Service Calls"
      subtitle="Log and manage customer service / AMC calls."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Service' }, { label: 'Service Calls' }]}
      endpoints={['/service-calls', '/service/calls']}
      createEndpoint="/service-calls"
      createLabel="New call"
      allowEdit
      columns={[
        { field: 'callNo', header: 'Call No', getValue: (r) => r.callNo || r.docNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'subject', header: 'Subject', getValue: (r) => r.subject || r.issue },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'assignedTo', header: 'Assigned' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'callDate', header: 'Date', type: 'datetime', getValue: (r) => r.callDate || r.createdAt },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'subject', label: 'Subject', required: true },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High', 'Critical'], defaultValue: 'Medium' },
        { name: 'assignedTo', label: 'Assigned to' },
        { name: 'status', label: 'Status', options: ['Open', 'In Progress', 'Resolved', 'Closed'], defaultValue: 'Open' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
