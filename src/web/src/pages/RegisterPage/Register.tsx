import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from 'services/AuthService';  
import styles from './Register.module.css'; 

function Register() {
    // Hook para navegar entre as rotas após uma ação
    const navigate = useNavigate();

    // Estados para controlar os campos do formulário
    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');

    // Estados para controlar o feedback ao usuário
    const [isLoading, setIsLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [isError, setIsError] = useState(false);

    // Função chamada ao submeter o formulário
    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        
        setIsLoading(true);
        setMessage('');
        setIsError(false);

        try {
            // Chama o serviço de autenticação que criamos
            await AuthService.register({ Nome: nome, Email: email, Senha: senha });

            setMessage('Registro realizado com sucesso! Verifique seu e-mail para ativar a conta.');
            
            // Após 3 segundos, redireciona o usuário para a página de login
            setTimeout(() => {
                navigate('/login');
            }, 3000);

        } catch (error: any) {
            // Define a mensagem de erro para ser exibida ao usuário
            setIsError(true);
            if (error.response && error.response.data) {
                // Se a API retornar uma mensagem de erro específica, a usamos
                setMessage(error.response.data.message || 'Ocorreu um erro ao tentar registar.');
            } else {
                setMessage('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
            }
        } finally {
            // Garante que o estado de loading seja desativado ao final da requisição
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.registerContainer}>
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

                <button type="submit" className={styles.submitButton} disabled={isLoading}>
                    {isLoading ? 'Aguarde...' : 'Criar Conta'}
                </button>

                {/* Exibe a mensagem de sucesso ou erro, se houver */}
                {message && (
                    <div className={`${styles.message} ${isError ? styles.error : styles.success}`}>
                        {message}
                    </div>
                )}
            </form>
        </div>
    );
}

export default Register;