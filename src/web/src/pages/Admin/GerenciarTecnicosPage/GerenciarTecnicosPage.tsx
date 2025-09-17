import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { Tecnico } from '../../../services/AdminService';
import styles from './GerenciarTecnicosPage.module.css';
import AddTecnicoModal from '../AddTecnicoModal/AddTecnicoModal';
import EditTecnicoModal from '../EditTecnicoModal/EditTecnicoModal';
import ConfirmModal from '../../../components/ConfirmModal/ConfirmModal';
import InfoModal from '../../../components/InfoModal/InfoModal'; 

function GerenciarTecnicosPage() {
    const [tecnicos, setTecnicos] = useState<Tecnico[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    
    // Controles dos modais
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedTecnico, setSelectedTecnico] = useState<Tecnico | null>(null);
    const [confirmToggleAction, setConfirmToggleAction] = useState<{ id: number; nome: string; enabled: boolean } | null>(null);
    const [resetPasswordAction, setResetPasswordAction] = useState<{ id: number; nome: string } | null>(null);
    
    
    const [infoModalData, setInfoModalData] = useState<{ title: string; message: string; info: string } | null>(null);

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
        if (!confirmToggleAction) return;
        try {
            await AdminService.toggleTecnicoStatus(confirmToggleAction.id);
            fetchTecnicos();
        } catch (error) {
            alert("Não foi possível alterar o status do técnico.");
        } finally {
            setConfirmToggleAction(null);
        }
    };

    
    const handleConfirmResetPassword = async () => {
        if (!resetPasswordAction) return;
        try {
            const response = await AdminService.resetPassword(resetPasswordAction.id);
            setInfoModalData({
                title: "Senha Redefinida com Sucesso!",
                message: `A nova senha temporária para ${resetPasswordAction.nome} é:`,
                info: response.temporaryPassword
            });
        } catch (error) {
            alert("Não foi possível resetar a senha do técnico.");
        } finally {
            setResetPasswordAction(null); // Fecha o modal de confirmação
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
                                        onClick={() => setConfirmToggleAction({ id: tecnico.id, nome: tecnico.nome, enabled: tecnico.enabled })}
                                    >
                                        {tecnico.enabled ? 'Desativar' : 'Ativar'}
                                    </button>
                                    <button
                                        className={`${styles.actionButton} ${styles.resetButton}`}
                                        onClick={() => setResetPasswordAction({ id: tecnico.id, nome: tecnico.nome })}
                                    >
                                        Resetar Senha
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
                isOpen={!!confirmToggleAction}
                onClose={() => setConfirmToggleAction(null)}
                onConfirm={handleConfirmToggleStatus}
                title="Confirmar Alteração de Status"
                message={`Você tem certeza que deseja ${confirmToggleAction?.enabled ? 'desativar' : 'ativar'} o técnico ${confirmToggleAction?.nome}?`}
            />

            <ConfirmModal
                isOpen={!!resetPasswordAction}
                onClose={() => setResetPasswordAction(null)}
                onConfirm={handleConfirmResetPassword}
                title="Confirmar Reset de Senha"
                message={`Você tem certeza que deseja resetar a senha do técnico ${resetPasswordAction?.nome}? Esta ação não pode ser desfeita.`}
            />

            
            <InfoModal
                isOpen={!!infoModalData}
                onClose={() => setInfoModalData(null)}
                title={infoModalData?.title || ''}
                message={infoModalData?.message || ''}
                info={infoModalData?.info || ''}
            />
        </>
    );
}

export default GerenciarTecnicosPage;