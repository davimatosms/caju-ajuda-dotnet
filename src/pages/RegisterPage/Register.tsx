import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import styles from './Register.module.css';
import { Button } from '../../components/UI';
import PageLayout from '../../components/PageLayout/PageLayout';
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
        <PageLayout variant="centered">
            <div className="pageLogoWrap logoWrap">
                <CajuLogoInline width={160} height={56} />
            </div>
            <form className={styles.registerForm} onSubmit={handleSubmit}>
            <h2>Criar Conta</h2>
            <div className={styles.inputGroup}>
                <label htmlFor="nome">Nome Completo</label>
                <input
                    type="text"
                    id="nome"
                    value={nome}
                    onChange={(e) => setNome(e.target.value)}
                    required
                    disabled={isLoading}
                />
            </div>
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
                    minLength={6}
                    required
                    disabled={isLoading}
                />
            </div>
            <Button type="submit" className={styles.submitButton} disabled={isLoading}>
                {isLoading ? 'Aguarde...' : 'Criar Conta'}
            </Button>
            {message && (
                <div className={`${styles.message} ${isError ? styles.error : styles.success}`}>
                    {message}
                </div>
            )}
            <Link to="/" className={styles.backLink}>&larr; Voltar para a página inicial</Link>
            </form>
        </PageLayout>
    );
}

export default RegisterPage;