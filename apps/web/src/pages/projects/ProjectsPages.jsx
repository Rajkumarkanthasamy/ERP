import ResourcePage from '../../components/ResourcePage';
import WorkflowActionButtons from '../../components/WorkflowActionButtons';
import { useAuth } from '../../auth/AuthContext';

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
      rowKey="indentNumber"
      mapCreateBody={({ itemCode, itemDescription, quantity, uom, ...header }) => ({
        ...header,
        lines: [{ itemCode, itemDescription, quantity, uom }],
      })}
      mapUpdateBody={({ projectCode, requiredDate, remarks }) => ({
        projectCode,
        requiredDate,
        remarks,
      })}
      columns={[
        { field: 'indentNumber', header: 'Indent', getValue: (r) => r.indentNumber || r.indentNo || r.id },
        { field: 'projectCode', header: 'Project' },
        { field: 'requestedBy', header: 'Requested by' },
        { field: 'requiredDate', header: 'Required', type: 'date' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'requestDate', header: 'Date', type: 'date', getValue: (r) => r.requestDate || r.createdAt },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'requiredDate', label: 'Required date', type: 'date' },
        { name: 'remarks', label: 'Remarks', multiline: true },
        { name: 'itemCode', label: 'Item code', required: true, createOnly: true },
        { name: 'itemDescription', label: 'Item description', createOnly: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1, createOnly: true },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS', createOnly: true },
      ]}
    />
  );
}

export function ProjectListPage() {
  const { user } = useAuth();

  return (
    <ResourcePage
      title="Projects"
      subtitle="Create and approve projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'Project List' }]}
      endpoints={['/masters/projects']}
      createEndpoint="/masters/projects"
      createLabel="Create project"
      allowEdit
      rowKey="projectCode"
      columns={[
        { field: 'projectCode', header: 'Code' },
        { field: 'projectName', header: 'Name' },
        { field: 'productNo', header: 'Product' },
        { field: 'customerCode', header: 'Customer' },
        { field: 'approvalStatus', header: 'Approval', type: 'status' },
        { field: 'status', header: 'Project status', type: 'status' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true, disabledOnEdit: true },
        { name: 'projectName', label: 'Project name', required: true },
        { name: 'productNo', label: 'Product no' },
        { name: 'customerCode', label: 'Customer code' },
        { name: 'pmName', label: 'Project manager' },
        { name: 'startDate', label: 'Start date', type: 'date' },
        { name: 'endDate', label: 'End date', type: 'date' },
        { name: 'status', label: 'Status', options: ['Draft', 'Active', 'On Hold', 'Closed'], defaultValue: 'Draft' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) =>
        row.approvalStatus === 'Pending' && user?.canApprovePR ? (
          <WorkflowActionButtons
            endpoint={`/masters/projects/${encodeURIComponent(row.projectCode)}/approve`}
            onComplete={reload}
            actions={[
              { label: 'Approve', value: 'approve', variant: 'outlined' },
              {
                label: 'Reject',
                value: 'reject',
                color: 'error',
                confirm: `Reject project ${row.projectCode}?`,
              },
            ]}
          />
        ) : null
      }
    />
  );
}

export function BomApprovePage() {
  const { user } = useAuth();

  return (
    <ResourcePage
      title="Project BOM Approve"
      subtitle="Review and approve project / product BOMs."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Projects' }, { label: 'BOM Approve' }]}
      endpoints={['/masters/boms']}
      createEndpoint="/masters/boms"
      createLabel="Create BOM"
      rowKey="bomCode"
      mapCreateBody={({ itemCode, itemDescription, quantity, uom, ...header }) => ({
        ...header,
        lines: itemCode ? [{ itemCode, itemDescription, quantity, uom }] : [],
      })}
      columns={[
        { field: 'bomCode', header: 'BOM', getValue: (r) => r.bomCode || r.code },
        { field: 'productCode', header: 'Product' },
        { field: 'version', header: 'Version' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'createdAt', header: 'Created', type: 'datetime' },
      ]}
      fields={[
        { name: 'bomCode', label: 'BOM code', required: true },
        { name: 'productCode', label: 'Product code', required: true },
        { name: 'productName', label: 'Product name' },
        { name: 'version', label: 'Version', defaultValue: '1.0' },
        { name: 'remarks', label: 'Remarks', multiline: true },
        { name: 'itemCode', label: 'First item code' },
        { name: 'itemDescription', label: 'First item description' },
        { name: 'quantity', label: 'Quantity', type: 'number', defaultValue: 1 },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS' },
      ]}
      rowActions={(row, reload) =>
        row.status === 'Draft' && user?.canApprovePR ? (
          <WorkflowActionButtons
            endpoint={`/masters/boms/${encodeURIComponent(row.bomCode)}/approve`}
            onComplete={reload}
            actions={[
              { label: 'Approve', value: 'approve', variant: 'outlined' },
              {
                label: 'Reject',
                value: 'reject',
                color: 'error',
                confirm: `Reject BOM ${row.bomCode}?`,
              },
            ]}
          />
        ) : null
      }
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
      rowKey="projectCode"
      columns={[
        { field: 'projectCode', header: 'Project' },
        { field: 'projectName', header: 'Name' },
        { field: 'installationStatus', header: 'Installation', type: 'status' },
        { field: 'shipmentDate', header: 'Shipment', type: 'date' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true, disabledOnEdit: true },
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
