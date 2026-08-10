import ResourcePage from '../../components/ResourcePage';

export function IndentPage() {
  return (
    <ResourcePage
      title="Indent"
      subtitle="Project material indents feeding procurement."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Indent' }]}
      endpoints={['/indents']}
      createEndpoint="/indents"
      createLabel="New indent"
      allowEdit
      columns={[
        { field: 'indentNumber', header: 'Indent', getValue: (r) => r.indentNumber || r.indentNo || r.id },
        { field: 'projectCode', header: 'Project' },
        { field: 'requestedBy', header: 'Requested by' },
        { field: 'totalAmount', header: 'Amount', type: 'money' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'requestDate', header: 'Date', type: 'date', getValue: (r) => r.requestDate || r.createdAt },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ProjectListPage() {
  return (
    <ResourcePage
      title="Projects"
      subtitle="Create and approve projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Project List' }]}
      endpoints={['/masters/projects']}
      createEndpoint="/masters/projects"
      createLabel="Create project"
      allowEdit
      columns={[
        { field: 'projectCode', header: 'Code' },
        { field: 'projectName', header: 'Name' },
        { field: 'productNo', header: 'Product' },
        { field: 'customerName', header: 'Customer' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'projectName', label: 'Project name', required: true },
        { name: 'productNo', label: 'Product no' },
        { name: 'customerName', label: 'Customer' },
        { name: 'status', label: 'Status', options: ['Active', 'Pending', 'Approved', 'Closed'], defaultValue: 'Active' },
      ]}
    />
  );
}

export function BomApprovePage() {
  return (
    <ResourcePage
      title="Project BOM Approve"
      subtitle="Review and approve project / product BOMs."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'BOM Approve' }]}
      endpoints={['/masters/boms']}
      createEndpoint="/masters/boms"
      createLabel="Create BOM"
      columns={[
        { field: 'bomCode', header: 'BOM', getValue: (r) => r.bomCode || r.code },
        { field: 'projectCode', header: 'Project' },
        { field: 'productCode', header: 'Product' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Created', type: 'datetime' },
      ]}
      fields={[
        { name: 'bomCode', label: 'BOM code', required: true },
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'productCode', label: 'Product code' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function InstallationStatusPage() {
  return (
    <ResourcePage
      title="Installation Status"
      subtitle="Update project installation progress."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Installation' }]}
      endpoints={['/masters/projects']}
      allowCreate={false}
      allowEdit
      createEndpoint="/masters/projects"
      columns={[
        { field: 'projectCode', header: 'Project' },
        { field: 'projectName', header: 'Name' },
        { field: 'installationStatus', header: 'Installation', type: 'status', getValue: (r) => r.installationStatus || r.status },
        { field: 'shipmentDate', header: 'Shipment', type: 'date' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'installationStatus', label: 'Installation status', options: ['Not Started', 'In Progress', 'Completed', 'On Hold'], defaultValue: 'Not Started' },
        { name: 'shipmentDate', label: 'Shipment date', type: 'date' },
      ]}
    />
  );
}

export function ProjectDocumentsPage() {
  return (
    <ResourcePage
      title="Project Documents"
      subtitle="Document checklist and collaboration attachments for projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Documents' }]}
      endpoints={['/masters/projects']}
      allowCreate={false}
      columns={[
        { field: 'projectCode', header: 'Project' },
        { field: 'projectName', header: 'Name' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'customerName', header: 'Customer' },
      ]}
    />
  );
}
