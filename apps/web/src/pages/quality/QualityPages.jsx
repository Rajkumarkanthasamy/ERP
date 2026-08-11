import ResourcePage from '../../components/ResourcePage';
import WorkflowActionButtons from '../../components/WorkflowActionButtons';

export function NcPage() {
  return (
    <ResourcePage
      title="Non-Conformance (NC)"
      subtitle="Raise and track quality non-conformances."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Quality' }, { label: 'NC' }]}
      endpoints={['/quality/ncs']}
      createEndpoint="/quality/ncs"
      createLabel="Raise NC"
      allowEdit
      rowKey="ncNumber"
      columns={[
        { field: 'ncNumber', header: 'NC No', getValue: (r) => r.ncNumber || r.id },
        { field: 'description', header: 'Description' },
        { field: 'projectCode', header: 'Project' },
        { field: 'severity', header: 'Severity', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Date', type: 'datetime' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code' },
        { name: 'itemCode', label: 'Item code' },
        { name: 'severity', label: 'Severity', options: ['Low', 'Medium', 'High', 'Critical'], defaultValue: 'Medium' },
        { name: 'description', label: 'Description', multiline: true, required: true },
        { name: 'assignedTo', label: 'Assigned to' },
        { name: 'correctiveAction', label: 'Corrective action', multiline: true, editOnly: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) => {
        const actions =
          row.status === 'Open'
            ? [
                { label: 'Assign', value: 'assign', variant: 'outlined' },
                { label: 'Escalate', value: 'escalate', color: 'warning' },
              ]
            : ['In Progress', 'Escalated'].includes(row.status)
              ? [
                  {
                    label: 'Close',
                    value: 'close',
                    variant: 'outlined',
                    prompt: {
                      label: 'Enter the corrective action',
                      field: 'correctiveAction',
                      fieldLabel: 'Corrective action',
                      required: true,
                    },
                  },
                ]
              : row.status === 'Closed'
                ? [{ label: 'Reopen', value: 'reopen' }]
                : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/quality/ncs/${encodeURIComponent(row.ncNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}

export function EscalationsPage() {
  return (
    <ResourcePage
      title="Escalations"
      subtitle="Escalate open quality / delivery issues."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Quality' }, { label: 'Escalations' }]}
      endpoints={['/quality/escalations']}
      createEndpoint="/quality/escalations"
      createLabel="Add escalation"
      allowEdit
      rowKey="escalationNumber"
      columns={[
        { field: 'escalationNumber', header: 'Escalation', getValue: (r) => r.escalationNumber || r.id },
        { field: 'relatedRef', header: 'Related ref' },
        { field: 'title', header: 'Title', getValue: (r) => r.title || r.subject },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Date', type: 'datetime' },
      ]}
      fields={[
        { name: 'title', label: 'Title', required: true },
        { name: 'relatedType', label: 'Related type', options: ['NC', 'Delivery', 'Project', 'Service'], defaultValue: 'NC' },
        { name: 'relatedRef', label: 'Related reference' },
        { name: 'projectCode', label: 'Project code' },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High', 'Critical'], defaultValue: 'High' },
        { name: 'description', label: 'Description', multiline: true },
        { name: 'assignedTo', label: 'Assigned to' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) => {
        const actions =
          row.status === 'Open'
            ? [{ label: 'Acknowledge', value: 'acknowledge', variant: 'outlined' }]
            : row.status === 'In Progress'
              ? [{ label: 'Resolve', value: 'resolve', variant: 'outlined' }]
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
            endpoint={`/quality/escalations/${encodeURIComponent(row.escalationNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
