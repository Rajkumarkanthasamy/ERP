import ResourcePage from '../../components/ResourcePage';

export function CitiesPage() {
  return (
    <ResourcePage
      title="Cities"
      subtitle="Maintain city master used across customers, vendors and projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Cities' }]}
      endpoints={['/cities', '/masters/cities']}
      createLabel="Add city"
      allowEdit
      allowDelete
      columns={[
        { field: 'cityCode', header: 'Code', getValue: (r) => r.cityCode || r.code },
        { field: 'cityName', header: 'City', getValue: (r) => r.cityName || r.name },
        { field: 'state', header: 'State' },
        { field: 'country', header: 'Country', getValue: (r) => r.country || 'India' },
        { field: 'active', header: 'Active', render: (v) => (v === 0 || v === false ? 'No' : 'Yes') },
      ]}
      fields={[
        { name: 'cityCode', label: 'City code', required: true },
        { name: 'cityName', label: 'City name', required: true },
        { name: 'state', label: 'State' },
        { name: 'country', label: 'Country', defaultValue: 'India' },
      ]}
    />
  );
}

export function CustomersPage() {
  return (
    <ResourcePage
      title="Customers"
      subtitle="Customer master for sales, projects and delivery."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Customers' }]}
      endpoints={['/customers', '/masters/customers']}
      createLabel="Add customer"
      allowEdit
      columns={[
        { field: 'customerCode', header: 'Code', getValue: (r) => r.customerCode || r.code },
        { field: 'customerName', header: 'Name', getValue: (r) => r.customerName || r.name },
        { field: 'city', header: 'City' },
        { field: 'gstin', header: 'GSTIN' },
        { field: 'phone', header: 'Phone', getValue: (r) => r.phone || r.mobile },
        { field: 'status', header: 'Status', type: 'status', getValue: (r) => r.status || (r.active === 0 ? 'Inactive' : 'Active') },
      ]}
      fields={[
        { name: 'customerCode', label: 'Customer code', required: true },
        { name: 'customerName', label: 'Customer name', required: true },
        { name: 'city', label: 'City' },
        { name: 'gstin', label: 'GSTIN' },
        { name: 'phone', label: 'Phone' },
        { name: 'email', label: 'Email' },
        { name: 'address', label: 'Address', multiline: true },
      ]}
    />
  );
}

export function VendorsPage() {
  return (
    <ResourcePage
      title="Vendors"
      subtitle="Vendor master for procurement and GRN."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Vendors' }]}
      endpoints={['/vendors', '/masters/vendors']}
      createEndpoint="/vendors"
      createLabel="Add vendor"
      allowEdit
      columns={[
        { field: 'vendorCode', header: 'Code' },
        { field: 'vendorName', header: 'Name' },
        { field: 'city', header: 'City' },
        { field: 'gstin', header: 'GSTIN' },
        { field: 'active', header: 'Active', render: (v) => (v === 0 || v === false ? 'No' : 'Yes') },
      ]}
      fields={[
        { name: 'vendorCode', label: 'Vendor code', required: true },
        { name: 'vendorName', label: 'Vendor name', required: true },
        { name: 'city', label: 'City' },
        { name: 'gstin', label: 'GSTIN' },
      ]}
    />
  );
}

export function ItemsPage() {
  return (
    <ResourcePage
      title="Items"
      subtitle="Item master with UOM, HSN and costing fields."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Items' }]}
      endpoints={['/items', '/masters/items']}
      createEndpoint="/items"
      createLabel="Add item"
      allowEdit
      columns={[
        { field: 'itemCode', header: 'Code' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'make', header: 'Make' },
        { field: 'uom', header: 'UOM' },
        { field: 'hsnCode', header: 'HSN' },
        { field: 'standardCost', header: 'Std cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Latest', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'itemDescription', label: 'Description', required: true },
        { name: 'specification', label: 'Specification', multiline: true },
        { name: 'make', label: 'Make' },
        { name: 'mfgPartNo', label: 'MFG part no' },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS' },
        { name: 'hsnCode', label: 'HSN' },
        { name: 'standardCost', label: 'Standard cost', type: 'number', defaultValue: 0 },
      ]}
    />
  );
}

export function AssetsPage() {
  return (
    <ResourcePage
      title="Assets"
      subtitle="Fixed assets and equipment register."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Assets' }]}
      endpoints={['/assets', '/masters/assets']}
      createLabel="Add asset"
      allowEdit
      columns={[
        { field: 'assetCode', header: 'Code', getValue: (r) => r.assetCode || r.code },
        { field: 'assetName', header: 'Name', getValue: (r) => r.assetName || r.name },
        { field: 'category', header: 'Category' },
        { field: 'location', header: 'Location' },
        { field: 'purchaseValue', header: 'Value', type: 'money', getValue: (r) => r.purchaseValue || r.value },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'assetCode', label: 'Asset code', required: true },
        { name: 'assetName', label: 'Asset name', required: true },
        { name: 'category', label: 'Category' },
        { name: 'location', label: 'Location' },
        { name: 'purchaseValue', label: 'Purchase value', type: 'number' },
        { name: 'status', label: 'Status', options: ['Active', 'Under Maintenance', 'Disposed'], defaultValue: 'Active' },
      ]}
    />
  );
}

export function StandardCostPage() {
  return (
    <ResourcePage
      title="Standard Cost"
      subtitle="Review and update item standard costs."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Standard Cost' }]}
      endpoints={['/items', '/masters/items', '/masters/standard-cost']}
      allowCreate={false}
      allowEdit
      updateEndpoint={(row) => `/items/${row.id || row.itemCode}`}
      columns={[
        { field: 'itemCode', header: 'Item' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'uom', header: 'UOM' },
        { field: 'standardCost', header: 'Standard cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Latest purchase', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code' },
        { name: 'standardCost', label: 'Standard cost', type: 'number', required: true },
      ]}
    />
  );
}
