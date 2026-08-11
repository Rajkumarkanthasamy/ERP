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
      rowKey="cityCode"
      columns={[
        { field: 'cityCode', header: 'Code / ID', getValue: (r) => r.cityCode || r.code },
        { field: 'cityName', header: 'City', getValue: (r) => r.cityName || r.name },
        { field: 'state', header: 'State' },
        { field: 'stateCode', header: 'State ID' },
        { field: 'country', header: 'Country', getValue: (r) => r.country || 'India' },
        { field: 'active', header: 'Active', render: (v) => (v === 0 || v === false ? 'No' : 'Yes') },
      ]}
      fields={[
        {
          name: 'cityCode',
          label: 'City code / ID',
          disabledOnEdit: true,
          helperText: 'Required in demo mode. Live SQL Server assigns CityMaster identity.',
        },
        { name: 'cityName', label: 'City name', required: true },
        {
          name: 'state',
          label: 'State name',
          helperText: 'Provide state name and/or StateMaster id. Live mode resolves against StateMaster.',
        },
        { name: 'stateCode', label: 'State ID (optional override)' },
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
      rowKey="customerCode"
      columns={[
        { field: 'customerCode', header: 'Code', getValue: (r) => r.customerCode || r.code },
        { field: 'customerName', header: 'Name', getValue: (r) => r.customerName || r.name },
        { field: 'city', header: 'City' },
        { field: 'gstin', header: 'GSTIN' },
        { field: 'phone', header: 'Phone', getValue: (r) => r.phone || r.mobile },
        { field: 'status', header: 'Status', type: 'status', getValue: (r) => r.status || (r.active === 0 ? 'Inactive' : 'Active') },
      ]}
      fields={[
        { name: 'customerCode', label: 'Customer code', required: true, disabledOnEdit: true },
        { name: 'customerName', label: 'Customer name', required: true },
        { name: 'city', label: 'City' },
        { name: 'state', label: 'State' },
        { name: 'gstin', label: 'GSTIN' },
        { name: 'contactPerson', label: 'Contact person' },
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
      rowKey="vendorCode"
      columns={[
        { field: 'vendorCode', header: 'Code' },
        { field: 'vendorName', header: 'Name' },
        { field: 'city', header: 'City' },
        { field: 'gstin', header: 'GSTIN' },
        { field: 'phone', header: 'Phone' },
        { field: 'active', header: 'Active', render: (v) => (v === 0 || v === false ? 'No' : 'Yes') },
      ]}
      fields={[
        { name: 'vendorCode', label: 'Vendor code', required: true, disabledOnEdit: true },
        { name: 'vendorName', label: 'Vendor name', required: true },
        { name: 'city', label: 'City' },
        { name: 'state', label: 'State' },
        { name: 'gstin', label: 'GSTIN' },
        { name: 'contactPerson', label: 'Contact person' },
        { name: 'phone', label: 'Phone' },
        { name: 'email', label: 'Email' },
        { name: 'address', label: 'Address', multiline: true },
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
      rowKey="itemCode"
      columns={[
        { field: 'itemCode', header: 'Code' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'make', header: 'Make' },
        { field: 'uom', header: 'UOM' },
        { field: 'standardCost', header: 'Std cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Last PO', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true, disabledOnEdit: true },
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
      rowKey="assetCode"
      columns={[
        { field: 'assetCode', header: 'Asset', getValue: (r) => r.assetCode || r.assetSlNo },
        { field: 'assetName', header: 'Description' },
        { field: 'category', header: 'Category' },
        { field: 'location', header: 'Location' },
        { field: 'purchaseValue', header: 'Purchase value', type: 'money' },
        { field: 'currentValue', header: 'Current value', type: 'money' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'assetCode', label: 'Asset code', required: true, disabledOnEdit: true },
        { name: 'assetName', label: 'Description', required: true },
        { name: 'category', label: 'Category' },
        { name: 'location', label: 'Location' },
        { name: 'purchaseDate', label: 'Purchase date', type: 'date' },
        { name: 'purchaseValue', label: 'Purchase value', type: 'number', defaultValue: 0 },
        { name: 'currentValue', label: 'Current value', type: 'number', defaultValue: 0 },
        { name: 'status', label: 'Status', options: ['Active', 'Disposed', 'Under Maintenance'], defaultValue: 'Active' },
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
      rowKey="itemCode"
      columns={[
        { field: 'itemCode', header: 'Item' },
        { field: 'itemDescription', header: 'Description' },
        { field: 'uom', header: 'UOM' },
        { field: 'standardCost', header: 'Std cost', type: 'money' },
        { field: 'latestPurchasePrice', header: 'Last purchase', type: 'money' },
      ]}
      fields={[
        { name: 'itemCode', label: 'Item code', required: true, disabledOnEdit: true },
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
      rowKey="productCode"
      columns={[
        { field: 'productCode', header: 'Code' },
        { field: 'productName', header: 'Name' },
        { field: 'uom', header: 'UOM' },
        { field: 'listPrice', header: 'List price', type: 'money' },
        { field: 'pegRate', header: 'PEG rate', type: 'money' },
        { field: 'isSparePart', header: 'Spare part', render: (v) => (v ? 'Yes' : 'No') },
      ]}
      fields={[
        { name: 'productCode', label: 'Product code', required: true, disabledOnEdit: true },
        { name: 'productName', label: 'Product name', required: true },
        { name: 'description', label: 'Description', multiline: true },
        { name: 'uom', label: 'UOM', defaultValue: 'NOS' },
        { name: 'listPrice', label: 'List price', type: 'number', defaultValue: 0 },
        { name: 'pegRate', label: 'PEG rate', type: 'number', defaultValue: 0 },
        { name: 'isSparePart', label: 'Spare part', type: 'checkbox', defaultValue: false },
      ]}
    />
  );
}
