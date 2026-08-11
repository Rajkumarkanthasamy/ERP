import ResourcePage from '../components/ResourcePage';
import WorkflowActionButtons from '../components/WorkflowActionButtons';
import { useAuth } from '../auth/AuthContext';

export default function TimesheetPage() {
  const { user } = useAuth();

  return (
    <ResourcePage
      title="Time Sheet"
      subtitle="Log project / service hours by employee."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Time Sheet' }]}
      endpoints={['/timesheets']}
      createEndpoint="/timesheets"
      createLabel="Add entry"
      allowEdit
      rowKey="entryNumber"
      columns={[
        { field: 'workDate', header: 'Date', type: 'date', getValue: (r) => r.workDate || r.entryDate || r.date },
        { field: 'userName', header: 'Employee', getValue: (r) => r.userName || r.employee || r.username },
        { field: 'projectCode', header: 'Project' },
        { field: 'hours', header: 'Hours' },
        { field: 'activity', header: 'Activity' },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'workDate', label: 'Date', type: 'date', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'hours', label: 'Hours', type: 'number', required: true, defaultValue: 8 },
        { name: 'activity', label: 'Activity', required: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
      rowActions={(row, reload) => {
        const actions = ['Draft', 'Rejected'].includes(row.status)
          ? [{ label: 'Submit', value: 'submit', variant: 'outlined' }]
          : row.status === 'Submitted' && user?.canApprovePR
            ? [
                { label: 'Approve', value: 'approve', variant: 'outlined' },
                {
                  label: 'Reject',
                  value: 'reject',
                  color: 'error',
                  prompt: {
                    label: 'Enter a rejection reason',
                    field: 'reason',
                    fieldLabel: 'Reason',
                    required: true,
                  },
                },
              ]
            : [];
        return actions.length ? (
          <WorkflowActionButtons
            endpoint={`/timesheets/${encodeURIComponent(row.entryNumber)}/status`}
            actions={actions}
            onComplete={reload}
          />
        ) : null;
      }}
    />
  );
}
