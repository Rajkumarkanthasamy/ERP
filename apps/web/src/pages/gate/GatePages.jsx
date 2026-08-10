import ResourcePage from '../../components/ResourcePage';

const gateColumns = [
  { field: 'entryNumber', header: 'Entry No', getValue: (r) => r.entryNumber || r.entryNo || r.id },
  { field: 'entryType', header: 'Type', type: 'status' },
  { field: 'vehicleNo', header: 'Vehicle' },
  { field: 'vendorName', header: 'Party', getValue: (r) => r.vendorName || r.partyName || r.customerName },
  { field: 'purpose', header: 'Purpose' },
  { field: 'createdAt', header: 'Date', type: 'datetime', getValue: (r) => r.createdAt || r.entryDate },
  { field: 'status', header: 'Status', type: 'status' },
];

export function GateInwardPage() {
  return (
    <ResourcePage
      title="Gate Inward"
      subtitle="Record vehicles and materials entering the premises."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Gate Entry' }, { label: 'Inward' }]}
      endpoints={['/gate-entries']}
      createEndpoint="/gate-entries"
      createLabel="New inward"
      mapCreateBody={(form) => ({ ...form, entryType: 'Inward' })}
      filters={[{ name: 'type', label: 'Type', options: ['Inward', 'Outward', 'Manual', 'All'] }]}
      columns={gateColumns}
      fields={[
        { name: 'vehicleNo', label: 'Vehicle no', required: true },
        { name: 'vendorName', label: 'Party / vendor name', required: true },
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
      mapCreateBody={(form) => ({ ...form, entryType: 'Outward' })}
      columns={gateColumns}
      fields={[
        { name: 'vehicleNo', label: 'Vehicle no', required: true },
        { name: 'vendorName', label: 'Party / vendor name', required: true },
        { name: 'purpose', label: 'Purpose' },
        { name: 'invoiceNo', label: 'Invoice / DC no' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ManualInwardPage() {
  return (
    <ResourcePage
      title="Gate Manual Inward"
      subtitle="Manual gate inward when no PO / invoice is available."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Gate Entry' }, { label: 'Manual Inward' }]}
      endpoints={['/gate-entries']}
      createEndpoint="/gate-entries"
      createLabel="Manual inward"
      mapCreateBody={(form) => ({ ...form, entryType: 'Manual' })}
      columns={gateColumns}
      fields={[
        { name: 'vehicleNo', label: 'Vehicle no' },
        { name: 'vendorName', label: 'Party / vendor name', required: true },
        { name: 'purpose', label: 'Purpose', required: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
