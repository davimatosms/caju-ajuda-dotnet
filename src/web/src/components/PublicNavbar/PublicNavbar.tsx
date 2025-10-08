import React from 'react';
import styles from './PublicNavbar.module.css';
import { Link } from 'react-router-dom';
import { Button } from '../../components/UI';
import logoCaju from 'assets/logo-caju.png'; 

function PublicNavbar() {
  return (
    <nav className={styles.navbar}>
      <Link to="/" className={styles.logoContainer}>
        {/* <img src={logoCaju} alt="Logo Caju Ajuda" className={styles.logoImage} /> */}
        <span className={styles.logoText}>Caju Ajuda</span>
      </Link>
      <div className={styles.navLinks}>
        <Link to="/login">Entrar</Link>
        <Link to="/register"><Button className={styles.ctaButton}>Criar Conta</Button></Link>
      </div>
    </nav>
  );
}

export default PublicNavbar;