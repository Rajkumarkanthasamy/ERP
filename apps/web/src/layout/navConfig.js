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
import PaletteOutlinedIcon from '@mui/icons-material/PaletteOutlined';

export const BISS_LOGO = 'https://www.biss.in/img/logo-036.png';

export const navSections = [
  {
    id: 'home',
    label: 'Home',
    icon: DashboardOutlinedIcon,
    items: [{ label: 'Dashboard', path: '/' }],
  },
  {
    id: 'appearance',
    label: 'Appearance',
    icon: PaletteOutlinedIcon,
    items: [{ label: 'Theme & Typography', path: '/settings/theme' }],
  },
  {
    id: 'procurement',
    label: 'Procurement',
    icon: ShoppingCartOutlinedIcon,
    items: [
      { label: 'Procurement Dashboard', path: '/procurement', permission: 'purchaseOrder' },
      { label: 'Kanban', path: '/procurement/kanban', permission: ['purchaseOrder', 'kanbanItems'] },
      { label: 'PR Generation', path: '/procurement/pr-generation', permission: 'purchaseOrder' },
      { label: 'PR Approval', path: '/procurement/pr-approval', permission: ['purchaseManager', 'manufacturingHead', 'generalManager', 'operationManager'] },
      { label: 'PR Clubbing', path: '/procurement/pr-clubbing', permission: 'purchaseOrder' },
      { label: 'PR → PO Workspace', path: '/procurement/pr-to-po', permission: ['purchaseOrder', 'poWoGenerate'] },
      { label: 'PO Approval', path: '/procurement/po-approval', permission: ['purchaseManager', 'manufacturingHead', 'generalManager', 'operationManager', 'financeManager'] },
      { label: 'PO Status View', path: '/procurement/po-status', permission: ['purchaseOrder', 'poTrack'] },
      { label: 'GRN', path: '/procurement/grn', permission: 'receipt' },
      { label: 'Price Variance', path: '/procurement/price-variance', permission: ['purchaseOrder', 'standardCostUpdate'] },
      { label: 'Item Code Creation', path: '/procurement/item-codes', permission: 'partMaster' },
      { label: 'Item Code Approval', path: '/procurement/item-code-approval', permission: ['partMaster', 'purchaseManager'] },
      { label: 'Work Orders', path: '/procurement/work-orders', permission: 'workOrder' },
    ],
  },
  {
    id: 'masters',
    label: 'ERP Masters',
    icon: Inventory2OutlinedIcon,
    items: [
      { label: 'Cities', path: '/masters/cities', permission: 'cityMaster' },
      { label: 'Customers', path: '/masters/customers', permission: 'customerMaster' },
      { label: 'Vendors', path: '/masters/vendors', permission: 'vendorMaster' },
      { label: 'Items', path: '/masters/items', permission: 'partMaster' },
      { label: 'Assets', path: '/masters/assets', permission: 'assetMaster' },
      { label: 'Standard Cost', path: '/masters/standard-cost', permission: 'standardCostUpdate' },
      { label: 'Target Cost', path: '/masters/target-cost', permission: 'standardCostUpdate' },
      { label: 'Additional Masters', path: '/masters/additional', permission: ['additionalMaster', 'purchaseOrder', 'pegRate'] },
      { label: 'Sales Product Master', path: '/masters/sales-products', permission: 'salesProduct' },
    ],
  },
  {
    id: 'stores',
    label: 'Stores / Inventory',
    icon: WarehouseOutlinedIcon,
    items: [
      { label: 'Stock Ledger', path: '/stores/stock-ledger', permission: 'materialLedger' },
      { label: 'GIN Receipt', path: '/stores/gin', permission: 'receipt' },
      { label: 'Item Issue', path: '/stores/issue', permission: 'issue' },
      { label: 'Item Return', path: '/stores/return', permission: ['receipt', 'issue'] },
      { label: 'Stock Adjust', path: '/stores/adjust', permission: 'kanbanStockAdjust' },
      { label: 'Item Production', path: '/stores/production', permission: 'kanbanItems' },
    ],
  },
  {
    id: 'gate',
    label: 'Gate Entry',
    icon: GateIcon,
    items: [
      { label: 'Inward', path: '/gate/inward', permission: 'securityCheck' },
      { label: 'Outward', path: '/gate/outward', permission: 'securityCheck' },
      { label: 'Manual Inward', path: '/gate/manual-inward', permission: 'securityCheck' },
    ],
  },
  {
    id: 'projects',
    label: 'Projects',
    icon: AccountTreeOutlinedIcon,
    items: [
      { label: 'Indent', path: '/projects/indents', permission: 'indent' },
      { label: 'Project List / Create', path: '/projects', permission: ['projectMaster', 'createProject'] },
      { label: 'BOM Approve', path: '/projects/bom-approve', permission: ['projectBom', 'bomAuthorise'] },
      { label: 'Installation Status', path: '/projects/installation', permission: 'projectInstallStatus' },
      { label: 'Documents', path: '/projects/documents', permission: 'projectDocuments' },
    ],
  },
  {
    id: 'sales',
    label: 'Sales',
    icon: TrendingUpOutlinedIcon,
    items: [
      { label: 'Enquiry Register', path: '/sales/enquiries', permission: 'enquiryRegister' },
      { label: 'Opportunities', path: '/sales/opportunities', permission: 'opportunityDetails' },
      { label: 'Quotes', path: '/sales/quotes', permission: ['salesQuote', 'quotation'] },
    ],
  },
  {
    id: 'service',
    label: 'Service',
    icon: BuildOutlinedIcon,
    items: [{ label: 'Service Calls', path: '/service/calls', permission: ['serviceQuote', 'projectMaster'] }],
  },
  {
    id: 'quality',
    label: 'Quality',
    icon: VerifiedOutlinedIcon,
    items: [
      { label: 'NC', path: '/quality/nc', permission: 'qualityManagement' },
      { label: 'Escalations', path: '/quality/escalations', permission: 'qualityManagement' },
    ],
  },
  {
    id: 'timesheet',
    label: 'Time Sheet',
    icon: AccessTimeOutlinedIcon,
    items: [{ label: 'Time Sheet', path: '/timesheets', permission: 'timesheet' }],
  },
  {
    id: 'delivery',
    label: 'Delivery Challan',
    icon: LocalShippingOutlinedIcon,
    items: [{ label: 'Delivery Challan', path: '/delivery-challans', permission: 'receipt' }],
  },
  {
    id: 'complaints',
    label: 'Complaints / Support',
    icon: SupportAgentOutlinedIcon,
    items: [{ label: 'Complaints', path: '/complaints', permission: 'complaint' }],
  },
  {
    id: 'users',
    label: 'Users',
    icon: PeopleOutlinedIcon,
    items: [
      { label: 'Users', path: '/users', permission: 'addUser' },
      { label: 'Change Password', path: '/users/change-password' },
    ],
  },
  {
    id: 'reports',
    label: 'Reports',
    icon: AssessmentOutlinedIcon,
    items: [{ label: 'Reports Summary', path: '/reports', permission: 'reports' }],
  },
];

export function canAccessNavItem(item, permissions) {
  if (!item.permission) return true;
  // SQLite/demo users predate the legacy permission map and retain full demo
  // navigation. MSSQL users receive the map from the Login table.
  if (!permissions || Object.keys(permissions).length === 0) return true;
  const required = Array.isArray(item.permission) ? item.permission : [item.permission];
  return required.some((key) => Boolean(permissions[key]));
}

export function flattenNav(permissions) {
  return navSections.flatMap((section) =>
    section.items
      .filter((item) => canAccessNavItem(item, permissions))
      .map((item) => ({
        ...item,
        section: section.label,
        sectionId: section.id,
      }))
  );
}
