import React from 'react';
import styles from './Footer.module.css';
import HeroCaju from '../HeroCaju/HeroCaju';

function Footer() {
  return (
    <footer className={styles.footer}>
      <div style={{marginBottom:12}}>
        <HeroCaju size="small" decorative />
      </div>
      <p>&copy; {new Date().getFullYear()} Caju Ajuda. Todos os direitos reservados.</p>
    </footer>
  );
}

export default Footer;