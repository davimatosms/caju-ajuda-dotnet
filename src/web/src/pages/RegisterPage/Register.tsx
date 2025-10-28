import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import CajuLogoInline from '../../components/UI/CajuLogoInline';

function RegisterPage() {
    const navigate = useNavigate();
    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [isError, setIsError] = useState(false);

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        setIsLoading(true);
        setMessage('');
        setIsError(false);

        try {
            await AuthService.register({ Nome: nome, Email: email, Senha: senha });
            setMessage('Registro realizado com sucesso! Redirecionando para o login...');
            
            setTimeout(() => {
                navigate('/login');
            }, 3000);

        } catch (error: any) {
            setIsError(true);
            if (error.response && error.response.data) {
                setMessage(error.response.data.message || 'Ocorreu um erro ao tentar registar.');
            } else {
                setMessage('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-primary-50 via-white to-primary-50 flex items-center justify-center p-4">
            <div className="w-full max-w-md">
                {/* Logo */}
                <div className="text-center mb-8">
                    <Link to="/" className="inline-block">
                        <CajuLogoInline width={160} height={56} />
                    </Link>
                </div>

                {/* Card de Registro */}
                <div className="card shadow-xl">
                    <div className="mb-6">
                        <h2 className="text-2xl font-bold text-gray-900 text-center mb-2">Criar Conta</h2>
                        <p className="text-gray-600 text-center">Preencha os dados para começar</p>
                    </div>

                    <form onSubmit={handleSubmit} className="space-y-5">
                        {/* Nome Input */}
                        <div>
                            <label htmlFor="nome" className="block text-sm font-medium text-gray-700 mb-2">
                                Nome Completo
                            </label>
                            <input
                                type="text"
                                id="nome"
                                value={nome}
                                onChange={(e) => setNome(e.target.value)}
                                required
                                disabled={isLoading}
                                className="input-field"
                                placeholder="Seu nome completo"
                            />
                        </div>

                        {/* Email Input */}
                        <div>
                            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                                E-mail
                            </label>
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                disabled={isLoading}
                                className="input-field"
                                placeholder="seu@email.com"
                            />
                        </div>

                        {/* Senha Input */}
                        <div>
                            <label htmlFor="senha" className="block text-sm font-medium text-gray-700 mb-2">
                                Senha
                            </label>
                            <input
                                type="password"
                                id="senha"
                                value={senha}
                                onChange={(e) => setSenha(e.target.value)}
                                minLength={6}
                                required
                                disabled={isLoading}
                                className="input-field"
                                placeholder="Mínimo 6 caracteres"
                            />
                            <p className="mt-1 text-xs text-gray-500">A senha deve ter pelo menos 6 caracteres</p>
                        </div>

                        {/* Mensagem de Sucesso/Erro */}
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

                        {/* Botão de Submit */}
                        <button
                            type="submit"
                            disabled={isLoading}
                            className="w-full btn-primary py-3 text-base font-semibold disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
                        >
                            {isLoading ? (
                                <>
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Criando conta...
                                </>
                            ) : (
                                'Criar Conta'
                            )}
                        </button>
                    </form>

                    {/* Links Adicionais */}
                    <div className="mt-6 text-center space-y-3">
                        <p className="text-sm text-gray-600">
                            Já tem uma conta?{' '}
                            <Link to="/login" className="text-primary-600 hover:text-primary-700 font-semibold hover:underline">
                                Faça login
                            </Link>
                        </p>
                        <Link to="/" className="inline-flex items-center text-sm text-gray-500 hover:text-gray-700 transition-colors">
                            <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                            </svg>
                            Voltar para a página inicial
                        </Link>
                    </div>
                </div>

                {/* Termos de Uso */}
                <div className="mt-6 text-center">
                    <p className="text-xs text-gray-500">
                        Ao criar uma conta, você concorda com nossos <br />
                        <span className="text-primary-600 hover:underline cursor-pointer">Termos de Uso</span> e{' '}
                        <span className="text-primary-600 hover:underline cursor-pointer">Política de Privacidade</span>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default RegisterPage;