import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import cajuLogo from '../../assets/Caju.png';
import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5205';
const API_URL = `${API_BASE}/api`;

function VerificarEmailPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
    const [message, setMessage] = useState('');
    const hasVerified = React.useRef(false);

    useEffect(() => {
        // Evita múltiplas chamadas (React StrictMode em desenvolvimento)
        if (hasVerified.current) return;
        
        const token = searchParams.get('token');
        
        if (!token) {
            setStatus('error');
            setMessage('Token de verificação não fornecido.');
            return;
        }

        const verifyEmail = async () => {
            try {
                hasVerified.current = true;
                const response = await axios.get(`${API_URL}/auth/verify?token=${token}`);
                setStatus('success');
                setMessage(response.data?.message || 'E-mail verificado com sucesso! Redirecionando para o login...');
                setTimeout(() => navigate('/login'), 3000);
            } catch (error: any) {
                setStatus('error');
                const errorMessage = error.response?.data?.message || 'Erro ao verificar e-mail. Token inválido ou expirado.';
                setMessage(errorMessage);
            }
        };

        verifyEmail();
    }, [searchParams, navigate]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 px-4">
            <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8">
                {/* Logo */}
                <div className="text-center mb-6">
                    <img src={cajuLogo} alt="Caju Ajuda" className="h-16 w-auto mx-auto mb-4" />
                    <h1 className="text-2xl font-bold text-gray-900">Verificação de E-mail</h1>
                </div>

                {/* Status */}
                <div className="text-center">
                    {status === 'loading' && (
                        <div className="space-y-4">
                            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
                            <p className="text-gray-600">Verificando seu e-mail...</p>
                        </div>
                    )}

                    {status === 'success' && (
                        <div className="space-y-4">
                            <div className="mx-auto w-16 h-16 bg-green-100 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                            </div>
                            <div>
                                <p className="text-lg font-semibold text-green-600 mb-2">✅ Sucesso!</p>
                                <p className="text-gray-600">{message}</p>
                            </div>
                        </div>
                    )}

                    {status === 'error' && (
                        <div className="space-y-4">
                            <div className="mx-auto w-16 h-16 bg-red-100 rounded-full flex items-center justify-center">
                                <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </div>
                            <div>
                                <p className="text-lg font-semibold text-red-600 mb-2">❌ Erro</p>
                                <p className="text-gray-600 mb-4">{message}</p>
                                <button
                                    onClick={() => navigate('/register')}
                                    className="px-6 py-2 bg-primary-500 hover:bg-primary-600 text-white font-medium rounded-lg transition-colors"
                                >
                                    Voltar para Registro
                                </button>
                            </div>
                        </div>
                    )}
                </div>

                {/* Link para login */}
                {status === 'success' && (
                    <div className="mt-6 text-center">
                        <button
                            onClick={() => navigate('/login')}
                            className="text-primary-600 hover:text-primary-700 font-medium"
                        >
                            Ir para o Login agora
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

export default VerificarEmailPage;
