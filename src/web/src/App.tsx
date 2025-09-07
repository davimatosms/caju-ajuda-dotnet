import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

function App() {
  return (
    <Router>
      <Routes>
        {/* Adicionaremos as nossas rotas aqui */}
        <Route path="/login" element={<div>Página de Login</div>} />
        <Route path="/register" element={<div>Página de Registo</div>} />
        <Route path="/" element={<div>Dashboard</div>} />
      </Routes>
    </Router>
  );
}

export default App;