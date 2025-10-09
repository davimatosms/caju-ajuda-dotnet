import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import styles from './LoginPage.module.css';
import { Button } from '../../components/UI';
import PageLayout from '../../components/PageLayout/PageLayout';
import CajuLogoInline from '../../components/UI/CajuLogoInline';

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

            if (user?.role === 'ADMIN') {
                navigate('/admin');
            } else if (user?.role === 'TECNICO') {
                AuthService.logout();
                setMessage("Técnicos devem usar a aplicação Desktop.");
            } else {
                navigate('/dashboard');
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
        <PageLayout>
            <div className="pageLogoWrap logoWrap">
                <CajuLogoInline width={160} height={56} />
            </div>
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
                <Button type="submit" className={styles.submitButton} disabled={isLoading}>
                    {isLoading ? 'Entrando...' : 'Entrar'}
                </Button>
                {message && (
                    <div className={`${styles.message} ${styles.error}`}>{message}</div>
                )}
                 <Link to="/" className={styles.backLink}>&larr; Voltar para a página inicial</Link>
            </form>
        </PageLayout>
    );
}

export default LoginPage;