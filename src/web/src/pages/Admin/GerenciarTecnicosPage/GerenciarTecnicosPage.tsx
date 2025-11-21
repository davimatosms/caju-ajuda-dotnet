import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { Tecnico } from '../../../services/AdminService';
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
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <svg className="animate-spin h-12 w-12 text-primary-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <p className="text-gray-600">Carregando técnicos...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="card max-w-md bg-red-50 border border-red-200">
                    <div className="flex items-center text-red-700">
                        <svg className="w-6 h-6 mr-2" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                        </svg>
                        <span>{error}</span>
                    </div>
                </div>
            </div>
        );
    }
    
    return (
        <>
            <div className="min-h-screen bg-gray-50 py-8">
                <div className="container mx-auto px-4 max-w-7xl">
                    {/* Header */}
                    <div className="mb-8 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                        <div>
                            <h1 className="text-3xl font-bold text-gray-900 mb-2">Gerenciar Técnicos</h1>
                            <p className="text-gray-600">Administre a equipe de suporte técnico</p>
                        </div>
                        <button 
                            onClick={() => setIsAddModalOpen(true)}
                            className="btn-primary flex items-center gap-2"
                        >
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                            </svg>
                            Adicionar Novo Técnico
                        </button>
                    </div>

                    {/* Desktop Table */}
                    <div className="hidden md:block card overflow-hidden">
                        <div className="overflow-x-auto">
                            <table className="min-w-full divide-y divide-gray-200">
                                <thead className="bg-gray-50">
                                    <tr>
                                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Nome
                                        </th>
                                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            E-mail
                                        </th>
                                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Status
                                        </th>
                                        <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Ações
                                        </th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white divide-y divide-gray-200">
                                    {tecnicos.map(tecnico => (
                                        <tr key={tecnico.id} className="hover:bg-gray-50 transition-colors">
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="flex items-center">
                                                    <div className="flex-shrink-0 h-10 w-10 bg-primary-100 rounded-full flex items-center justify-center">
                                                        <span className="text-primary-600 font-semibold text-sm">
                                                            {tecnico.nome.charAt(0).toUpperCase()}
                                                        </span>
                                                    </div>
                                                    <div className="ml-4">
                                                        <div className="text-sm font-medium text-gray-900">{tecnico.nome}</div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm text-gray-900">{tecnico.email}</div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className={`badge ${tecnico.enabled ? 'badge-success' : 'badge-secondary'}`}>
                                                    {tecnico.enabled ? 'Ativo' : 'Inativo'}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                                <div className="flex justify-end gap-2">
                                                    <button
                                                        onClick={() => handleEditClick(tecnico)}
                                                        className="text-blue-600 hover:text-blue-900 font-medium flex items-center gap-1 px-3 py-1 rounded-lg hover:bg-blue-50 transition-colors"
                                                    >
                                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                                        </svg>
                                                        Editar
                                                    </button>
                                                    <button
                                                        onClick={() => setConfirmToggleAction({ id: tecnico.id, nome: tecnico.nome, enabled: tecnico.enabled })}
                                                        className={`font-medium flex items-center gap-1 px-3 py-1 rounded-lg transition-colors ${
                                                            tecnico.enabled 
                                                                ? 'text-red-600 hover:text-red-900 hover:bg-red-50' 
                                                                : 'text-green-600 hover:text-green-900 hover:bg-green-50'
                                                        }`}
                                                    >
                                                        {tecnico.enabled ? (
                                                            <>
                                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728A9 9 0 015.636 5.636m12.728 12.728L5.636 5.636" />
                                                                </svg>
                                                                Desativar
                                                            </>
                                                        ) : (
                                                            <>
                                                                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                                                </svg>
                                                                Ativar
                                                            </>
                                                        )}
                                                    </button>
                                                    <button
                                                        onClick={() => setResetPasswordAction({ id: tecnico.id, nome: tecnico.nome })}
                                                        className="text-purple-600 hover:text-purple-900 font-medium flex items-center gap-1 px-3 py-1 rounded-lg hover:bg-purple-50 transition-colors"
                                                    >
                                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                                                        </svg>
                                                        Resetar Senha
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>

                    {/* Mobile Cards */}
                    <div className="md:hidden space-y-4">
                        {tecnicos.map(tecnico => (
                            <div key={tecnico.id} className="card">
                                <div className="flex items-center mb-3">
                                    <div className="flex-shrink-0 h-12 w-12 bg-primary-100 rounded-full flex items-center justify-center">
                                        <span className="text-primary-600 font-semibold">
                                            {tecnico.nome.charAt(0).toUpperCase()}
                                        </span>
                                    </div>
                                    <div className="ml-4 flex-1">
                                        <div className="text-base font-semibold text-gray-900">{tecnico.nome}</div>
                                        <div className="text-sm text-gray-600">{tecnico.email}</div>
                                    </div>
                                    <span className={`badge ${tecnico.enabled ? 'badge-success' : 'badge-secondary'}`}>
                                        {tecnico.enabled ? 'Ativo' : 'Inativo'}
                                    </span>
                                </div>
                                <div className="flex gap-2 mt-4">
                                    <button
                                        onClick={() => handleEditClick(tecnico)}
                                        className="flex-1 btn-secondary text-sm py-2 flex items-center justify-center gap-1"
                                    >
                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                        </svg>
                                        Editar
                                    </button>
                                    <button
                                        onClick={() => setConfirmToggleAction({ id: tecnico.id, nome: tecnico.nome, enabled: tecnico.enabled })}
                                        className={`flex-1 text-sm py-2 px-4 rounded-lg font-medium flex items-center justify-center gap-1 transition-colors ${
                                            tecnico.enabled 
                                                ? 'bg-red-50 text-red-600 hover:bg-red-100' 
                                                : 'bg-green-50 text-green-600 hover:bg-green-100'
                                        }`}
                                    >
                                        {tecnico.enabled ? 'Desativar' : 'Ativar'}
                                    </button>
                                    <button
                                        onClick={() => setResetPasswordAction({ id: tecnico.id, nome: tecnico.nome })}
                                        className="btn-secondary text-sm py-2 px-3 flex items-center justify-center"
                                    >
                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                                        </svg>
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>

                    {/* Empty State */}
                    {tecnicos.length === 0 && (
                        <div className="card text-center py-12">
                            <svg className="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                            </svg>
                            <h3 className="text-lg font-semibold text-gray-900 mb-2">Nenhum técnico cadastrado</h3>
                            <p className="text-gray-600 mb-4">Comece adicionando o primeiro técnico da equipe</p>
                            <button 
                                onClick={() => setIsAddModalOpen(true)}
                                className="btn-primary mx-auto"
                            >
                                Adicionar Primeiro Técnico
                            </button>
                        </div>
                    )}
                </div>
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