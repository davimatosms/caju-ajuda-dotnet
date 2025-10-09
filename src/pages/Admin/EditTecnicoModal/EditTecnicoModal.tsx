import React, { useState, useEffect } from 'react';
import AdminService, { Tecnico } from 'services/AdminService';
import styles from './EditTecnicoModal.module.css';
import { Button } from '../../../components/UI';

interface EditTecnicoModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  tecnico: Tecnico | null; // Recebe o técnico a ser editado
}

function EditTecnicoModal({ isOpen, onClose, onSuccess, tecnico }: EditTecnicoModalProps) {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  // useEffect para preencher o formulário quando o modal abre
  useEffect(() => {
    if (tecnico) {
      setNome(tecnico.nome);
      setEmail(tecnico.email);
    }
  }, [tecnico]);

  if (!isOpen || !tecnico) return null;

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setIsLoading(true);
    setError('');
    try {
      await AdminService.updateTecnico(tecnico.id, { Nome: nome, Email: email });
      onSuccess();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao atualizar técnico.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <div className={styles.modalHeader}>
          <h3>Editar Técnico</h3>
          <Button onClick={onClose} className={styles.closeButton}>&times;</Button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className={styles.inputGroup}>
            <label htmlFor="nome">Nome</label>
            <input id="nome" type="text" value={nome} onChange={(e) => setNome(e.target.value)} required />
          </div>
          <div className={styles.inputGroup}>
            <label htmlFor="email">E-mail</label>
            <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          {error && <p className={styles.error}>{error}</p>}
          <div className={styles.buttonContainer}>
            <Button type="submit" className={styles.submitButton} disabled={isLoading}>
              {isLoading ? 'Salvando...' : 'Salvar Alterações'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default EditTecnicoModal;