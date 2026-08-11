import ResourcePage from '../components/ResourcePage';
import WorkflowActionButtons from '../components/WorkflowActionButtons';

export default function DeliveryChallanPage() {
  return (
    <ResourcePage
      title="Delivery Challan"
      subtitle="Create and track delivery challans for dispatches."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Delivery Challan' }]}
      endpoints={['/delivery-challans']}
      createEndpoint="/delivery-challans"
      createLabel="New challan"
      allowEdit
      rowKey="dcNumber"
      mapCreateBody={({ itemCode, itemDescription, quantity, uom, ...header }) => ({
        ...header,
        lines: [{ itemCode, itemDescription, quantity, uom }],
      })}
      mapUpdateBody={({ customerName, projectCode, vehicleNo, transporter, ginNumber, remarks }) => ({
        customerName,
        projectCode,
        vehicleNo,
        transporter,
        ginNumber,
        remarks,
      })}
      columns={[
        { field: 'dcNumber', header: 'Challan' },
        { field: 'customerName', header: 'Customer', getValue: (r) => r.customerName || r.partyName },
        { field: 'projectCode', header: 'Project' },
        { field: 'vehicleNo', header: 'Vehicle' },
        { field: 'dcDate', header: 'Date', type: 'date' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'vehicleNo', label: 'Vehicle no' },
        { name: 'transporter', label: 'Transporter' },
        { name: 'ginNumber', label: 'GIN number' },
        { name: 'remarks', label: 'Remarks', multiline: true },
        { name: 'itemCode', label: 'Item code', required: true, createOnly: true },
        { name: 'itemDescription', label: 'Item description', multiline: true, createOnly: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1, createOnly: true },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS', createOnly: true },
      ]}
      rowActions={(row, reload) => {
        const actions =
          row.status === 'Draft'
            ? [
                { label: 'Issue', value: 'issue', variant: 'outlined' },
                { label: 'Cancel', value: 'cancel', color: 'error' },
              ]
            : row.status === 'Issued'
              ? [
                  { label: 'Dispatch', value: 'dispatch', variant: 'outlined' },
                  { label: 'Cancel', value: 'cancel', color: 'error' },
                ]
              : row.status === 'Dispatched'
                ? [{ label: 'Deliver', value: 'deliver', variant: 'outlined' }]
                : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/delivery-challans/${encodeURIComponent(row.dcNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
