import ResourcePage from '../../components/ResourcePage';
import WorkflowActionButtons from '../../components/WorkflowActionButtons';
import { useAuth } from '../../auth/AuthContext';

export function EnquiryRegisterPage() {
  return (
    <ResourcePage
      title="Enquiry Register"
      subtitle="Capture and track customer enquiries."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Sales' }, { label: 'Enquiries' }]}
      endpoints={['/sales/enquiries']}
      createEndpoint="/sales/enquiries"
      createLabel="New enquiry"
      allowEdit
      rowKey="enquiryNumber"
      columns={[
        { field: 'enquiryNumber', header: 'Enquiry', getValue: (r) => r.enquiryNumber || r.enquiryNo || r.id },
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
      endpoints={['/sales/opportunities']}
      createEndpoint="/sales/opportunities"
      createLabel="New opportunity"
      allowEdit
      rowKey="opportunityNumber"
      columns={[
        { field: 'opportunityNumber', header: 'Opp No', getValue: (r) => r.opportunityNumber || r.opportunityNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'title', header: 'Title', getValue: (r) => r.title || r.name },
        { field: 'expectedValue', header: 'Value', type: 'money', getValue: (r) => r.expectedValue || r.value || r.amount },
        { field: 'stage', header: 'Stage', type: 'status', getValue: (r) => r.stage || r.status },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'title', label: 'Title', required: true },
        { name: 'expectedValue', label: 'Value (INR)', type: 'number', defaultValue: 0 },
        { name: 'stage', label: 'Stage', options: ['Prospect', 'Qualified', 'Proposal', 'Negotiation', 'Won', 'Lost'], defaultValue: 'Prospect' },
      ]}
    />
  );
}

export function QuotesPage() {
  const { user } = useAuth();

  return (
    <ResourcePage
      title="Quotes"
      subtitle="Prepare and track commercial quotations."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Sales' }, { label: 'Quotes' }]}
      endpoints={['/sales/quotes']}
      createEndpoint="/sales/quotes"
      createLabel="New quote"
      allowEdit
      rowKey="quoteNumber"
      columns={[
        { field: 'quoteNumber', header: 'Quote', getValue: (r) => r.quoteNumber || r.quoteNo || r.docNo || r.id },
        { field: 'customerName', header: 'Customer' },
        { field: 'opportunityNumber', header: 'Opportunity' },
        { field: 'totalAmount', header: 'Amount', type: 'money', getValue: (r) => r.totalAmount || r.amount },
        { field: 'quoteDate', header: 'Date', type: 'date', getValue: (r) => r.quoteDate || r.createdAt },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'customerName', label: 'Customer', required: true },
        { name: 'opportunityNumber', label: 'Opportunity number' },
        { name: 'validUntil', label: 'Valid until', type: 'date' },
        { name: 'totalAmount', label: 'Amount', type: 'number', required: true },
        { name: 'currency', label: 'Currency', defaultValue: 'INR' },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) => {
        const actions =
          row.status === 'Draft'
            ? [{ label: 'Submit', value: 'submit', variant: 'outlined' }]
            : row.status === 'Submitted' && user?.canApprovePR
              ? [
                  { label: 'Approve', value: 'approve', variant: 'outlined' },
                  { label: 'Reject', value: 'reject', color: 'error' },
                ]
              : row.status === 'Approved'
                ? [
                    { label: 'Won', value: 'win', variant: 'outlined' },
                    { label: 'Lost', value: 'lose', color: 'error' },
                  ]
                : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/sales/quotes/${encodeURIComponent(row.quoteNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
