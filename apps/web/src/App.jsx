import { Navigate, Route, Routes } from 'react-router-dom';
import { CircularProgress, Box } from '@mui/material';
import { useAuth } from './auth/AuthContext';
import LoginPage from './auth/LoginPage';
import AppLayout from './layout/AppLayout';
import DashboardPage from './pages/DashboardPage';
import ProcurementDashboardPage from './pages/procurement/ProcurementDashboardPage';
import KanbanPage from './pages/procurement/KanbanPage';
import PrGenerationPage from './pages/procurement/PrGenerationPage';
import PrApprovalPage from './pages/procurement/PrApprovalPage';
import PrClubbingPage from './pages/procurement/PrClubbingPage';
import PrToPoWorkspacePage from './pages/procurement/PrToPoWorkspacePage';
import PoApprovalPage from './pages/procurement/PoApprovalPage';
import PoStatusViewPage from './pages/procurement/PoStatusViewPage';
import GrnPage from './pages/procurement/GrnPage';
import PriceVariancePage from './pages/procurement/PriceVariancePage';
import ItemCodePage from './pages/procurement/ItemCodePage';
import WorkOrdersPage from './pages/procurement/WorkOrdersPage';
import {
  AssetsPage,
  CitiesPage,
  CustomersPage,
  ItemsPage,
  SalesProductsPage,
  StandardCostPage,
  VendorsPage,
} from './pages/masters/MastersPages';
import {
  GinReceiptPage,
  ItemIssuePage,
  ItemProductionPage,
  ItemReturnPage,
  StockAdjustPage,
  StockLedgerPage,
} from './pages/stores/StoresPages';
import { GateInwardPage, GateOutwardPage, ManualInwardPage } from './pages/gate/GatePages';
import {
  BomApprovePage,
  IndentPage,
  InstallationStatusPage,
  ProjectDocumentsPage,
  ProjectListPage,
} from './pages/projects/ProjectsPages';
import { EnquiryRegisterPage, OpportunitiesPage, QuotesPage } from './pages/sales/SalesPages';
import ServiceCallsPage from './pages/service/ServiceCallsPage';
import { EscalationsPage, NcPage } from './pages/quality/QualityPages';
import TimesheetPage from './pages/TimesheetPage';
import DeliveryChallanPage from './pages/DeliveryChallanPage';
import ComplaintsPage from './pages/ComplaintsPage';
import { ChangePasswordPage, UsersPage } from './pages/users/UsersPages';
import ReportsPage from './pages/ReportsPage';
import ThemeSettingsPage from './pages/ThemeSettingsPage';

function Protected({ children }) {
  const { isAuthenticated, booting } = useAuth();
  if (booting) {
    return (
      <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return children;
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <Protected>
            <AppLayout />
          </Protected>
        }
      >
        <Route index element={<DashboardPage />} />
        <Route path="procurement" element={<ProcurementDashboardPage />} />
        <Route path="procurement/kanban" element={<KanbanPage />} />
        <Route path="procurement/pr-generation" element={<PrGenerationPage />} />
        <Route path="procurement/pr-approval" element={<PrApprovalPage />} />
        <Route path="procurement/pr-clubbing" element={<PrClubbingPage />} />
        <Route path="procurement/pr-to-po" element={<PrToPoWorkspacePage />} />
        <Route path="procurement/po-approval" element={<PoApprovalPage />} />
        <Route path="procurement/po-status" element={<PoStatusViewPage />} />
        <Route path="procurement/grn" element={<GrnPage />} />
        <Route path="procurement/price-variance" element={<PriceVariancePage />} />
        <Route path="procurement/item-codes" element={<ItemCodePage mode="create" />} />
        <Route path="procurement/item-code-approval" element={<ItemCodePage mode="approval" />} />
        <Route path="procurement/work-orders" element={<WorkOrdersPage />} />

        <Route path="masters/cities" element={<CitiesPage />} />
        <Route path="masters/customers" element={<CustomersPage />} />
        <Route path="masters/vendors" element={<VendorsPage />} />
        <Route path="masters/items" element={<ItemsPage />} />
        <Route path="masters/assets" element={<AssetsPage />} />
        <Route path="masters/standard-cost" element={<StandardCostPage />} />
        <Route path="masters/sales-products" element={<SalesProductsPage />} />

        <Route path="stores/stock-ledger" element={<StockLedgerPage />} />
        <Route path="stores/gin" element={<GinReceiptPage />} />
        <Route path="stores/issue" element={<ItemIssuePage />} />
        <Route path="stores/return" element={<ItemReturnPage />} />
        <Route path="stores/adjust" element={<StockAdjustPage />} />
        <Route path="stores/production" element={<ItemProductionPage />} />

        <Route path="gate/inward" element={<GateInwardPage />} />
        <Route path="gate/outward" element={<GateOutwardPage />} />
        <Route path="gate/manual-inward" element={<ManualInwardPage />} />

        <Route path="projects" element={<ProjectListPage />} />
        <Route path="projects/indents" element={<IndentPage />} />
        <Route path="projects/bom-approve" element={<BomApprovePage />} />
        <Route path="projects/installation" element={<InstallationStatusPage />} />
        <Route path="projects/documents" element={<ProjectDocumentsPage />} />

        <Route path="sales/enquiries" element={<EnquiryRegisterPage />} />
        <Route path="sales/opportunities" element={<OpportunitiesPage />} />
        <Route path="sales/quotes" element={<QuotesPage />} />

        <Route path="service/calls" element={<ServiceCallsPage />} />
        <Route path="quality/nc" element={<NcPage />} />
        <Route path="quality/escalations" element={<EscalationsPage />} />
        <Route path="timesheets" element={<TimesheetPage />} />
        <Route path="delivery-challans" element={<DeliveryChallanPage />} />
        <Route path="complaints" element={<ComplaintsPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="users/change-password" element={<ChangePasswordPage />} />
        <Route path="reports" element={<ReportsPage />} />
        <Route path="settings/theme" element={<ThemeSettingsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
