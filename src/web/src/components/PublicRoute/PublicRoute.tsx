import React from 'react';
import { Navigate } from 'react-router-dom';
import AuthService from '../../services/AuthService';

interface PublicRouteProps {
  children: React.ReactElement;
}

/**
 * Componente para proteger rotas públicas (login/register)
 * Se o usuário já estiver autenticado, redireciona para o dashboard
 */
const PublicRoute: React.FC<PublicRouteProps> = ({ children }) => {
  const currentUser = AuthService.getCurrentUser();

  // Se já estiver autenticado, redireciona para dashboard apropriado
  if (currentUser) {
    if (currentUser.role === 'ADMIN' || currentUser.role === 'TECNICO') {
      return <Navigate to="/admin" replace />;
    }
    return <Navigate to="/dashboard" replace />;
  }

  // Se não estiver autenticado, permite acessar a página pública
  return children;
};

export default PublicRoute;
