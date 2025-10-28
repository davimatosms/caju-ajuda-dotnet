import React, { useState, useEffect } from 'react';
import ChamadoService, { Chamado } from '../../services/ChamadoService';
import { useNavigate } from 'react-router-dom';

function DashboardPage() {
    const [chamados, setChamados] = useState<Chamado[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState('TODOS');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchChamados = async () => {
            try {
                const response: any = await ChamadoService.getMeusChamados();
                if (response && response.$values) {
                    setChamados(response.$values);
                } else {
                    setChamados(response || []); 
                }
            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar seus chamados.");
                }
            } finally {
                setIsLoading(false);
            }
        };

        fetchChamados();
    }, [navigate]);

    // Filtrar chamados
    const filteredChamados = chamados.filter(chamado => {
        const matchesSearch = chamado.titulo.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            chamado.id.toString().includes(searchTerm);
        const matchesStatus = statusFilter === 'TODOS' || chamado.status === statusFilter;
        return matchesSearch && matchesStatus;
    });

    // Calcular estatísticas
    const stats = {
        total: chamados.length,
        abertos: chamados.filter(c => c.status === 'ABERTO').length,
        emAndamento: chamados.filter(c => c.status === 'EM_ANDAMENTO').length,
        resolvidos: chamados.filter(c => c.status === 'RESOLVIDO').length,
    };

    const getStatusInfo = (status: string) => {
        const statusMap: { [key: string]: { color: string; bg: string; icon: string; label: string } } = {
            'ABERTO': { 
                color: 'text-blue-700', 
                bg: 'bg-blue-50 border-blue-200', 
                icon: '📋',
                label: 'Aberto'
            },
            'EM_ANDAMENTO': { 
                color: 'text-yellow-700', 
                bg: 'bg-yellow-50 border-yellow-200', 
                icon: '⏱️',
                label: 'Em Andamento'
            },
            'PENDENTE': { 
                color: 'text-purple-700', 
                bg: 'bg-purple-50 border-purple-200', 
                icon: '⏸️',
                label: 'Pendente'
            },
            'RESOLVIDO': { 
                color: 'text-green-700', 
                bg: 'bg-green-50 border-green-200', 
                icon: '✅',
                label: 'Resolvido'
            },
        };
        return statusMap[status] || statusMap['ABERTO'];
    };

    const getPrioridadeInfo = (prioridade: string) => {
        const prioridadeMap: { [key: string]: { color: string; label: string } } = {
            'BAIXA': { color: 'bg-gray-100 text-gray-700', label: 'Baixa' },
            'MEDIA': { color: 'bg-yellow-100 text-yellow-800', label: 'Média' },
            'ALTA': { color: 'bg-orange-100 text-orange-800', label: 'Alta' },
            'CRITICA': { color: 'bg-red-100 text-red-800', label: 'Crítica' },
        };
        return prioridadeMap[prioridade] || prioridadeMap['BAIXA'];
    };

    const handleNovoChamadoClick = () => {
        navigate('/chamados/novo');
    };

    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <svg className="animate-spin h-12 w-12 text-primary-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <p className="text-gray-600">Carregando chamados...</p>
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
        <div className="min-h-screen bg-gray-50 py-8">
            <div className="container mx-auto px-4 max-w-7xl">
                {/* Header */}
                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
                    <div>
                        <h1 className="text-3xl font-bold text-gray-900 mb-2">Meus Chamados</h1>
                        <p className="text-gray-600">Gerencie e acompanhe seus tickets de suporte</p>
                    </div>
                    <button 
                        onClick={handleNovoChamadoClick}
                        className="btn-primary flex items-center gap-2"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                        </svg>
                        Novo Chamado
                    </button>
                </div>

                {/* Stats Cards */}
                <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
                    <div 
                        className={`card p-4 cursor-pointer transition-all hover:scale-105 ${
                            statusFilter === 'ABERTO' ? 'ring-2 ring-blue-500' : ''
                        } bg-blue-50 border-blue-200`}
                        onClick={() => setStatusFilter(statusFilter === 'ABERTO' ? 'TODOS' : 'ABERTO')}
                    >
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-blue-600 font-medium mb-1">Abertos</p>
                                <p className="text-3xl font-bold text-blue-700">{stats.abertos}</p>
                            </div>
                            <div className="w-12 h-12 bg-blue-500 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <div 
                        className={`card p-4 cursor-pointer transition-all hover:scale-105 ${
                            statusFilter === 'EM_ANDAMENTO' ? 'ring-2 ring-yellow-500' : ''
                        } bg-yellow-50 border-yellow-200`}
                        onClick={() => setStatusFilter(statusFilter === 'EM_ANDAMENTO' ? 'TODOS' : 'EM_ANDAMENTO')}
                    >
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-yellow-600 font-medium mb-1">Em Andamento</p>
                                <p className="text-3xl font-bold text-yellow-700">{stats.emAndamento}</p>
                            </div>
                            <div className="w-12 h-12 bg-yellow-500 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <div 
                        className={`card p-4 cursor-pointer transition-all hover:scale-105 ${
                            statusFilter === 'RESOLVIDO' ? 'ring-2 ring-green-500' : ''
                        } bg-green-50 border-green-200`}
                        onClick={() => setStatusFilter(statusFilter === 'RESOLVIDO' ? 'TODOS' : 'RESOLVIDO')}
                    >
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-green-600 font-medium mb-1">Resolvidos</p>
                                <p className="text-3xl font-bold text-green-700">{stats.resolvidos}</p>
                            </div>
                            <div className="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <div className="card p-4 bg-primary-50 border-primary-200">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-primary-600 font-medium mb-1">Total</p>
                                <p className="text-3xl font-bold text-primary-700">{stats.total}</p>
                            </div>
                            <div className="w-12 h-12 bg-primary-500 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Search and Filter Bar */}
                <div className="card mb-6 p-4">
                    <div className="flex flex-col sm:flex-row gap-4">
                        <div className="flex-1 relative">
                            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                                </svg>
                            </div>
                            <input
                                type="text"
                                placeholder="Buscar por título ou ID..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                className="input-field pl-10 w-full"
                            />
                        </div>
                        <div className="flex items-center gap-2">
                            <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
                            </svg>
                            <select 
                                value={statusFilter}
                                onChange={(e) => setStatusFilter(e.target.value)}
                                className="input-field"
                            >
                                <option value="TODOS">Todos Status</option>
                                <option value="ABERTO">Abertos</option>
                                <option value="EM_ANDAMENTO">Em Andamento</option>
                                <option value="PENDENTE">Pendentes</option>
                                <option value="RESOLVIDO">Resolvidos</option>
                            </select>
                        </div>
                    </div>
                </div>

                {/* Tickets List */}
                {filteredChamados.length === 0 ? (
                    <div className="card text-center py-16">
                        <svg className="w-20 h-20 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                        </svg>
                        <h3 className="text-xl font-semibold text-gray-900 mb-2">
                            {searchTerm || statusFilter !== 'TODOS' ? 'Nenhum chamado encontrado' : 'Nenhum chamado ainda'}
                        </h3>
                        <p className="text-gray-600 mb-6">
                            {searchTerm || statusFilter !== 'TODOS' 
                                ? 'Tente ajustar os filtros de busca' 
                                : 'Crie seu primeiro chamado para começar'}
                        </p>
                        {!searchTerm && statusFilter === 'TODOS' && (
                            <button onClick={handleNovoChamadoClick} className="btn-primary mx-auto">
                                Criar Primeiro Chamado
                            </button>
                        )}
                    </div>
                ) : (
                    <div className="space-y-4">
                        {filteredChamados.map((chamado) => {
                            const statusInfo = getStatusInfo(chamado.status);
                            const prioridadeInfo = getPrioridadeInfo(chamado.prioridade);
                            
                            return (
                                <div
                                    key={chamado.id}
                                    onClick={() => navigate(`/chamado/${chamado.id}`)}
                                    className={`card border-l-4 hover:shadow-lg transition-all cursor-pointer ${statusInfo.bg} border-l-${statusInfo.color.replace('text-', '')}`}
                                >
                                    <div className="flex items-start justify-between gap-4">
                                        <div className="flex-1">
                                            <div className="flex items-center gap-3 mb-2">
                                                <span className="text-sm font-semibold text-gray-500">
                                                    #{chamado.id}
                                                </span>
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusInfo.color} ${statusInfo.bg}`}>
                                                    <span className="mr-1">{statusInfo.icon}</span>
                                                    {statusInfo.label}
                                                </span>
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${prioridadeInfo.color}`}>
                                                    {prioridadeInfo.label}
                                                </span>
                                            </div>
                                            <h3 className="text-lg font-semibold text-gray-900 mb-2">
                                                {chamado.titulo}
                                            </h3>
                                            <div className="flex items-center text-sm text-gray-500">
                                                <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                                </svg>
                                                {new Date(chamado.dataCriacao).toLocaleDateString('pt-BR', {
                                                    day: '2-digit',
                                                    month: 'short',
                                                    year: 'numeric'
                                                })}
                                            </div>
                                        </div>
                                        <button className="text-gray-400 hover:text-gray-600 transition-colors flex-shrink-0">
                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                                            </svg>
                                        </button>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}
            </div>
        </div>
    );
}

export default DashboardPage;
