import React, { useState } from 'react';
import AdminService from '../../../services/AdminService';
import styles from './AddTecnicoModal.module.css';
import { Button } from '../../../components/UI';

interface AddTecnicoModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

function AddTecnicoModal({ isOpen, onClose, onSuccess }: AddTecnicoModalProps) {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  if (!isOpen) return null;

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setIsLoading(true);
    setError('');
    try {
      await AdminService.createTecnico({ Nome: nome, Email: email, Senha: senha });
      onSuccess();
      onClose();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao criar técnico.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <div className={styles.modalHeader}>
          <h3>Adicionar Novo Técnico</h3>
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
          <div className={styles.inputGroup}>
            <label htmlFor="senha">Senha</label>
            <input id="senha" type="password" value={senha} onChange={(e) => setSenha(e.target.value)} required minLength={6} />
          </div>
          {error && <p className={styles.error}>{error}</p>}
          <div className={styles.buttonContainer}>
            <Button type="submit" className={styles.submitButton} disabled={isLoading}>
              {isLoading ? 'Criando...' : 'Criar Técnico'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AddTecnicoModal;