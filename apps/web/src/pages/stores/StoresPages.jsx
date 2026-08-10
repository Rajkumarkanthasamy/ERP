import ResourcePage from '../../components/ResourcePage';

function stockColumns(docLabel) {
  return [
    { field: 'txnNumber', header: docLabel, getValue: (r) => r.txnNumber || r.docNo || r.id },
    { field: 'itemCode', header: 'Item' },
    { field: 'itemDescription', header: 'Description' },
    { field: 'quantity', header: 'Qty' },
    { field: 'uom', header: 'UOM' },
    { field: 'projectCode', header: 'Project' },
    { field: 'createdAt', header: 'Date', type: 'datetime' },
    { field: 'status', header: 'Status', type: 'status' },
  ];
}

const stockFields = [
  { name: 'itemCode', label: 'Item code', required: true },
  { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
  { name: 'uom', label: 'UOM', defaultValue: 'NOS' },
  { name: 'projectCode', label: 'Project code' },
  { name: 'referenceNo', label: 'Reference no' },
  { name: 'remarks', label: 'Remarks', multiline: true },
];

export function StockLedgerPage() {
  return (
    <ResourcePage
      title="Stock Ledger"
      subtitle="On-hand balances by item."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Stock Ledger' }]}
      endpoints={['/stock/ledger', '/stock']}
      allowCreate={false}
      columns={[
        { field: 'itemCode', header: 'Item' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'location', header: 'Location', getValue: (r) => r.location || r.warehouse || 'MAIN' },
        { field: 'quantityOnHand', header: 'On hand', getValue: (r) => r.quantityOnHand ?? r.qtyOnHand ?? r.quantity },
        { field: 'reservedQty', header: 'Reserved' },
        { field: 'uom', header: 'UOM' },
        { field: 'lastTxnDate', header: 'Last txn', type: 'datetime' },
      ]}
    />
  );
}

export function GinReceiptPage() {
  return (
    <ResourcePage
      title="GIN Receipt"
      subtitle="Goods inward notes for store receipt."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'GIN Receipt' }]}
      endpoints={['/stock/gin']}
      createEndpoint="/stock/gin"
      createLabel="New GIN"
      columns={stockColumns('GIN No')}
      fields={stockFields}
      mapCreateBody={(form) => ({ ...form, txnType: 'GIN' })}
    />
  );
}

export function ItemIssuePage() {
  return (
    <ResourcePage
      title="Item Issue"
      subtitle="Issue stock to projects, departments or production."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Issue' }]}
      endpoints={['/stock/issue']}
      createEndpoint="/stock/issue"
      createLabel="Issue items"
      columns={stockColumns('Issue No')}
      fields={[
        ...stockFields,
        { name: 'toLocation', label: 'Issued to / location' },
      ]}
      mapCreateBody={(form) => ({ ...form, txnType: 'Issue' })}
    />
  );
}

export function ItemReturnPage() {
  return (
    <ResourcePage
      title="Item Return"
      subtitle="Return unused material to stores."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Return' }]}
      endpoints={['/stock/return']}
      createEndpoint="/stock/return"
      createLabel="Return items"
      columns={stockColumns('Return No')}
      fields={stockFields}
      mapCreateBody={(form) => ({ ...form, txnType: 'Return' })}
    />
  );
}

export function StockAdjustPage() {
  return (
    <ResourcePage
      title="Stock Adjust"
      subtitle="Adjust on-hand quantity with remarks."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Stock Adjust' }]}
      endpoints={['/stock/adjust']}
      createEndpoint="/stock/adjust"
      createLabel="Adjust stock"
      columns={stockColumns('Adjust No')}
      fields={stockFields}
      mapCreateBody={(form) => ({ ...form, txnType: 'Adjust' })}
    />
  );
}

export function ItemProductionPage() {
  return (
    <ResourcePage
      title="Item Production"
      subtitle="Record finished / semi-finished production receipts."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Production' }]}
      endpoints={['/stock/production']}
      createEndpoint="/stock/production"
      createLabel="Post production"
      columns={stockColumns('Production No')}
      fields={stockFields}
      mapCreateBody={(form) => ({ ...form, txnType: 'Production' })}
    />
  );
}
