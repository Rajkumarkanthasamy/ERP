import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import ShoppingCartOutlinedIcon from '@mui/icons-material/ShoppingCartOutlined';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import WarehouseOutlinedIcon from '@mui/icons-material/WarehouseOutlined';
import GateIcon from '@mui/icons-material/DoorFrontOutlined';
import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined';
import TrendingUpOutlinedIcon from '@mui/icons-material/TrendingUpOutlined';
import BuildOutlinedIcon from '@mui/icons-material/BuildOutlined';
import VerifiedOutlinedIcon from '@mui/icons-material/VerifiedOutlined';
import AccessTimeOutlinedIcon from '@mui/icons-material/AccessTimeOutlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import SupportAgentOutlinedIcon from '@mui/icons-material/SupportAgentOutlined';
import PeopleOutlinedIcon from '@mui/icons-material/PeopleOutlined';
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';

export const BISS_LOGO = 'https://www.biss.in/img/logo-036.png';

export const navSections = [
  {
    id: 'home',
    label: 'Home',
    icon: DashboardOutlinedIcon,
    items: [{ label: 'Dashboard', path: '/' }],
  },
  {
    id: 'procurement',
    label: 'Procurement',
    icon: ShoppingCartOutlinedIcon,
    items: [
      { label: 'Procurement Dashboard', path: '/procurement' },
      { label: 'Kanban', path: '/procurement/kanban' },
      { label: 'PR Generation', path: '/procurement/pr-generation' },
      { label: 'PR Approval', path: '/procurement/pr-approval' },
      { label: 'PR Clubbing', path: '/procurement/pr-clubbing' },
      { label: 'PR → PO Workspace', path: '/procurement/pr-to-po' },
      { label: 'PO Approval', path: '/procurement/po-approval' },
      { label: 'PO Status View', path: '/procurement/po-status' },
      { label: 'GRN', path: '/procurement/grn' },
      { label: 'Price Variance', path: '/procurement/price-variance' },
      { label: 'Item Code Creation', path: '/procurement/item-codes' },
      { label: 'Item Code Approval', path: '/procurement/item-code-approval' },
      { label: 'Work Orders', path: '/procurement/work-orders' },
    ],
  },
  {
    id: 'masters',
    label: 'ERP Masters',
    icon: Inventory2OutlinedIcon,
    items: [
      { label: 'Cities', path: '/masters/cities' },
      { label: 'Customers', path: '/masters/customers' },
      { label: 'Vendors', path: '/masters/vendors' },
      { label: 'Items', path: '/masters/items' },
      { label: 'Assets', path: '/masters/assets' },
      { label: 'Standard Cost', path: '/masters/standard-cost' },
      { label: 'Sales Product Master', path: '/masters/sales-products' },
    ],
  },
  {
    id: 'stores',
    label: 'Stores / Inventory',
    icon: WarehouseOutlinedIcon,
    items: [
      { label: 'Stock Ledger', path: '/stores/stock-ledger' },
      { label: 'GIN Receipt', path: '/stores/gin' },
      { label: 'Item Issue', path: '/stores/issue' },
      { label: 'Item Return', path: '/stores/return' },
      { label: 'Stock Adjust', path: '/stores/adjust' },
      { label: 'Item Production', path: '/stores/production' },
    ],
  },
  {
    id: 'gate',
    label: 'Gate Entry',
    icon: GateIcon,
    items: [
      { label: 'Inward', path: '/gate/inward' },
      { label: 'Outward', path: '/gate/outward' },
      { label: 'Manual Inward', path: '/gate/manual-inward' },
    ],
  },
  {
    id: 'projects',
    label: 'Projects',
    icon: AccountTreeOutlinedIcon,
    items: [
      { label: 'Indent', path: '/projects/indents' },
      { label: 'Project List / Create', path: '/projects' },
      { label: 'BOM Approve', path: '/projects/bom-approve' },
      { label: 'Installation Status', path: '/projects/installation' },
      { label: 'Documents', path: '/projects/documents' },
    ],
  },
  {
    id: 'sales',
    label: 'Sales',
    icon: TrendingUpOutlinedIcon,
    items: [
      { label: 'Enquiry Register', path: '/sales/enquiries' },
      { label: 'Opportunities', path: '/sales/opportunities' },
      { label: 'Quotes', path: '/sales/quotes' },
    ],
  },
  {
    id: 'service',
    label: 'Service',
    icon: BuildOutlinedIcon,
    items: [{ label: 'Service Calls', path: '/service/calls' }],
  },
  {
    id: 'quality',
    label: 'Quality',
    icon: VerifiedOutlinedIcon,
    items: [
      { label: 'NC', path: '/quality/nc' },
      { label: 'Escalations', path: '/quality/escalations' },
    ],
  },
  {
    id: 'timesheet',
    label: 'Time Sheet',
    icon: AccessTimeOutlinedIcon,
    items: [{ label: 'Time Sheet', path: '/timesheets' }],
  },
  {
    id: 'delivery',
    label: 'Delivery Challan',
    icon: LocalShippingOutlinedIcon,
    items: [{ label: 'Delivery Challan', path: '/delivery-challans' }],
  },
  {
    id: 'complaints',
    label: 'Complaints / Support',
    icon: SupportAgentOutlinedIcon,
    items: [{ label: 'Complaints', path: '/complaints' }],
  },
  {
    id: 'users',
    label: 'Users',
    icon: PeopleOutlinedIcon,
    items: [
      { label: 'Users', path: '/users' },
      { label: 'Change Password', path: '/users/change-password' },
    ],
  },
  {
    id: 'reports',
    label: 'Reports',
    icon: AssessmentOutlinedIcon,
    items: [{ label: 'Reports Summary', path: '/reports' }],
  },
];

export function flattenNav() {
  return navSections.flatMap((section) =>
    section.items.map((item) => ({
      ...item,
      section: section.label,
      sectionId: section.id,
    }))
  );
}
