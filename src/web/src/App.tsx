import React from 'react';
import { BrowserRouter as Router, Route, Routes, Outlet } from 'react-router-dom';

// Layouts
import Layout from './components/Layout/Layout';
import PublicNavbar from './components/PublicNavbar/PublicNavbar';
import Footer from './components/Footer/Footer';
import AuthLayout from './components/AuthLayout/AuthLayout';
import AdminHomePage from './pages/Admin/AdminHomePage/AdminHomePage';

// Páginas Públicas
import LoginPage from './pages/LoginPage/LoginPage';
import RegisterPage from './pages/RegisterPage/Register';
import LandingPage from './pages/LandingPage/LandingPage';

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
  <>
    <PublicNavbar />
    <Outlet />
    <Footer />
  </>
);

function App() {
  return (
    <Router>
      <Routes>
        {/* Rotas Públicas */}
        <Route element={<PublicLayout />}>
          <Route path="/" element={<LandingPage />} />
        </Route>

        {/* Rotas de Login/Registro usam o AuthLayout para centralizar */}
        <Route element={<AuthLayout />}>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
        </Route>
        
        {/* Rotas Privadas usam o Layout para usuários logados */}
        <Route element={<Layout />}>
          {/* Rotas do Cliente */}
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/chamados/novo" element={<NovoChamadoPage />} />
          <Route path="/chamado/:id" element={<ChamadoDetailPage />} />
          <Route path="/perfil" element={<MeuPerfilPage />} />

          {/* Rotas do Admin */}
          <Route path="/admin" element={<AdminHomePage />} />
          <Route path="/admin/dashboard" element={<DashboardAdminPage />} />
          <Route path="/admin/tecnicos" element={<GerenciarTecnicosPage />} />
          <Route path="/admin/clientes" element={<GerenciarClientesPage />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;