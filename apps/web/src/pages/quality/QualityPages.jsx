import ResourcePage from '../../components/ResourcePage';

export function NcPage() {
  return (
    <ResourcePage
      title="Non-Conformance (NC)"
      subtitle="Quality non-conformance register and dispositions."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Quality' }, { label: 'NC' }]}
      endpoints={['/ncs', '/quality/ncs']}
      createEndpoint="/ncs"
      createLabel="Raise NC"
      allowEdit
      columns={[
        { field: 'ncNo', header: 'NC No', getValue: (r) => r.ncNo || r.docNo || r.id },
        { field: 'itemCode', header: 'Item', getValue: (r) => r.itemCode || r.partCode },
        { field: 'source', header: 'Source', getValue: (r) => r.source || r.origin },
        { field: 'severity', header: 'Severity', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'raisedDate', header: 'Raised', type: 'date', getValue: (r) => r.raisedDate || r.createdAt },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item / part code' },
        { name: 'source', label: 'Source', options: ['Incoming', 'In-process', 'Customer', 'Audit'], defaultValue: 'Incoming' },
        { name: 'severity', label: 'Severity', options: ['Minor', 'Major', 'Critical'], defaultValue: 'Minor' },
        { name: 'description', label: 'Description', required: true, multiline: true, minRows: 3 },
        { name: 'status', label: 'Status', options: ['Open', 'Under Review', 'Closed'], defaultValue: 'Open' },
      ]}
    />
  );
}

export function EscalationsPage() {
  return (
    <ResourcePage
      title="Escalations"
      subtitle="Quality and delivery escalations requiring management attention."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Quality' }, { label: 'Escalations' }]}
      endpoints={['/escalations', '/quality/escalations', '/ncs']}
      createEndpoint="/escalations"
      createLabel="New escalation"
      allowEdit
      columns={[
        { field: 'escalationNo', header: 'Escalation', getValue: (r) => r.escalationNo || r.docNo || r.id },
        { field: 'relatedRef', header: 'Related', getValue: (r) => r.relatedRef || r.ncNo || r.refNo },
        { field: 'owner', header: 'Owner', getValue: (r) => r.owner || r.assignedTo },
        { field: 'priority', header: 'Priority', type: 'status' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'dueDate', header: 'Due', type: 'date' },
      ]}
      fields={[
        { name: 'relatedRef', label: 'Related NC / ref' },
        { name: 'owner', label: 'Owner' },
        { name: 'priority', label: 'Priority', options: ['Low', 'Medium', 'High'], defaultValue: 'High' },
        { name: 'dueDate', label: 'Due date', type: 'date' },
        { name: 'description', label: 'Description', required: true, multiline: true },
        { name: 'status', label: 'Status', options: ['Open', 'In Progress', 'Closed'], defaultValue: 'Open' },
      ]}
    />
  );
}
