import React from 'react';
import styles from './InfoModal.module.css';
import { Button } from '../UI';

interface InfoModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  message: string;
  info: string;
}

function InfoModal({ isOpen, onClose, title, message, info }: InfoModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <h3>{title}</h3>
        <p>{message}</p>
        <div className={styles.infoBox}>{info}</div>
        <Button className={styles.closeButton} onClick={onClose}>
          Fechar
        </Button>
      </div>
    </div>
  );
}

export default InfoModal;