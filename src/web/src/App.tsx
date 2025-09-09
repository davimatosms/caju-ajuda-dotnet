import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import RegisterPage from './pages/RegisterPage/Register';
import LoginPage from './pages/LoginPage/LoginPage';
import DashboardPage from './pages/DashboardPage/DashboardPage';
import NovoChamadoPage from './pages/NovoChamadoPage/NovoChamadoPage'; 

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        
        <Route path="/" element={<DashboardPage />} />
        <Route path="/chamados/novo" element={<NovoChamadoPage />} /> {/* Adicionamos a nova rota */}
      </Routes>
    </Router>
  );
}

export default App;