import React, { useState, useEffect } from 'react';
import styles from './Navbar.module.css';
import { useNavigate, Link, NavLink } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import ConfirmModal from '../ConfirmModal/ConfirmModal';

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
  const homeLink = userRole === 'ADMIN' ? '/admin/dashboard' : '/';

  return (
    <>
      <nav className={styles.navbar}>
        <Link to={homeLink} className={styles.logo}>Caju Ajuda</Link>
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
               <NavLink to="/" className={({ isActive }) => isActive ? styles.activeLink : ''}>Meus Chamados</NavLink>
            </>
          )}
          <button className={styles.logoutButton} onClick={() => setIsModalOpen(true)}>
            Sair
          </button>
        </div>
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