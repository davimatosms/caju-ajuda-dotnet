import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { DashboardMetrics } from '../../../services/AdminService';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, LineChart, Line, PieChart, Pie, Cell } from 'recharts';

const COLORS = ['#f97316', '#3b82f6', '#10b981', '#f59e0b', '#ef4444'];

function DashboardAdminPage() {
    const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchMetrics = async () => {
            try {
                const data = await AdminService.getDashboardMetrics();
                setMetrics(data);
            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar as métricas do dashboard.");
                }
            } finally {
                setIsLoading(false);
            }
        };
        fetchMetrics();
    }, [navigate]);

    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <svg className="animate-spin h-12 w-12 text-primary-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <p className="text-gray-600">Carregando métricas...</p>
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

    if (!metrics) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="card max-w-md text-center">
                    <p className="text-gray-600">Nenhuma métrica encontrada.</p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 py-8">
            <div className="container mx-auto px-4 max-w-7xl">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">Dashboard Administrativo</h1>
                    <p className="text-gray-600">Visão geral do sistema e métricas de desempenho</p>
                </div>

                {/* Stats Cards */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                    {/* Total de Chamados */}
                    <div className="card bg-gradient-to-br from-blue-500 to-blue-600 text-white">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-blue-100 text-sm font-medium mb-1">Total de Chamados</p>
                                <p className="text-4xl font-bold">{metrics.totalChamados}</p>
                            </div>
                            <div className="w-16 h-16 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    {/* Chamados Abertos */}
                    <div className="card bg-gradient-to-br from-orange-500 to-orange-600 text-white">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-orange-100 text-sm font-medium mb-1">Chamados Abertos</p>
                                <p className="text-4xl font-bold">{metrics.chamadosAbertos}</p>
                            </div>
                            <div className="w-16 h-16 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    {/* Tempo Médio de Resposta */}
                    <div className="card bg-gradient-to-br from-purple-500 to-purple-600 text-white">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-purple-100 text-sm font-medium mb-1">Tempo Médio de Resposta</p>
                                <p className="text-4xl font-bold">{metrics.tempoMedioPrimeiraRespostaHoras.toFixed(1)}<span className="text-2xl">h</span></p>
                            </div>
                            <div className="w-16 h-16 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    {/* Tempo Médio de Resolução */}
                    <div className="card bg-gradient-to-br from-green-500 to-green-600 text-white">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-green-100 text-sm font-medium mb-1">Tempo Médio de Resolução</p>
                                <p className="text-4xl font-bold">{metrics.tempoMedioResolucaoHoras.toFixed(1)}<span className="text-2xl">h</span></p>
                            </div>
                            <div className="w-16 h-16 bg-white bg-opacity-20 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Charts Grid */}
                <div className="grid lg:grid-cols-2 gap-6 mb-8">
                    {/* Line Chart - Atividade */}
                    <div className="card">
                        <h3 className="text-lg font-semibold text-gray-900 mb-4">Atividade nos Últimos 7 Dias</h3>
                        <ResponsiveContainer width="100%" height={300}>
                            <LineChart data={metrics.statsUltimos7Dias}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                                <XAxis 
                                    dataKey="dia" 
                                    stroke="#6b7280"
                                    style={{ fontSize: '12px' }}
                                />
                                <YAxis 
                                    allowDecimals={false} 
                                    stroke="#6b7280"
                                    style={{ fontSize: '12px' }}
                                />
                                <Tooltip 
                                    contentStyle={{ 
                                        backgroundColor: '#fff', 
                                        border: '1px solid #e5e7eb',
                                        borderRadius: '8px',
                                        boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                                    }}
                                />
                                <Legend />
                                <Line 
                                    type="monotone" 
                                    dataKey="criados" 
                                    name="Criados" 
                                    stroke="#f97316" 
                                    strokeWidth={2}
                                    dot={{ fill: '#f97316', r: 4 }}
                                    activeDot={{ r: 6 }}
                                />
                                <Line 
                                    type="monotone" 
                                    dataKey="fechados" 
                                    name="Fechados" 
                                    stroke="#10b981" 
                                    strokeWidth={2}
                                    dot={{ fill: '#10b981', r: 4 }}
                                    activeDot={{ r: 6 }}
                                />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>

                    {/* Bar Chart - Prioridade */}
                    <div className="card">
                        <h3 className="text-lg font-semibold text-gray-900 mb-4">Chamados por Prioridade</h3>
                        <ResponsiveContainer width="100%" height={300}>
                            <BarChart data={metrics.chamadosPorPrioridade}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                                <XAxis 
                                    dataKey="name" 
                                    stroke="#6b7280"
                                    style={{ fontSize: '12px' }}
                                />
                                <YAxis 
                                    allowDecimals={false} 
                                    stroke="#6b7280"
                                    style={{ fontSize: '12px' }}
                                />
                                <Tooltip 
                                    contentStyle={{ 
                                        backgroundColor: '#fff', 
                                        border: '1px solid #e5e7eb',
                                        borderRadius: '8px',
                                        boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                                    }}
                                />
                                <Legend />
                                <Bar 
                                    dataKey="total" 
                                    name="Total" 
                                    fill="#f97316" 
                                    radius={[8, 8, 0, 0]}
                                />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                {/* Additional Stats */}
                <div className="grid md:grid-cols-3 gap-6">
                    {/* Performance Card */}
                    <div className="card">
                        <div className="flex items-center mb-4">
                            <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
                                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                                </svg>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">Performance</h3>
                        </div>
                        <div className="space-y-3">
                            <div className="flex justify-between items-center">
                                <span className="text-sm text-gray-600">Taxa de Resolução</span>
                                <span className="text-sm font-semibold text-gray-900">
                                    {metrics.totalChamados > 0 
                                        ? ((metrics.totalChamados - metrics.chamadosAbertos) / metrics.totalChamados * 100).toFixed(1)
                                        : 0}%
                                </span>
                            </div>
                            <div className="w-full bg-gray-200 rounded-full h-2">
                                <div 
                                    className="bg-green-500 h-2 rounded-full transition-all duration-500" 
                                    style={{ 
                                        width: `${metrics.totalChamados > 0 
                                            ? ((metrics.totalChamados - metrics.chamadosAbertos) / metrics.totalChamados * 100) 
                                            : 0}%` 
                                    }}
                                />
                            </div>
                        </div>
                    </div>

                    {/* Quick Stats */}
                    <div className="card">
                        <div className="flex items-center mb-4">
                            <div className="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center mr-3">
                                <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                                </svg>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">Resumo Rápido</h3>
                        </div>
                        <div className="space-y-2 text-sm">
                            <div className="flex justify-between">
                                <span className="text-gray-600">Em Andamento</span>
                                <span className="font-semibold text-gray-900">
                                    {metrics.totalChamados - metrics.chamadosAbertos}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-600">Resolvidos Hoje</span>
                                <span className="font-semibold text-gray-900">
                                    {metrics.statsUltimos7Dias?.[metrics.statsUltimos7Dias.length - 1]?.fechados || 0}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-600">Criados Hoje</span>
                                <span className="font-semibold text-gray-900">
                                    {metrics.statsUltimos7Dias?.[metrics.statsUltimos7Dias.length - 1]?.criados || 0}
                                </span>
                            </div>
                        </div>
                    </div>

                    {/* Actions Card */}
                    <div className="card bg-gradient-to-br from-gray-50 to-gray-100">
                        <div className="flex items-center mb-4">
                            <div className="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center mr-3">
                                <svg className="w-6 h-6 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
                                </svg>
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900">Ações Rápidas</h3>
                        </div>
                        <div className="space-y-2">
                            <button 
                                onClick={() => navigate('/admin/tecnicos')}
                                className="w-full btn-secondary text-sm py-2 text-left flex items-center"
                            >
                                <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                                </svg>
                                Gerenciar Técnicos
                            </button>
                            <button 
                                onClick={() => navigate('/admin/clientes')}
                                className="w-full btn-secondary text-sm py-2 text-left flex items-center"
                            >
                                <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                                </svg>
                                Ver Clientes
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default DashboardAdminPage;