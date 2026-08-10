import ResourcePage from '../components/ResourcePage';

export default function TimesheetPage() {
  return (
    <ResourcePage
      title="Time Sheet"
      subtitle="Log project / service hours by employee."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Time Sheet' }]}
      endpoints={['/timesheets', '/time-sheets']}
      createEndpoint="/timesheets"
      createLabel="Add entry"
      allowEdit
      columns={[
        { field: 'entryDate', header: 'Date', type: 'date', getValue: (r) => r.entryDate || r.date || r.workDate },
        { field: 'employee', header: 'Employee', getValue: (r) => r.employee || r.userName || r.username },
        { field: 'projectCode', header: 'Project' },
        { field: 'hours', header: 'Hours', getValue: (r) => r.hours ?? r.duration },
        { field: 'activity', header: 'Activity', getValue: (r) => r.activity || r.task },
        { field: 'status', header: 'Status', type: 'status' },
      ]}
      fields={[
        { name: 'entryDate', label: 'Date', type: 'date', required: true },
        { name: 'projectCode', label: 'Project code' },
        { name: 'hours', label: 'Hours', type: 'number', required: true, defaultValue: 8 },
        { name: 'activity', label: 'Activity', required: true },
        { name: 'remarks', label: 'Remarks', multiline: true },
      ]}
    />
  );
}
