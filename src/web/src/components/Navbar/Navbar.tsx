import React, { useState, useEffect, useRef } from 'react';
import { useNavigate, Link, NavLink } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import ConfirmModal from '../ConfirmModal/ConfirmModal';
import cajuLogo from '../../assets/Caju.png';

function Navbar() {
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [userRole, setUserRole] = useState<string | null>(null);
  const [userName, setUserName] = useState<string>('');
  const [mobileOpen, setMobileOpen] = useState(false);
  const hamburgerRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const user = AuthService.getCurrentUser();
    if (user) {
      setUserRole(user.role);
      setUserName(user.name || 'Usuário');
    }
  }, []);

  const handleLogout = () => {
    AuthService.logout();
    navigate('/login');
  };

  const homeLink = userRole === 'ADMIN' ? '/admin/dashboard' : '/dashboard';

  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const useFocusTrap = require('../../hooks/useFocusTrap').default;
  useFocusTrap(menuRef, mobileOpen, () => setMobileOpen(false));

  return (
    <>
      <nav className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-50">
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between h-16">
            {/* Logo */}
            <Link to={homeLink} className="flex items-center space-x-2" aria-label="Caju Ajuda">
              <img src={cajuLogo} alt="Caju Ajuda" className="h-10 w-auto" />
              <span className="text-xl font-bold text-gray-900 hidden sm:block">Caju Ajuda</span>
            </Link>

            {/* Desktop Navigation */}
            <div className="hidden md:flex items-center space-x-1">
              {userRole === 'ADMIN' && (
                <>
                  <NavLink
                    to="/admin/dashboard"
                    className={({ isActive }) =>
                      `px-4 py-2 rounded-lg font-medium transition-colors ${
                        isActive
                          ? 'bg-primary-50 text-primary-700'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`
                    }
                  >
                    Dashboard
                  </NavLink>
                  <NavLink
                    to="/admin/tecnicos"
                    className={({ isActive }) =>
                      `px-4 py-2 rounded-lg font-medium transition-colors ${
                        isActive
                          ? 'bg-primary-50 text-primary-700'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`
                    }
                  >
                    Técnicos
                  </NavLink>
                  <NavLink
                    to="/admin/clientes"
                    className={({ isActive }) =>
                      `px-4 py-2 rounded-lg font-medium transition-colors ${
                        isActive
                          ? 'bg-primary-50 text-primary-700'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`
                    }
                  >
                    Clientes
                  </NavLink>
                </>
              )}
              {userRole === 'CLIENTE' && (
                <>
                  <NavLink
                    to="/dashboard"
                    className={({ isActive }) =>
                      `px-4 py-2 rounded-lg font-medium transition-colors ${
                        isActive
                          ? 'bg-primary-50 text-primary-700'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`
                    }
                  >
                    Meus Chamados
                  </NavLink>
                  <NavLink
                    to="/perfil"
                    className={({ isActive }) =>
                      `px-4 py-2 rounded-lg font-medium transition-colors ${
                        isActive
                          ? 'bg-primary-50 text-primary-700'
                          : 'text-gray-700 hover:bg-gray-100'
                      }`
                    }
                  >
                    Meu Perfil
                  </NavLink>
                </>
              )}

              {/* User Menu */}
              <div className="ml-4 flex items-center space-x-3">
                <div className="hidden lg:block text-right">
                  <p className="text-sm font-medium text-gray-900">{userName}</p>
                  <p className="text-xs text-gray-500">{userRole === 'ADMIN' ? 'Administrador' : 'Cliente'}</p>
                </div>
                <button
                  onClick={() => setIsModalOpen(true)}
                  className="flex items-center space-x-2 px-4 py-2 text-gray-700 hover:bg-red-50 hover:text-red-600 rounded-lg transition-colors"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span className="font-medium">Sair</span>
                </button>
              </div>
            </div>

            {/* Mobile Hamburger */}
            <button
              ref={hamburgerRef}
              className="md:hidden p-2 rounded-lg hover:bg-gray-100 transition-colors"
              aria-label="Abrir menu"
              aria-expanded={mobileOpen}
              aria-controls="mobile-menu"
              onClick={() => setMobileOpen(true)}
            >
              <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>

        {/* Mobile Menu */}
        {mobileOpen && (
          <>
            {/* Backdrop */}
            <div 
              className="fixed inset-0 bg-black bg-opacity-50 z-40 md:hidden"
              onClick={() => setMobileOpen(false)}
            />
            
            {/* Menu Panel */}
            <div
              ref={menuRef}
              id="mobile-menu"
              className="fixed top-0 right-0 bottom-0 w-72 bg-white shadow-xl z-50 md:hidden animate-slide-in"
              role="dialog"
              aria-modal="true"
              aria-label="Menu Principal"
            >
              <div className="p-4 h-full flex flex-col">
                {/* Header */}
                <div className="flex justify-between items-center mb-6">
                  <div>
                    <p className="text-lg font-bold text-gray-900">{userName}</p>
                    <p className="text-sm text-gray-500">{userRole === 'ADMIN' ? 'Administrador' : 'Cliente'}</p>
                  </div>
                  <button
                    className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
                    aria-label="Fechar menu"
                    onClick={() => setMobileOpen(false)}
                  >
                    <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
                
                {/* Menu Items */}
                <div className="flex-1 flex flex-col space-y-2">
                  {userRole === 'ADMIN' && (
                    <>
                      <NavLink
                        to="/admin/dashboard"
                        onClick={() => setMobileOpen(false)}
                        className={({ isActive }) =>
                          `px-4 py-3 rounded-lg font-medium transition-colors ${
                            isActive
                              ? 'bg-primary-50 text-primary-700'
                              : 'text-gray-700 hover:bg-gray-100'
                          }`
                        }
                      >
                        📊 Dashboard
                      </NavLink>
                      <NavLink
                        to="/admin/tecnicos"
                        onClick={() => setMobileOpen(false)}
                        className={({ isActive }) =>
                          `px-4 py-3 rounded-lg font-medium transition-colors ${
                            isActive
                              ? 'bg-primary-50 text-primary-700'
                              : 'text-gray-700 hover:bg-gray-100'
                          }`
                        }
                      >
                        👨‍💻 Técnicos
                      </NavLink>
                      <NavLink
                        to="/admin/clientes"
                        onClick={() => setMobileOpen(false)}
                        className={({ isActive }) =>
                          `px-4 py-3 rounded-lg font-medium transition-colors ${
                            isActive
                              ? 'bg-primary-50 text-primary-700'
                              : 'text-gray-700 hover:bg-gray-100'
                          }`
                        }
                      >
                        👥 Clientes
                      </NavLink>
                    </>
                  )}
                  {userRole === 'CLIENTE' && (
                    <>
                      <NavLink
                        to="/dashboard"
                        onClick={() => setMobileOpen(false)}
                        className={({ isActive }) =>
                          `px-4 py-3 rounded-lg font-medium transition-colors ${
                            isActive
                              ? 'bg-primary-50 text-primary-700'
                              : 'text-gray-700 hover:bg-gray-100'
                          }`
                        }
                      >
                        📋 Meus Chamados
                      </NavLink>
                      <NavLink
                        to="/perfil"
                        onClick={() => setMobileOpen(false)}
                        className={({ isActive }) =>
                          `px-4 py-3 rounded-lg font-medium transition-colors ${
                            isActive
                              ? 'bg-primary-50 text-primary-700'
                              : 'text-gray-700 hover:bg-gray-100'
                          }`
                        }
                      >
                        👤 Meu Perfil
                      </NavLink>
                    </>
                  )}
                </div>

                {/* Logout Button */}
                <button
                  onClick={() => { setIsModalOpen(true); setMobileOpen(false); }}
                  className="w-full px-4 py-3 bg-red-50 text-red-600 rounded-lg font-medium hover:bg-red-100 transition-colors flex items-center justify-center space-x-2"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span>Sair</span>
                </button>
              </div>
            </div>
          </>
        )}
      </nav>

      <ConfirmModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onConfirm={handleLogout}
        title="Confirmar Saída"
        message="Você tem certeza que deseja sair do sistema?"
      />
    </>
  );
}

export default Navbar;