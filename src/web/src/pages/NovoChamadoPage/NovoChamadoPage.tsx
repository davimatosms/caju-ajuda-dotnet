import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ChamadoService from '../../services/ChamadoService';
import styles from './NovoChamadoPage.module.css';

function NovoChamadoPage() {
    const navigate = useNavigate();
    const [titulo, setTitulo] = useState('');
    const [descricao, setDescricao] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [isError, setIsError] = useState(false);

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        setIsLoading(true);
        setMessage('');
        setIsError(false);

        try {
            await ChamadoService.createChamado({ Titulo: titulo, Descricao: descricao });
            setMessage('Chamado criado com sucesso! Redirecionando para o Dashboard...');

            setTimeout(() => {
                navigate('/'); // Redireciona para o dashboard após 2 segundos
            }, 2000);

        } catch (error) {
            setIsError(true);
            setMessage('Ocorreu um erro ao criar o chamado. Tente novamente.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.pageContainer}>
            <div className={styles.formContainer}>
                <form onSubmit={handleSubmit}>
                    <h2>Abrir Novo Chamado</h2>

                    <div className={styles.inputGroup}>
                        <label htmlFor="titulo">Título</label>
                        <input
                            type="text"
                            id="titulo"
                            value={titulo}
                            onChange={(e) => setTitulo(e.target.value)}
                            required
                            maxLength={150}
                            disabled={isLoading}
                        />
                    </div>

                    <div className={styles.inputGroup}>
                        <label htmlFor="descricao">Descrição do Problema</label>
                        <textarea
                            id="descricao"
                            value={descricao}
                            onChange={(e) => setDescricao(e.target.value)}
                            required
                            disabled={isLoading}
                        />
                    </div>

                    <button type="submit" className={styles.submitButton} disabled={isLoading}>
                        {isLoading ? 'Enviando...' : 'Enviar Chamado'}
                    </button>

                    {message && (
                        <div className={`${styles.message} ${isError ? styles.error : styles.success}`}>
                            {message}
                        </div>
                    )}
                </form>
            </div>
        </div>
    );
}

export default NovoChamadoPage;