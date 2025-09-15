import React from 'react';
import { Outlet } from 'react-router-dom';
import styles from './AuthLayout.module.css';

function AuthLayout() {
  return (
    <div className={styles.authContainer}>
      {/* O Outlet renderizará o formulário de Login ou Registro */}
      <Outlet />
    </div>
  );
}

export default AuthLayout;