import React from 'react';
import styles from './PublicNavbar.module.css';
import { Link } from 'react-router-dom';
import logoCaju from '../../assets/logo-caju.png'; 

function PublicNavbar() {
  return (
    <nav className={styles.navbar}>
      <Link to="/" className={styles.logoContainer}>
        {/* <img src={logoCaju} alt="Logo Caju Ajuda" className={styles.logoImage} /> */}
        <span className={styles.logoText}>Caju Ajuda</span>
      </Link>
      <div className={styles.navLinks}>
        {/* NOVO LINK PARA A BASE DE CONHECIMENTO */}
        <Link to="/ajuda">Ajuda</Link>
        <Link to="/login">Entrar</Link>
        <Link to="/register" className={styles.ctaButton}>Criar Conta</Link>
      </div>
    </nav>
  );
}

export default PublicNavbar;