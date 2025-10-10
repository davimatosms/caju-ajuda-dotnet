import React from 'react';
import { Outlet } from 'react-router-dom';
import styles from './AuthLayout.module.css';
import PublicNavbar from '../PublicNavbar/PublicNavbar';

function AuthLayout() {
  return (
    <div>
      <PublicNavbar />
      <div className={styles.authContainer}>
        {/* O Outlet renderizará o formulário de Login ou Registro */}
        <Outlet />
      </div>
    </div>
  );
}

export default AuthLayout;