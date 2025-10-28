import React from 'react';
import { Link } from 'react-router-dom';

function AdminHomePage() {
    // Futuramente, podemos pegar o nome do admin logado para personalizar a saudação
    const adminName = "Admin";

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-orange-50 to-gray-50 py-12">
            <div className="container mx-auto px-4 max-w-6xl">
                {/* Welcome Header */}
                <div className="text-center mb-12 animate-fade-in">
                    <h1 className="text-4xl font-bold text-gray-900 mb-4">
                        Bem-vindo, <span className="text-primary-600">{adminName}</span>! 👋
                    </h1>
                    <p className="text-xl text-gray-600">
                        Selecione uma das opções abaixo para gerenciar o sistema.
                    </p>
                </div>

                {/* Navigation Grid */}
                <div className="grid md:grid-cols-3 gap-6 mb-8">
                    {/* Dashboard Card */}
                    <Link 
                        to="/admin/dashboard" 
                        className="group card hover:shadow-2xl hover:scale-105 transition-all duration-300 bg-gradient-to-br from-blue-500 to-blue-600 text-white border-0"
                    >
                        <div className="flex flex-col items-center text-center">
                            <div className="w-20 h-20 bg-white bg-opacity-20 rounded-full flex items-center justify-center mb-4 group-hover:bg-opacity-30 transition-all">
                                <svg className="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                                </svg>
                            </div>
                            <h2 className="text-2xl font-bold mb-2">Ver Dashboard</h2>
                            <p className="text-blue-100">
                                Acesse as métricas e os gráficos para uma visão geral do suporte.
                            </p>
                        </div>
                        <div className="mt-4 flex items-center justify-center text-sm font-medium group-hover:translate-x-2 transition-transform">
                            Acessar
                            <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                            </svg>
                        </div>
                    </Link>

                    {/* Técnicos Card */}
                    <Link 
                        to="/admin/tecnicos" 
                        className="group card hover:shadow-2xl hover:scale-105 transition-all duration-300 bg-gradient-to-br from-primary-500 to-primary-600 text-white border-0"
                    >
                        <div className="flex flex-col items-center text-center">
                            <div className="w-20 h-20 bg-white bg-opacity-20 rounded-full flex items-center justify-center mb-4 group-hover:bg-opacity-30 transition-all">
                                <svg className="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                                </svg>
                            </div>
                            <h2 className="text-2xl font-bold mb-2">Gerenciar Técnicos</h2>
                            <p className="text-orange-100">
                                Adicione, edite e gerencie as contas da sua equipe de suporte técnico.
                            </p>
                        </div>
                        <div className="mt-4 flex items-center justify-center text-sm font-medium group-hover:translate-x-2 transition-transform">
                            Acessar
                            <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                            </svg>
                        </div>
                    </Link>

                    {/* Clientes Card */}
                    <Link 
                        to="/admin/clientes" 
                        className="group card hover:shadow-2xl hover:scale-105 transition-all duration-300 bg-gradient-to-br from-purple-500 to-purple-600 text-white border-0"
                    >
                        <div className="flex flex-col items-center text-center">
                            <div className="w-20 h-20 bg-white bg-opacity-20 rounded-full flex items-center justify-center mb-4 group-hover:bg-opacity-30 transition-all">
                                <svg className="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                                </svg>
                            </div>
                            <h2 className="text-2xl font-bold mb-2">Gerenciar Clientes</h2>
                            <p className="text-purple-100">
                                Visualize e administre as contas de todos os clientes do sistema.
                            </p>
                        </div>
                        <div className="mt-4 flex items-center justify-center text-sm font-medium group-hover:translate-x-2 transition-transform">
                            Acessar
                            <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                            </svg>
                        </div>
                    </Link>
                </div>

                {/* Quick Stats Section */}
                <div className="grid md:grid-cols-4 gap-4">
                    <div className="card bg-white border-l-4 border-blue-500">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600 mb-1">Sistema</p>
                                <p className="text-2xl font-bold text-gray-900">Online</p>
                            </div>
                            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
                                <div className="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
                            </div>
                        </div>
                    </div>

                    <div className="card bg-white border-l-4 border-primary-500">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600 mb-1">Versão</p>
                                <p className="text-2xl font-bold text-gray-900">2.0</p>
                            </div>
                            <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <div className="card bg-white border-l-4 border-purple-500">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600 mb-1">Uptime</p>
                                <p className="text-2xl font-bold text-gray-900">99.9%</p>
                            </div>
                            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                                </svg>
                            </div>
                        </div>
                    </div>

                    <div className="card bg-white border-l-4 border-green-500">
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600 mb-1">Performance</p>
                                <p className="text-2xl font-bold text-gray-900">Ótima</p>
                            </div>
                            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                                <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AdminHomePage;