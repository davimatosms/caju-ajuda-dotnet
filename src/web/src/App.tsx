import React from 'react';
import { BrowserRouter as Router, Route, Routes, Outlet } from 'react-router-dom';

// Componentes de Proteção de Rotas
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute';
import PublicRoute from './components/PublicRoute/PublicRoute';

// Layouts
import Layout from './components/Layout/Layout';
import PublicNavbar from './components/PublicNavbar/PublicNavbar';
import Footer from './components/Footer/Footer';
import AdminHomePage from './pages/Admin/AdminHomePage/AdminHomePage';

// Páginas Públicas
import LoginPage from './pages/LoginPage/LoginPage';
import RegisterPage from './pages/RegisterPage/Register';
import LandingPage from './pages/LandingPage/LandingPage';
import NotFoundPage from './pages/NotFoundPage/NotFoundPage';

// Páginas do Cliente (Privadas)
import DashboardPage from './pages/DashboardPage/DashboardPage';
import NovoChamadoPage from './pages/NovoChamadoPage/NovoChamadoPage';
import ChamadoDetailPage from './pages/ChamadoDetailPage/ChamadoDetailPage';
import MeuPerfilPage from './pages/MeuPerfilPage/MeuPerfilPage';

// Páginas do Admin (Privadas)

import DashboardAdminPage from './pages/Admin/DashboardAdminPage/DashboardAdminPage';
import GerenciarTecnicosPage from './pages/Admin/GerenciarTecnicosPage/GerenciarTecnicosPage';
import GerenciarClientesPage from './pages/Admin/GerenciarClientesPage/GerenciarClientesPage';


// Componente de Layout para a Landing Page (com Navbar e Footer)
const PublicLayout = () => (
  <div className="flex flex-col min-h-screen">
    <PublicNavbar />
    <main className="flex-grow">
      <Outlet />
    </main>
    <Footer />
  </div>
);

function App() {
  return (
    <Router>
      <Routes>
        {/* Rotas Públicas */}
        <Route element={<PublicLayout />}>
          <Route path="/" element={<LandingPage />} />
        </Route>

        {/* Rotas de Login/Registro - Redireciona se já estiver autenticado */}
        <Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} />
        <Route path="/register" element={<PublicRoute><RegisterPage /></PublicRoute>} />
        
        {/* Rotas Privadas - Requer Autenticação (CLIENTE, TECNICO ou ADMIN) */}
        <Route element={<ProtectedRoute allowedRoles={['CLIENTE', 'TECNICO', 'ADMIN']} />}>
          <Route element={<Layout />}>
            {/* Rotas do Cliente */}
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/chamados/novo" element={<NovoChamadoPage />} />
            <Route path="/chamado/:id" element={<ChamadoDetailPage />} />
            <Route path="/perfil" element={<MeuPerfilPage />} />
          </Route>
        </Route>

        {/* Rotas Administrativas - Requer ADMIN ou TECNICO */}
        <Route element={<ProtectedRoute allowedRoles={['ADMIN', 'TECNICO']} />}>
          <Route element={<Layout />}>
            <Route path="/admin" element={<AdminHomePage />} />
            <Route path="/admin/dashboard" element={<DashboardAdminPage />} />
            <Route path="/admin/tecnicos" element={<GerenciarTecnicosPage />} />
            <Route path="/admin/clientes" element={<GerenciarClientesPage />} />
          </Route>
        </Route>

        {/* Rota 404 - Página não encontrada */}
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </Router>
  );
}

export default App;