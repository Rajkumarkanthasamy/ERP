import ResourcePage from '../../components/ResourcePage';
import WorkflowActionButtons from '../../components/WorkflowActionButtons';
import { useAuth } from '../../auth/AuthContext';

export default function WorkOrdersPage() {
  const { user } = useAuth();

  return (
    <ResourcePage
      title="Work Orders"
      subtitle="Track production / maintenance work orders linked to procurement."
      crumbs={[
        { label: 'Home', to: '/' },
        { label: 'Procurement', to: '/procurement' },
        { label: 'Work Orders' },
      ]}
      endpoints={['/work-orders']}
      createEndpoint="/work-orders"
      createLabel="New work order"
      rowKey="woNumber"
      mapCreateBody={({ itemCode, itemDescription, quantity, uom, unitCost, ...header }) => ({
        ...header,
        lines: [{ itemCode, itemDescription, quantity, uom, unitCost }],
      })}
      mapUpdateBody={({ projectCode, productNo, vendorCode, vendorName, startDate, dueDate, remarks }) => ({
        projectCode,
        productNo,
        vendorCode,
        vendorName,
        startDate,
        dueDate,
        remarks,
      })}
      columns={[
        { field: 'woNumber', header: 'WO No' },
        { field: 'projectCode', header: 'Project' },
        { field: 'productNo', header: 'Product' },
        { field: 'vendorName', header: 'Vendor' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'dueDate', header: 'Due', type: 'date' },
      ]}
      fields={[
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'productNo', label: 'Product number' },
        { name: 'vendorCode', label: 'Vendor code' },
        { name: 'vendorName', label: 'Vendor name' },
        { name: 'startDate', label: 'Start date', type: 'date' },
        { name: 'dueDate', label: 'Due date', type: 'date' },
        { name: 'remarks', label: 'Remarks', multiline: true },
        { name: 'itemCode', label: 'Item code', required: true, createOnly: true },
        { name: 'itemDescription', label: 'Item description', createOnly: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1, createOnly: true },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS', createOnly: true },
        { name: 'unitCost', label: 'Unit cost', type: 'number', defaultValue: 0, createOnly: true },
      ]}
      allowEdit
      rowActions={(row, reload) => {
        if (!user?.canApprovePR) return null;
        const actions =
          row.status === 'Open'
            ? [
                { label: 'Approve', value: 'approve', variant: 'outlined' },
                { label: 'Cancel', value: 'cancel', color: 'error' },
              ]
            : row.status === 'Approved'
              ? [{ label: 'Start', value: 'start', variant: 'outlined' }]
              : row.status === 'In Progress'
                ? [{ label: 'Complete', value: 'complete', variant: 'outlined' }]
                : row.status === 'Completed'
                  ? [{ label: 'Close', value: 'close', variant: 'outlined' }]
                  : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/work-orders/${encodeURIComponent(row.woNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
