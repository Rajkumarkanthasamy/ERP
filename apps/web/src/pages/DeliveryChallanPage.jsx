import ResourcePage from '../components/ResourcePage';

export default function DeliveryChallanPage() {
  return (
    <ResourcePage
      title="Delivery Challan"
      subtitle="Create and track delivery challans for dispatches."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Delivery Challan' }]}
      endpoints={['/delivery-challans', '/challans']}
      createEndpoint="/delivery-challans"
      createLabel="New challan"
      allowEdit
      columns={[
        { field: 'challanNo', header: 'Challan', getValue: (r) => r.challanNo || r.docNo || r.id },
        { field: 'customerName', header: 'Customer', getValue: (r) => r.customerName || r.partyName },
        { field: 'projectCode', header: 'Project' },
        { field: 'vehicleNo', header: 'Vehicle' },
        { field: 'challanDate', header: 'Date', type: 'date', getValue: (r) => r.challanDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'vehicleNo', label: 'Vehicle no' },
        { name: 'itemDescription', label: 'Items / description', required: true, multiline: true },
        { name: 'status', label: 'Status', options: ['Draft', 'Dispatched', 'Delivered', 'Cancelled'], defaultValue: 'Draft' },
      ]}
    />
  );
}
