import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ChamadoService from '../../services/ChamadoService';

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
            setMessage('Chamado criado com sucesso! Redirecionando...');

            setTimeout(() => {
                navigate('/dashboard'); 
            }, 2000);

        } catch (error) {
            setIsError(true);
            setMessage('Ocorreu um erro ao criar o chamado. Tente novamente.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 py-8">
            <div className="container mx-auto px-4 max-w-3xl">
                {/* Header */}
                <div className="mb-8">
                    <button
                        onClick={() => navigate('/dashboard')}
                        className="flex items-center text-gray-600 hover:text-gray-900 mb-4 transition-colors"
                    >
                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                        Voltar para Meus Chamados
                    </button>
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">Abrir Novo Chamado</h1>
                    <p className="text-gray-600">Descreva seu problema e nossa IA irá categorizá-lo automaticamente</p>
                </div>

                {/* Info Card */}
                <div className="card mb-6 bg-blue-50 border-blue-200">
                    <div className="flex items-start">
                        <div className="flex-shrink-0">
                            <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div className="ml-3">
                            <h3 className="text-sm font-semibold text-blue-900 mb-1">Como funciona a IA?</h3>
                            <p className="text-sm text-blue-700">
                                Nossa inteligência artificial analisa a descrição do seu problema e automaticamente:
                            </p>
                            <ul className="mt-2 space-y-1 text-sm text-blue-700 list-disc list-inside">
                                <li>Define a prioridade do chamado (Baixa, Média, Alta ou Crítica)</li>
                                <li>Sugere uma possível solução antes do técnico assumir</li>
                                <li>Categoriza o tipo de problema</li>
                            </ul>
                        </div>
                    </div>
                </div>

                {/* Form Card */}
                <div className="card">
                    <form onSubmit={handleSubmit} className="space-y-6">
                        {/* Título */}
                        <div>
                            <label htmlFor="titulo" className="block text-sm font-semibold text-gray-700 mb-2">
                                Título do Chamado *
                            </label>
                            <input
                                type="text"
                                id="titulo"
                                value={titulo}
                                onChange={(e) => setTitulo(e.target.value)}
                                required
                                maxLength={150}
                                disabled={isLoading}
                                className="input-field"
                                placeholder="Ex: Problema ao acessar o sistema"
                            />
                            <p className="mt-1 text-xs text-gray-500">{titulo.length}/150 caracteres</p>
                        </div>

                        {/* Descrição */}
                        <div>
                            <label htmlFor="descricao" className="block text-sm font-semibold text-gray-700 mb-2">
                                Descrição Detalhada do Problema *
                            </label>
                            <textarea
                                id="descricao"
                                value={descricao}
                                onChange={(e) => setDescricao(e.target.value)}
                                required
                                disabled={isLoading}
                                rows={8}
                                className="input-field resize-none"
                                placeholder="Descreva o problema com o máximo de detalhes possível. Quanto mais informações você fornecer, melhor nossa IA poderá analisar e categorizar seu chamado.

Exemplos de informações úteis:
- O que você estava fazendo quando o problema ocorreu?
- Qual mensagem de erro apareceu (se houver)?
- Este problema acontece sempre ou apenas às vezes?
- Você já tentou alguma solução?"
                            />
                            <p className="mt-1 text-xs text-gray-500">
                                Seja específico para que nossa IA possa categorizar melhor seu chamado
                            </p>
                        </div>

                        {/* Message */}
                        {message && (
                            <div className={`px-4 py-3 rounded-lg flex items-start animate-fade-in ${
                                isError 
                                    ? 'bg-red-50 border border-red-200 text-red-700' 
                                    : 'bg-green-50 border border-green-200 text-green-700'
                            }`}>
                                <svg className="w-5 h-5 mr-2 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                                    {isError ? (
                                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                                    ) : (
                                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                                    )}
                                </svg>
                                <span className="text-sm">{message}</span>
                            </div>
                        )}

                        {/* Buttons */}
                        <div className="flex gap-4 pt-4">
                            <button
                                type="button"
                                onClick={() => navigate('/dashboard')}
                                disabled={isLoading}
                                className="flex-1 btn-secondary py-3"
                            >
                                Cancelar
                            </button>
                            <button
                                type="submit"
                                disabled={isLoading}
                                className="flex-1 btn-primary py-3 flex items-center justify-center disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                {isLoading ? (
                                    <>
                                        <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                        </svg>
                                        Criando chamado...
                                    </>
                                ) : (
                                    <>
                                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                                        </svg>
                                        Enviar Chamado
                                    </>
                                )}
                            </button>
                        </div>
                    </form>
                </div>

                {/* Tips Card */}
                <div className="mt-6 card bg-gray-50">
                    <h3 className="text-sm font-semibold text-gray-900 mb-3">💡 Dicas para um atendimento mais rápido:</h3>
                    <ul className="space-y-2 text-sm text-gray-700">
                        <li className="flex items-start">
                            <span className="text-primary-500 mr-2">•</span>
                            <span>Seja claro e objetivo na descrição do problema</span>
                        </li>
                        <li className="flex items-start">
                            <span className="text-primary-500 mr-2">•</span>
                            <span>Inclua capturas de tela ou códigos de erro (você poderá anexar após criar o chamado)</span>
                        </li>
                        <li className="flex items-start">
                            <span className="text-primary-500 mr-2">•</span>
                            <span>Mencione se o problema impede completamente seu trabalho</span>
                        </li>
                        <li className="flex items-start">
                            <span className="text-primary-500 mr-2">•</span>
                            <span>Verifique se já existe um chamado similar antes de criar um novo</span>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    );
}

export default NovoChamadoPage;