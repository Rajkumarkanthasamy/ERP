import ResourcePage from '../../components/ResourcePage';

export function StockLedgerPage() {
  return (
    <ResourcePage
      title="Stock Ledger"
      subtitle="On-hand balances and movement history."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Stock Ledger' }]}
      endpoints={['/stock', '/stock/ledger', '/stores/stock']}
      allowCreate={false}
      columns={[
        { field: 'itemCode', header: 'Item' },
        { field: 'itemDescription', header: 'Description', getValue: (r) => r.itemDescription || r.description },
        { field: 'warehouse', header: 'Warehouse', getValue: (r) => r.warehouse || r.location || 'MAIN' },
        { field: 'qtyOnHand', header: 'On hand', getValue: (r) => r.qtyOnHand ?? r.quantity ?? r.qty },
        { field: 'uom', header: 'UOM' },
        { field: 'value', header: 'Value', type: 'money', getValue: (r) => r.value || r.stockValue },
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
      endpoints={['/stock/gin', '/stores/gin', '/gins']}
      createEndpoint="/stock/gin"
      createLabel="New GIN"
      columns={[
        { field: 'ginNumber', header: 'GIN No', getValue: (r) => r.ginNumber || r.docNo || r.id },
        { field: 'itemCode', header: 'Item' },
        { field: 'quantity', header: 'Qty' },
        { field: 'vendorCode', header: 'Vendor', getValue: (r) => r.vendorCode || r.vendorName },
        { field: 'ginDate', header: 'Date', type: 'date', getValue: (r) => r.ginDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
        { name: 'vendorCode', label: 'Vendor code' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ItemIssuePage() {
  return (
    <ResourcePage
      title="Item Issue"
      subtitle="Issue stock to projects, departments or production."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Issue' }]}
      endpoints={['/stock/issue', '/stores/issue']}
      createEndpoint="/stock/issue"
      createLabel="Issue items"
      columns={[
        { field: 'issueNo', header: 'Issue No', getValue: (r) => r.issueNo || r.docNo || r.id },
        { field: 'itemCode', header: 'Item' },
        { field: 'quantity', header: 'Qty' },
        { field: 'projectCode', header: 'Project' },
        { field: 'issuedTo', header: 'Issued to', getValue: (r) => r.issuedTo || r.department },
        { field: 'issueDate', header: 'Date', type: 'date', getValue: (r) => r.issueDate || r.createdAt },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
        { name: 'projectCode', label: 'Project code' },
        { name: 'issuedTo', label: 'Issued to' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ItemReturnPage() {
  return (
    <ResourcePage
      title="Item Return"
      subtitle="Return previously issued materials to stores."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Return' }]}
      endpoints={['/stock/return', '/stores/return']}
      createEndpoint="/stock/return"
      createLabel="Record return"
      columns={[
        { field: 'returnNo', header: 'Return No', getValue: (r) => r.returnNo || r.docNo || r.id },
        { field: 'itemCode', header: 'Item' },
        { field: 'quantity', header: 'Qty' },
        { field: 'projectCode', header: 'Project' },
        { field: 'returnDate', header: 'Date', type: 'date', getValue: (r) => r.returnDate || r.createdAt },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
        { name: 'projectCode', label: 'Project code' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function StockAdjustPage() {
  return (
    <ResourcePage
      title="Stock Adjust"
      subtitle="Positive / negative stock adjustments with reason codes."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Stock Adjust' }]}
      endpoints={['/stock/adjust', '/stores/adjust']}
      createEndpoint="/stock/adjust"
      createLabel="New adjustment"
      columns={[
        { field: 'adjustNo', header: 'Doc No', getValue: (r) => r.adjustNo || r.docNo || r.id },
        { field: 'itemCode', header: 'Item' },
        { field: 'quantity', header: 'Qty (+/-)' },
        { field: 'reason', header: 'Reason' },
        { field: 'adjustDate', header: 'Date', type: 'date', getValue: (r) => r.adjustDate || r.createdAt },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity (+/-)', type: 'number', required: true },
        { name: 'reason', label: 'Reason', required: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function ItemProductionPage() {
  return (
    <ResourcePage
      title="Item Production"
      subtitle="Record finished-goods production receipts into stock."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Stores' }, { label: 'Item Production' }]}
      endpoints={['/stock/production', '/stores/production']}
      createEndpoint="/stock/production"
      createLabel="Record production"
      columns={[
        { field: 'productionNo', header: 'Doc No', getValue: (r) => r.productionNo || r.docNo || r.id },
        { field: 'itemCode', header: 'Item' },
        { field: 'quantity', header: 'Qty' },
        { field: 'workOrderNo', header: 'Work order', getValue: (r) => r.workOrderNo || r.woNumber },
        { field: 'productionDate', header: 'Date', type: 'date', getValue: (r) => r.productionDate || r.createdAt },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'quantity', label: 'Quantity', type: 'number', required: true, defaultValue: 1 },
        { name: 'workOrderNo', label: 'Work order' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
