import React, { useState, useEffect } from 'react';
import styles from './Navbar.module.css';
import { useNavigate, Link, NavLink } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import ConfirmModal from '../ConfirmModal/ConfirmModal';
import { Button } from '../UI';


function Navbar() {
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [userRole, setUserRole] = useState<string | null>(null);

  useEffect(() => {
    const user = AuthService.getCurrentUser();
    if (user) {
      setUserRole(user.role);
    }
  }, []);

  const handleLogout = () => {
    AuthService.logout();
    navigate('/login');
  };

  // Define o link principal com base no papel do usuário
  const homeLink = userRole === 'ADMIN' ? '/admin/dashboard' : '/dashboard';
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <>
      <nav className={styles.navbar}>
        <Link to={homeLink} className={styles.logo} aria-label="Caju Ajuda">
          Caju Ajuda
        </Link>
        <button className={styles.hamburger} aria-label="Abrir menu" onClick={() => setMobileOpen(true)}>
          <span />
          <span />
          <span />
        </button>
        <div className={styles.navLinks}>
          {userRole === 'ADMIN' && (
            <>
              <NavLink to="/admin/dashboard" className={({ isActive }) => isActive ? styles.activeLink : ''}>Dashboard</NavLink>
              <NavLink to="/admin/tecnicos" className={({ isActive }) => isActive ? styles.activeLink : ''}>Gerenciar Técnicos</NavLink>
              <NavLink to="/admin/clientes" className={({ isActive }) => isActive ? styles.activeLink : ''}>Gerenciar Clientes</NavLink>
            </>
          )}
           {userRole === 'CLIENTE' && (
            <>
               <NavLink to="/dashboard" className={({ isActive }) => isActive ? styles.activeLink : ''}>Meus Chamados</NavLink>
               <NavLink to="/perfil" className={({ isActive }) => isActive ? styles.activeLink : ''}>Meu Perfil</NavLink>
            </>
          )}
          <Button className={styles.logoutButton} onClick={() => setIsModalOpen(true)}>
            Sair
          </Button>
        </div>
        {mobileOpen && (
          <div className={styles.mobileMenu} role="dialog" aria-modal="true">
            <button className={styles.mobileClose} aria-label="Fechar menu" onClick={() => setMobileOpen(false)}>×</button>
            <div className={styles.mobileLinks}>
              {userRole === 'ADMIN' && (
                <>
                  <Link to="/admin/dashboard" onClick={() => setMobileOpen(false)}>Dashboard</Link>
                  <Link to="/admin/tecnicos" onClick={() => setMobileOpen(false)}>Gerenciar Técnicos</Link>
                  <Link to="/admin/clientes" onClick={() => setMobileOpen(false)}>Gerenciar Clientes</Link>
                </>
              )}
              {userRole === 'CLIENTE' && (
                <>
                  <Link to="/dashboard" onClick={() => setMobileOpen(false)}>Meus Chamados</Link>
                  <Link to="/perfil" onClick={() => setMobileOpen(false)}>Meu Perfil</Link>
                </>
              )}
              <button className={styles.logoutButton} onClick={() => { setIsModalOpen(true); setMobileOpen(false); }}>Sair</button>
            </div>
          </div>
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