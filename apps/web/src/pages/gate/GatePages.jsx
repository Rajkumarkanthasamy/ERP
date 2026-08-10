import ResourcePage from '../../components/ResourcePage';

export function GateInwardPage() {
  return (
    <ResourcePage
      title="Gate Inward"
      subtitle="Record vehicles and materials entering the premises."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Gate Entry' }, { label: 'Inward' }]}
      endpoints={['/gate-entries', '/gate-entries/inward']}
      createEndpoint="/gate-entries"
      createLabel="New inward"
      mapCreateBody={(form) => ({ ...form, direction: 'Inward' })}
      filters={[{ name: 'direction', label: 'Direction', options: ['Inward', 'Outward', 'All'] }]}
      columns={[
        { field: 'entryNo', header: 'Entry No', getValue: (r) => r.entryNo || r.docNo || r.id },
        { field: 'vehicleNo', header: 'Vehicle', getValue: (r) => r.vehicleNo || r.vehicleNumber },
        { field: 'partyName', header: 'Party', getValue: (r) => r.partyName || r.vendorName || r.customerName },
        { field: 'purpose', header: 'Purpose' },
        { field: 'entryDate', header: 'Date', type: 'datetime', getValue: (r) => r.entryDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'vehicleNo', label: 'Vehicle no', required: true },
        { name: 'partyName', label: 'Party name', required: true },
        { name: 'purpose', label: 'Purpose' },
        { name: 'invoiceNo', label: 'Invoice / DC no' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function GateOutwardPage() {
  return (
    <ResourcePage
      title="Gate Outward"
      subtitle="Authorize and log materials / vehicles leaving the gate."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Gate Entry' }, { label: 'Outward' }]}
      endpoints={['/gate-entries']}
      createEndpoint="/gate-entries"
      createLabel="New outward"
      mapCreateBody={(form) => ({ ...form, direction: 'Outward' })}
      columns={[
        { field: 'entryNo', header: 'Entry No', getValue: (r) => r.entryNo || r.docNo || r.id },
        { field: 'vehicleNo', header: 'Vehicle', getValue: (r) => r.vehicleNo || r.vehicleNumber },
        { field: 'partyName', header: 'Party', getValue: (r) => r.partyName || r.customerName },
        { field: 'purpose', header: 'Purpose' },
        { field: 'entryDate', header: 'Date', type: 'datetime', getValue: (r) => r.entryDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'vehicleNo', label: 'Vehicle no', required: true },
        { name: 'partyName', label: 'Party name', required: true },
        { name: 'purpose', label: 'Purpose' },
        { name: 'challanNo', label: 'Delivery challan' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      transformRows={(rows) => rows.filter((r) => !r.direction || String(r.direction).toLowerCase().includes('out'))}
    />
  );
}

export function ManualInwardPage() {
  return (
    <ResourcePage
      title="Manual Inward"
      subtitle="Manual gate inward for cash purchases or non-PO receipts."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Gate Entry' }, { label: 'Manual Inward' }]}
      endpoints={['/gate-entries/manual', '/gate-entries']}
      createEndpoint="/gate-entries"
      createLabel="Manual inward"
      mapCreateBody={(form) => ({ ...form, direction: 'Inward', manual: true })}
      columns={[
        { field: 'entryNo', header: 'Entry No', getValue: (r) => r.entryNo || r.docNo || r.id },
        { field: 'itemDescription', header: 'Material', getValue: (r) => r.itemDescription || r.material },
        { field: 'quantity', header: 'Qty' },
        { field: 'partyName', header: 'From' },
        { field: 'entryDate', header: 'Date', type: 'datetime', getValue: (r) => r.entryDate || r.createdAt },
      ]}
      fields={[
        { name: 'partyName', label: 'Received from', required: true },
        { name: 'itemDescription', label: 'Material description', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', defaultValue: 1 },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
