import React from 'react';
import styles from './ConfirmModal.module.css';
import { Button } from '../UI';

// Definimos as propriedades que o modal irá receber
interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
}

function ConfirmModal({ isOpen, onClose, onConfirm, title, message }: ConfirmModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <h3>{title}</h3>
        <p>{message}</p>
        <div className={styles.buttonContainer}>
          <Button className={styles.cancelButton} onClick={onClose}>
            Cancelar
          </Button>
          <Button className={styles.confirmButton} onClick={onConfirm}>
            Confirmar
          </Button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmModal;