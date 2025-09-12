import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import RegisterPage from './pages/RegisterPage/Register';
import LoginPage from './pages/LoginPage/LoginPage';
import DashboardPage from './pages/DashboardPage/DashboardPage';
import NovoChamadoPage from './pages/NovoChamadoPage/NovoChamadoPage';
import ChamadoDetailPage from './pages/ChamadoDetailPage/ChamadoDetailPage';
import Layout from './components/Layout/Layout';
import GerenciarTecnicosPage from './pages/Admin/GerenciarTecnicosPage/GerenciarTecnicosPage';
import GerenciarClientesPage from './pages/Admin/GerenciarClientesPage/GerenciarClientesPage';

function App() {
  return (
    <Router>
      <Routes>
        {/* Rotas públicas */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        
        {/* Rotas privadas */}
        <Route element={<Layout />}>
          {/* Rotas do Cliente */}
          <Route path="/" element={<DashboardPage />} />
          <Route path="/chamados/novo" element={<NovoChamadoPage />} />
          <Route path="/chamado/:id" element={<ChamadoDetailPage />} />

          {/* Rotas do Admin */}
          <Route path="/admin/tecnicos" element={<GerenciarTecnicosPage />} />
          <Route path="/admin/clientes" element={<GerenciarClientesPage />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;