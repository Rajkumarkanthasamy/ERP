import ResourcePage from '../../components/ResourcePage';

export default function WorkOrdersPage() {
  return (
    <ResourcePage
      title="Work Orders"
      subtitle="Track production / maintenance work orders linked to procurement."
      crumbs={[
        { label: 'Home', to: '/' },
        { label: 'Procurement', to: '/procurement' },
        { label: 'Work Orders' },
      ]}
      endpoints={['/work-orders', '/masters/work-orders']}
      createEndpoint="/work-orders"
      createLabel="New work order"
      columns={[
        { field: 'woNumber', header: 'WO No', getValue: (r) => r.woNumber || r.workOrderNo || r.id },
        { field: 'projectCode', header: 'Project', getValue: (r) => r.projectCode || r.project },
        { field: 'itemCode', header: 'Item', getValue: (r) => r.itemCode || r.item },
        { field: 'quantity', header: 'Qty' },
        { field: 'status', header: 'Status', type: 'status' },
        { field: 'dueDate', header: 'Due', type: 'date', getValue: (r) => r.dueDate || r.targetDate },
      ]}
      fields={[
        { name: 'woNumber', label: 'WO Number' },
        { name: 'projectCode', label: 'Project code', required: true },
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', defaultValue: 1 },
        {
          name: 'status',
          label: 'Status',
          options: ['Open', 'In Progress', 'Completed', 'Cancelled'],
          defaultValue: 'Open',
        },
        { name: 'dueDate', label: 'Due date', type: 'date' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      allowEdit
    />
  );
}
