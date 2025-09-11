import React, { useState, useEffect } from 'react';
import styles from './Navbar.module.css';
import { useNavigate, Link } from 'react-router-dom';
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

  return (
    <>
      <nav className={styles.navbar}>
        <div className={styles.logo}>Caju Ajuda</div>
        <div className={styles.navLinks}>
          {/* O link só aparece se o usuário for ADMIN */}
          {userRole === 'ADMIN' && (
            <Link to="/admin/tecnicos">Gerenciar Técnicos</Link>
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