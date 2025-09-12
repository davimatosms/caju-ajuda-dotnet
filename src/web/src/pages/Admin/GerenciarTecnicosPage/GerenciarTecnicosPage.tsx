import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { Tecnico } from '../../../services/AdminService';
import styles from './GerenciarTecnicosPage.module.css';
import AddTecnicoModal from '../AddTecnicoModal/AddTecnicoModal';
import EditTecnicoModal from '../EditTecnicoModal/EditTecnicoModal';
import ConfirmModal from '../../../components/ConfirmModal/ConfirmModal';

function GerenciarTecnicosPage() {
    const [tecnicos, setTecnicos] = useState<Tecnico[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedTecnico, setSelectedTecnico] = useState<Tecnico | null>(null);
    const [confirmAction, setConfirmAction] = useState<{ id: number; nome: string; enabled: boolean } | null>(null);

    const fetchTecnicos = useCallback(async () => {
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
    }, [navigate]);

    useEffect(() => {
        setIsLoading(true);
        fetchTecnicos();
    }, [fetchTecnicos]);

    const handleConfirmToggleStatus = async () => {
        if (!confirmAction) return;
        try {
            await AdminService.toggleTecnicoStatus(confirmAction.id);
            fetchTecnicos();
        } catch (error) {
            alert("Não foi possível alterar o status do técnico.");
        } finally {
            setConfirmAction(null);
        }
    };

    const handleEditClick = (tecnico: Tecnico) => {
        setSelectedTecnico(tecnico);
        setIsEditModalOpen(true);
    };

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
                    <button className={styles.addButton} onClick={() => setIsAddModalOpen(true)}>
                        Adicionar Novo Técnico
                    </button>
                </div>
                <table className={styles.tecnicosTable}>
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>E-mail</th>
                            <th>Status</th>
                            <th>Ações</th>
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
                                <td className={styles.actionsCell}>
                                    <button 
                                        className={`${styles.actionButton} ${styles.editButton}`}
                                        onClick={() => handleEditClick(tecnico)}
                                    >
                                        Editar
                                    </button>
                                    <button 
                                        className={`${styles.actionButton} ${styles.toggleButton}`}
                                        onClick={() => setConfirmAction({ id: tecnico.id, nome: tecnico.nome, enabled: tecnico.enabled })}
                                    >
                                        {tecnico.enabled ? 'Desativar' : 'Ativar'}
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            
            <AddTecnicoModal
                isOpen={isAddModalOpen}
                onClose={() => setIsAddModalOpen(false)}
                onSuccess={fetchTecnicos}
            />
            <EditTecnicoModal
                isOpen={isEditModalOpen}
                onClose={() => setIsEditModalOpen(false)}
                onSuccess={fetchTecnicos}
                tecnico={selectedTecnico}
            />
            <ConfirmModal
                isOpen={!!confirmAction}
                onClose={() => setConfirmAction(null)}
                onConfirm={handleConfirmToggleStatus}
                title="Confirmar Alteração de Status"
                message={`Você tem certeza que deseja ${confirmAction?.enabled ? 'desativar' : 'ativar'} o técnico ${confirmAction?.nome}?`}
            />
        </>
    );
}

export default GerenciarTecnicosPage;