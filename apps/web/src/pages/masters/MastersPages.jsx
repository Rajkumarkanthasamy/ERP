import ResourcePage from '../../components/ResourcePage';

export function CitiesPage() {
  return (
    <ResourcePage
      title="Cities"
      subtitle="Maintain city master used across customers, vendors and projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Cities' }]}
      endpoints={['/masters/cities']}
      createEndpoint="/masters/cities"
      createLabel="Add city"
      allowEdit
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
      endpoints={['/masters/customers']}
      createEndpoint="/masters/customers"
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
      endpoints={['/masters/vendors']}
      createEndpoint="/masters/vendors"
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
      endpoints={['/masters/items']}
      createEndpoint="/masters/items"
      createLabel="Add item"
      allowEdit
      columns={[
        { field: 'itemCode', header: 'Code' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'make', header: 'Make' },
        { field: 'uom', header: 'UOM' },
        { field: 'standardCost', header: 'Std cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Last PO', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'itemDescription', label: 'Description', required: true },
        { name: 'specification', label: 'Specification' },
        { name: 'make', label: 'Make' },
        { name: 'mfgPartNo', label: 'Mfg part no' },
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
      title="Fixed Assets"
      subtitle="Add and update fixed asset register."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Assets' }]}
      endpoints={['/masters/assets']}
      createEndpoint="/masters/assets"
      createLabel="Add asset"
      allowEdit
      columns={[
        { field: 'assetCode', header: 'Asset', getValue: (r) => r.assetCode || r.assetSlNo },
        { field: 'assetDescription', header: 'Description', getValue: (r) => r.assetDescription || r.description },
        { field: 'location', header: 'Location' },
        { field: 'vendorName', header: 'Vendor' },
        { field: 'amount', header: 'Amount', type: 'money' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'assetCode', label: 'Asset code', required: true },
        { name: 'assetDescription', label: 'Description', required: true },
        { name: 'location', label: 'Location' },
        { name: 'vendorName', label: 'Vendor' },
        { name: 'amount', label: 'Amount', type: 'number', defaultValue: 0 },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function StandardCostPage() {
  return (
    <ResourcePage
      title="Standard Cost"
      subtitle="Review and maintain item standard costs."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Standard Cost' }]}
      endpoints={['/masters/items']}
      allowCreate={false}
      allowEdit
      createEndpoint="/masters/items"
      columns={[
        { field: 'itemCode', header: 'Item' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'uom', header: 'UOM' },
        { field: 'standardCost', header: 'Std cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Last purchase', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true },
        { name: 'standardCost', label: 'Standard cost', type: 'number', required: true },
      ]}
    />
  );
}

export function SalesProductsPage() {
  return (
    <ResourcePage
      title="Sales Product Master"
      subtitle="Finished goods / sales products for quotes and projects."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Masters' }, { label: 'Sales Products' }]}
      endpoints={['/masters/sales-products']}
      createEndpoint="/masters/sales-products"
      createLabel="Add product"
      allowEdit
      columns={[
        { field: 'productCode', header: 'Code' },
        { field: 'productName', header: 'Name' },
        { field: 'productType', header: 'Type' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'productCode', label: 'Product code', required: true },
        { name: 'productName', label: 'Product name', required: true },
        { name: 'productType', label: 'Type', defaultValue: 'Machine' },
        { name: 'status', label: 'Status', options: ['Active', 'Inactive'], defaultValue: 'Active' },
      ]}
    />
  );
}
