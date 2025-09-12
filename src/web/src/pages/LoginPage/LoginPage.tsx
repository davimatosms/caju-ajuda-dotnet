import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import styles from './LoginPage.module.css';

function LoginPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        setIsLoading(true);
        setMessage('');

        try {
            await AuthService.login({ Email: email, Senha: senha });
            
            const user = AuthService.getCurrentUser();

            // LÓGICA DE REDIRECIONAMENTO CORRIGIDA
            if (user?.role === 'ADMIN') {
                // Se for Admin, vai para a página de gerenciar técnicos
                navigate('/admin/tecnicos');
            } else if (user?.role === 'TECNICO') {
                // Se for Técnico, impede o login na web e mostra uma mensagem
                AuthService.logout(); // Desloga imediatamente
                setMessage("Técnicos devem usar a aplicação Desktop para acessar o sistema.");
            } else {
                // Se for Cliente, vai para o dashboard principal
                navigate('/');
            }

        } catch (error: any) {
            if (error.response && error.response.status === 401) {
                setMessage('E-mail ou senha inválidos.');
            } else {
                setMessage('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
            }
        } finally {
            setIsLoading(false);
        }
    };
    
    return (
        <div className={styles.loginContainer}>
            <form className={styles.loginForm} onSubmit={handleSubmit}>
                <h2>Entrar</h2>
                <div className={styles.inputGroup}>
                    <label htmlFor="email">E-mail</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        disabled={isLoading}
                    />
                </div>
                <div className={styles.inputGroup}>
                    <label htmlFor="senha">Senha</label>
                    <input
                        type="password"
                        id="senha"
                        value={senha}
                        onChange={(e) => setSenha(e.target.value)}
                        required
                        disabled={isLoading}
                    />
                </div>
                <button type="submit" className={styles.submitButton} disabled={isLoading}>
                    {isLoading ? 'Entrando...' : 'Entrar'}
                </button>
                {message && (
                    <div className={`${styles.message} ${styles.error}`}>
                        {message}
                    </div>
                )}
            </form>
        </div>
    );
}

export default LoginPage;