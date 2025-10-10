import React from 'react';
import styles from './Footer.module.css';
// Hero artwork removed to keep app logo-free; decorative artwork archived.

function Footer() {
  return (
    <footer className={styles.footer}>
      {/* decorative hero removed */}
      <p>&copy; {new Date().getFullYear()} Caju Ajuda. Todos os direitos reservados.</p>
    </footer>
  );
}

export default Footer;