import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { Tecnico } from '../../../services/AdminService';
import styles from './GerenciarTecnicosPage.module.css';
import AddTecnicoModal from '../AddTecnicoModal/AddTecnicoModal';

function GerenciarTecnicosPage() {
    const [tecnicos, setTecnicos] = useState<Tecnico[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    
    const [isModalOpen, setIsModalOpen] = useState(false);

    const fetchTecnicos = async () => {
        try {
            const response = await AdminService.getTecnicos();
            setTecnicos(response?.$values || response || []);
        } catch (err: any) {
             if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                navigate('/login');
            } else {
                setError("Não foi possível carregar a lista de técnicos.");
            }
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        setIsLoading(true);
        fetchTecnicos();
    }, [navigate]);

    if (isLoading) {
        return <div className={styles.loading}>Carregando técnicos...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }
    
    return (
        <>
            <div className={styles.pageContainer}>
                <div className={styles.header}>
                    <h1>Gerenciar Técnicos</h1>
                    <button className={styles.addButton} onClick={() => setIsModalOpen(true)}>
                        Adicionar Novo Técnico
                    </button>
                </div>
                <table className={styles.tecnicosTable}>
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>E-mail</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tecnicos.map(tecnico => (
                            <tr key={tecnico.id}>
                                <td>{tecnico.nome}</td>
                                <td>{tecnico.email}</td>
                                <td>
                                    <span className={`${styles.statusBadge} ${tecnico.enabled ? styles.statusAtivo : styles.statusInativo}`}>
                                        {tecnico.enabled ? 'Ativo' : 'Inativo'}
                                    </span>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            
            <AddTecnicoModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSuccess={() => {
                    fetchTecnicos();
                }}
            />
        </>
    );
}

export default GerenciarTecnicosPage;