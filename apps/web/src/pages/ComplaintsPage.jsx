import ResourcePage from '../components/ResourcePage';
import WorkflowActionButtons from '../components/WorkflowActionButtons';

export default function ComplaintsPage() {
  return (
    <ResourcePage
      title="Complaints / Support"
      subtitle="Customer complaints and support tickets."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Complaints' }]}
      endpoints={['/complaints']}
      createEndpoint="/complaints"
      createLabel="New complaint"
      allowEdit
      rowKey="complaintNumber"
      columns={[
        { field: 'complaintNumber', header: 'Ticket' },
        { field: 'customerName', header: 'Customer' },
        { field: 'subject', header: 'Subject' },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Created', type: 'datetime', getValue: (r) => r.createdAt || r.complaintDate },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'subject', label: 'Subject', required: true },
        { name: 'category', label: 'Category', options: ['Support', 'Product', 'Delivery', 'Quality'], defaultValue: 'Support' },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High'], defaultValue: 'Medium' },
        { name: 'description', label: 'Description', multiline: true, minRows: 3 },
        { name: 'assignedTo', label: 'Assigned to' },
        { name: 'resolution', label: 'Resolution', multiline: true, editOnly: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) => {
        const actions =
          row.status === 'Open'
            ? [{ label: 'Start', value: 'start', variant: 'outlined' }]
            : row.status === 'Assigned'
              ? [{ label: 'Start', value: 'start', variant: 'outlined' }]
              : row.status === 'In Progress'
                ? [
                    {
                      label: 'Resolve',
                      value: 'resolve',
                      variant: 'outlined',
                      prompt: {
                        label: 'Enter the resolution',
                        field: 'resolution',
                        fieldLabel: 'Resolution',
                        required: true,
                      },
                    },
                  ]
                : row.status === 'Resolved'
                  ? [
                      { label: 'Close', value: 'close', variant: 'outlined' },
                      { label: 'Reopen', value: 'reopen' },
                    ]
                  : row.status === 'Closed'
                    ? [{ label: 'Reopen', value: 'reopen' }]
                    : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/complaints/${encodeURIComponent(row.complaintNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
