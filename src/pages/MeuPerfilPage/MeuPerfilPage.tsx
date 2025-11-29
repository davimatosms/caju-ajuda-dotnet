import React, { useState, useEffect } from 'react';
import PerfilService from 'services/PerfilService';

function MeuPerfilPage() {
    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const [senhaAtual, setSenhaAtual] = useState('');
    const [novaSenha, setNovaSenha] = useState('');
    const [isSavingSenha, setIsSavingSenha] = useState(false);
    const [errorSenha, setErrorSenha] = useState('');
    const [successSenha, setSuccessSenha] = useState('');

    // Esconder scroll do body quando o componente montar
    useEffect(() => {
        // Não esconde mais o overflow do body
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, []);

    useEffect(() => {
        const fetchPerfil = async () => {
            try {
                const data = await PerfilService.getPerfil();
                setNome(data.nome);
                setEmail(data.email);
            } catch (err) {
                setError("Não foi possível carregar os dados do perfil.");
            } finally {
                setIsLoading(false);
            }
        };
        fetchPerfil();
    }, []);

    const handleUpdatePerfil = async (event: React.FormEvent) => {
        event.preventDefault();
        setIsLoading(true);
        setError('');
        setSuccess('');
        try {
            await PerfilService.updatePerfil({ Nome: nome, Email: email });
            setSuccess("Perfil atualizado com sucesso!");
        } catch (err: any) {
            setError(err.response?.data?.message || "Erro ao atualizar o perfil.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleUpdateSenha = async (event: React.FormEvent) => {
        event.preventDefault();
        setIsSavingSenha(true);
        setErrorSenha('');
        setSuccessSenha('');
        try {
            await PerfilService.updateSenha({ SenhaAtual: senhaAtual, NovaSenha: novaSenha });
            setSuccessSenha("Senha alterada com sucesso!");
            setSenhaAtual('');
            setNovaSenha('');
        } catch (err: any) {
            setErrorSenha(err.response?.data?.message || "Erro ao alterar a senha.");
        } finally {
            setIsSavingSenha(false);
        }
    };

    if (isLoading && !nome) { 
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <svg className="animate-spin h-12 w-12 text-primary-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <p className="text-gray-600">Carregando perfil...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            {/* Header */}
            <div className="flex-shrink-0 bg-white border-b border-gray-200 px-4 py-6">
                <div className="w-full mx-auto px-4">
                    <h1 className="text-3xl font-bold text-gray-900 mb-1">Meu Perfil</h1>
                    <p className="text-gray-600">Gerencie suas informações pessoais e senha</p>
                </div>
            </div>

            <div className="flex-1 py-6">
                <div className="w-full mx-auto px-4">
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Profile Info Sidebar (33% em telas grandes) */}
                    <div className="lg:col-span-1 flex flex-col gap-6">
                        <div className="card text-center flex-shrink-0">
                            <div className="w-24 h-24 bg-gradient-to-br from-primary-500 to-primary-600 rounded-full flex items-center justify-center mx-auto mb-4">
                                <span className="text-white font-bold text-4xl">
                                    {nome.charAt(0).toUpperCase()}
                                </span>
                            </div>
                            <h2 className="text-xl font-bold text-gray-900 mb-1">{nome}</h2>
                            <p className="text-sm text-gray-600">{email}</p>
                            <div className="mt-4 pt-4 border-t">
                                <div className="text-sm text-gray-600">
                                    <p className="mb-2">
                                        <span className="font-semibold">Tipo de Conta:</span> Cliente
                                    </p>
                                    <p>
                                        <span className="font-semibold">Membro desde:</span><br />
                                        {new Date().toLocaleDateString('pt-BR')}
                                    </p>
                                </div>
                            </div>
                        </div>

                        {/* Security Tips */}
                        <div className="card bg-blue-50 border-blue-200 flex-shrink-0">
                            <div className="flex items-start">
                                <svg className="w-6 h-6 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                                </svg>
                                <div className="ml-3">
                                    <h4 className="text-sm font-semibold text-blue-900 mb-2">Dicas de Segurança</h4>
                                    <ul className="space-y-1 text-sm text-blue-700 list-disc list-inside">
                                        <li>Use uma senha forte com letras, números e símbolos</li>
                                        <li>Não compartilhe sua senha com outras pessoas</li>
                                        <li>Altere sua senha regularmente</li>
                                        <li>Evite usar a mesma senha em diferentes serviços</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Forms (67% em telas grandes) */}
                    <div className="lg:col-span-2 flex flex-col gap-6">
                        {/* Personal Data Form */}
                        <div className="card flex-shrink-0">
                            <h3 className="text-lg font-semibold text-gray-900 mb-6">Dados Pessoais</h3>
                            <form onSubmit={handleUpdatePerfil} className="space-y-5">
                                <div>
                                    <label htmlFor="nome" className="block text-sm font-medium text-gray-700 mb-2">
                                        Nome Completo
                                    </label>
                                    <input
                                        id="nome"
                                        type="text"
                                        value={nome}
                                        onChange={(e) => setNome(e.target.value)}
                                        required
                                        className="input-field"
                                    />
                                </div>

                                <div>
                                    <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                                        E-mail
                                    </label>
                                    <input
                                        id="email"
                                        type="email"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required
                                        className="input-field"
                                    />
                                </div>

                                {error && (
                                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                                        {error}
                                    </div>
                                )}

                                {success && (
                                    <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg text-sm">
                                        {success}
                                    </div>
                                )}

                                <button
                                    type="submit"
                                    disabled={isLoading}
                                    className="w-full btn-primary py-3 disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    {isLoading ? 'Salvando...' : 'Salvar Alterações'}
                                </button>
                            </form>
                        </div>

                        {/* Password Form */}
                        <div className="card flex-shrink-0">
                            <h3 className="text-lg font-semibold text-gray-900 mb-6">Alterar Senha</h3>
                            <form onSubmit={handleUpdateSenha} className="space-y-5">
                                <div>
                                    <label htmlFor="senhaAtual" className="block text-sm font-medium text-gray-700 mb-2">
                                        Senha Atual
                                    </label>
                                    <input
                                        id="senhaAtual"
                                        type="password"
                                        value={senhaAtual}
                                        onChange={(e) => setSenhaAtual(e.target.value)}
                                        required
                                        className="input-field"
                                        placeholder="Digite sua senha atual"
                                    />
                                </div>

                                <div>
                                    <label htmlFor="novaSenha" className="block text-sm font-medium text-gray-700 mb-2">
                                        Nova Senha
                                    </label>
                                    <input
                                        id="novaSenha"
                                        type="password"
                                        value={novaSenha}
                                        onChange={(e) => setNovaSenha(e.target.value)}
                                        required
                                        minLength={6}
                                        className="input-field"
                                        placeholder="Digite sua nova senha (mín. 6 caracteres)"
                                    />
                                    <p className="mt-1 text-xs text-gray-500">A senha deve ter pelo menos 6 caracteres</p>
                                </div>

                                {errorSenha && (
                                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                                        {errorSenha}
                                    </div>
                                )}

                                {successSenha && (
                                    <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg text-sm">
                                        {successSenha}
                                    </div>
                                )}

                                <button
                                    type="submit"
                                    disabled={isSavingSenha}
                                    className="w-full btn-primary py-3 disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    {isSavingSenha ? 'Alterando...' : 'Alterar Senha'}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </div>
    );
}

export default MeuPerfilPage;