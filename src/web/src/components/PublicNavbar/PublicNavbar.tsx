import React, { useState, useRef } from 'react';
import styles from './PublicNavbar.module.css';
import { Link } from 'react-router-dom';
import { Button } from '../../components/UI';

function PublicNavbar() {
  const [open, setOpen] = useState(false);
  const hamburgerRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);
  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const useFocusTrap = require('../../hooks/useFocusTrap').default;
  useFocusTrap(menuRef, open, () => setOpen(false));

  return (
    <nav className={styles.navbar}>
      <Link to="/" className={styles.logoContainer} aria-label="Caju Ajuda">
        <div className={styles.logoText}>Caju Ajuda</div>
      </Link>
      <button ref={hamburgerRef} className={styles.hamburger} aria-label="Abrir menu" aria-expanded={open} aria-controls="public-mobile-menu" onClick={() => setOpen(true)}>
        <span />
        <span />
        <span />
      </button>
      <div className={styles.navLinks}>
        <Link to="/login">Entrar</Link>
        <Link to="/register"><Button className={styles.ctaButton}>Criar Conta</Button></Link>
      </div>

      {open && (
        <div ref={menuRef} id="public-mobile-menu" className={styles.mobileMenu} role="dialog" aria-modal="true" aria-label="Menu Público">
          <button className={styles.mobileClose} aria-label="Fechar menu" onClick={() => setOpen(false)}>×</button>
          <div className={styles.mobileLinks}>
            <Link to="/login" onClick={() => setOpen(false)}>Entrar</Link>
            <Link to="/register" onClick={() => setOpen(false)}>Criar Conta</Link>
          </div>
        </div>
      )}
    </nav>
  );
}

export default PublicNavbar;