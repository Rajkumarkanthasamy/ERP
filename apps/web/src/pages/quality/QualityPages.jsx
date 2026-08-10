import ResourcePage from '../../components/ResourcePage';

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
      ]}
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
      columns={[
        { field: 'escalationNumber', header: 'Escalation', getValue: (r) => r.escalationNumber || r.id },
        { field: 'ncNumber', header: 'NC Ref' },
        { field: 'title', header: 'Title', getValue: (r) => r.title || r.subject },
        { field: 'level', header: 'Level' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Date', type: 'datetime' },
      ]}
      fields={[
        { name: 'title', label: 'Title', required: true },
        { name: 'ncNumber', label: 'NC number' },
        { name: 'level', label: 'Level', options: ['L1', 'L2', 'L3'], defaultValue: 'L1' },
        { name: 'description', label: 'Description', multiline: true },
      ]}
    />
  );
}
