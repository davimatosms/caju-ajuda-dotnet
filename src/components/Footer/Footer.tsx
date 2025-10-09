import React from 'react';
import styles from './Footer.module.css';

function Footer() {
  return (
    <footer className={styles.footer}>
      <div style={{marginBottom:12}}>
        <img src="/assets/caju-logo.png" alt="Caju Ajuda" style={{height:48}} />
      </div>
      <p>&copy; {new Date().getFullYear()} Caju Ajuda. Todos os direitos reservados.</p>
    </footer>
  );
}

export default Footer;