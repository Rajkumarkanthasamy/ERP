import ResourcePage from '../../components/ResourcePage';

export function EnquiryRegisterPage() {
  return (
    <ResourcePage
      title="Enquiry Register"
      subtitle="Capture and track customer enquiries."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Sales' }, { label: 'Enquiries' }]}
      endpoints={['/enquiries', '/sales/enquiries']}
      createEndpoint="/enquiries"
      createLabel="New enquiry"
      allowEdit
      columns={[
        { field: 'enquiryNo', header: 'Enquiry', getValue: (r) => r.enquiryNo || r.docNo || r.id },
        { field: 'customerName', header: 'Customer', getValue: (r) => r.customerName || r.customerCode },
        { field: 'subject', header: 'Subject', getValue: (r) => r.subject || r.title },
        { field: 'source', header: 'Source' },
        { field: 'enquiryDate', header: 'Date', type: 'date', getValue: (r) => r.enquiryDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'subject', label: 'Subject', required: true },
        { name: 'source', label: 'Source', options: ['Email', 'Phone', 'Walk-in', 'Partner', 'Web'], defaultValue: 'Email' },
        { name: 'status', label: 'Status', options: ['Open', 'Qualified', 'Quoted', 'Won', 'Lost'], defaultValue: 'Open' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}

export function OpportunitiesPage() {
  return (
    <ResourcePage
      title="Opportunities"
      subtitle="Sales pipeline opportunities linked to enquiries."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Sales' }, { label: 'Opportunities' }]}
      endpoints={['/opportunities', '/sales/opportunities']}
      createEndpoint="/opportunities"
      createLabel="New opportunity"
      allowEdit
      columns={[
        { field: 'opportunityNo', header: 'Opp No', getValue: (r) => r.opportunityNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'title', header: 'Title', getValue: (r) => r.title || r.name },
        { field: 'value', header: 'Value', type: 'money', getValue: (r) => r.value || r.amount },
        { field: 'stage', header: 'Stage', type: 'status', getValue: (r) => r.stage || r.status },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'title', label: 'Title', required: true },
        { name: 'value', label: 'Value (INR)', type: 'number', defaultValue: 0 },
        { name: 'stage', label: 'Stage', options: ['Prospect', 'Qualified', 'Proposal', 'Negotiation', 'Won', 'Lost'], defaultValue: 'Prospect' },
      ]}
    />
  );
}

export function QuotesPage() {
  return (
    <ResourcePage
      title="Quotes"
      subtitle="Prepare and track commercial quotations."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Sales' }, { label: 'Quotes' }]}
      endpoints={['/quotes', '/sales/quotes']}
      createEndpoint="/quotes"
      createLabel="New quote"
      allowEdit
      columns={[
        { field: 'quoteNo', header: 'Quote', getValue: (r) => r.quoteNo || r.docNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'projectCode', header: 'Project' },
        { field: 'amount', header: 'Amount', type: 'money', getValue: (r) => r.amount || r.totalAmount },
        { field: 'quoteDate', header: 'Date', type: 'date', getValue: (r) => r.quoteDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'amount', label: 'Amount', type: 'number', required: true },
        { name: 'status', label: 'Status', options: ['Draft', 'Sent', 'Accepted', 'Rejected'], defaultValue: 'Draft' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
