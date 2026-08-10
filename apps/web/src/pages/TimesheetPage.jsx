import ResourcePage from '../components/ResourcePage';

export default function TimesheetPage() {
  return (
    <ResourcePage
      title="Time Sheet"
      subtitle="Log project / service hours by employee."
      crumbs={[{ label: 'Home', to: '/' }, { label: 'Time Sheet' }]}
      endpoints={['/timesheets']}
      createEndpoint="/timesheets"
      createLabel="Add entry"
      allowEdit
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
    />
  );
}
