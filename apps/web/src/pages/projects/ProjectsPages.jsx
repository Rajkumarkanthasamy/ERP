import ResourcePage from '../../components/ResourcePage';

export function IndentPage() {
  return (
    <ResourcePage
      title="Project Indents"
      subtitle="Raise material indents against projects for procurement."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Indent' }]}
      endpoints={['/indents', '/projects/indents']}
      createEndpoint="/indents"
      createLabel="New indent"
      columns={[
        { field: 'indentNo', header: 'Indent No', getValue: (r) => r.indentNo || r.docNo || r.id },
        { field: 'projectCode', header: 'Project' },
        { field: 'itemCode', header: 'Item', getValue: (r) => r.itemCode || r.itemDescription },
        { field: 'quantity', header: 'Qty' },
        { field: 'requiredDate', header: 'Required', type: 'date' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
        { name: 'requiredDate', label: 'Required date', type: 'date' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ProjectListPage() {
  return (
    <ResourcePage
      title="Projects"
      subtitle="Create and manage project master records."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'List' }]}
      endpoints={['/projects', '/masters/projects']}
      createEndpoint="/projects"
      createLabel="Create project"
      allowEdit
      columns={[
        { field: 'projectCode', header: 'Code' },
        { field: 'projectName', header: 'Name' },
        { field: 'productNo', header: 'Product' },
        { field: 'customerName', header: 'Customer', getValue: (r) => r.customerName || r.customerCode || '—' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'projectName', label: 'Project name', required: true },
        { name: 'productNo', label: 'Product no' },
        { name: 'customerCode', label: 'Customer code' },
        { name: 'status', label: 'Status', options: ['Active', 'On Hold', 'Completed', 'Cancelled'], defaultValue: 'Active' },
      ]}
    />
  );
}

export function BomApprovePage() {
  return (
    <ResourcePage
      title="BOM Approve"
      subtitle="Review and approve bills of material for projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'BOM Approve' }]}
      endpoints={['/projects/bom', '/boms']}
      createLabel="Submit BOM"
      columns={[
        { field: 'bomCode', header: 'BOM', getValue: (r) => r.bomCode || r.docNo || r.id },
        { field: 'projectCode', header: 'Project' },
        { field: 'revision', header: 'Rev', getValue: (r) => r.revision || r.rev },
        { field: 'itemCount', header: 'Items', getValue: (r) => r.itemCount ?? r.lines?.length ?? '—' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'bomCode', label: 'BOM code' },
        { name: 'revision', label: 'Revision', defaultValue: 'A' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function InstallationStatusPage() {
  return (
    <ResourcePage
      title="Installation Status"
      subtitle="Track site installation progress by project."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Installation' }]}
      endpoints={['/projects/installation', '/installations']}
      createLabel="Update status"
      allowEdit
      columns={[
        { field: 'projectCode', header: 'Project' },
        { field: 'site', header: 'Site', getValue: (r) => r.site || r.location },
        { field: 'progressPct', header: 'Progress %', getValue: (r) => r.progressPct ?? r.progress },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'updatedAt', header: 'Updated', type: 'datetime', getValue: (r) => r.updatedAt || r.modifiedAt },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'site', label: 'Site' },
        { name: 'progressPct', label: 'Progress %', type: 'number', defaultValue: 0 },
        { name: 'status', label: 'Status', options: ['Not Started', 'In Progress', 'Completed', 'On Hold'], defaultValue: 'In Progress' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ProjectDocumentsPage() {
  return (
    <ResourcePage
      title="Project Documents"
      subtitle="Document register for drawings, manuals and handover packs."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Documents' }]}
      endpoints={['/projects/documents', '/documents']}
      createLabel="Add document"
      columns={[
        { field: 'docNo', header: 'Doc No', getValue: (r) => r.docNo || r.id },
        { field: 'projectCode', header: 'Project' },
        { field: 'title', header: 'Title', getValue: (r) => r.title || r.fileName || r.name },
        { field: 'docType', header: 'Type', getValue: (r) => r.docType || r.type },
        { field: 'uploadedAt', header: 'Uploaded', type: 'datetime', getValue: (r) => r.uploadedAt || r.createdAt },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'title', label: 'Title', required: true },
        { name: 'docType', label: 'Type', options: ['Drawing', 'Manual', 'Certificate', 'Other'], defaultValue: 'Drawing' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
