import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import AuthService from '../../services/AuthService';

interface ProtectedRouteProps {
  allowedRoles?: string[];
  redirectPath?: string;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
  allowedRoles = [], 
  redirectPath = '/login' 
}) => {
  // Obtém o usuário atual através do serviço de autenticação
  const currentUser = AuthService.getCurrentUser();

  // Se não houver usuário (token inválido ou inexistente), redireciona para login
  if (!currentUser) {
    console.warn('🚫 Acesso negado: Usuário não autenticado. Redirecionando para login...');
    return <Navigate to={redirectPath} replace />;
  }

  // Se allowedRoles estiver vazio, qualquer usuário autenticado pode acessar
  if (allowedRoles.length === 0) {
    return <Outlet />;
  }

  // Se houver roles especificadas, verifica se o usuário tem permissão
  if (allowedRoles.includes(currentUser.role)) {
    return <Outlet />;
  }

  // Usuário autenticado mas sem permissão adequada
  console.warn(`🚫 Acesso negado: Role "${currentUser.role}" não autorizada. Requer: ${allowedRoles.join(', ')}`);
  
  // Redireciona para a página apropriada baseado no role
  if (currentUser.role === 'ADMIN' || currentUser.role === 'TECNICO') {
    return <Navigate to="/admin" replace />;
  }
  
  return <Navigate to="/dashboard" replace />;
};

export default ProtectedRoute;
