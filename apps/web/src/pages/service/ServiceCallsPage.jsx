import ResourcePage from '../../components/ResourcePage';
import WorkflowActionButtons from '../../components/WorkflowActionButtons';

export default function ServiceCallsPage() {
  return (
    <ResourcePage
      title="Service Calls"
      subtitle="Log and manage customer service / AMC calls."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Service' }, { label: 'Service Calls' }]}
      endpoints={['/service-calls']}
      createEndpoint="/service-calls"
      createLabel="New call"
      allowEdit
      rowKey="callNumber"
      columns={[
        { field: 'callNumber', header: 'Call No' },
        { field: 'customerName', header: 'Customer' },
        { field: 'subject', header: 'Subject', getValue: (r) => r.subject || r.issue },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'assignedTo', header: 'Assigned' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'reportedDate', header: 'Date', type: 'datetime', getValue: (r) => r.reportedDate || r.createdAt },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'subject', label: 'Subject', required: true },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High', 'Critical'], defaultValue: 'Medium' },
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
            endpoint={`/service-calls/${encodeURIComponent(row.callNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
