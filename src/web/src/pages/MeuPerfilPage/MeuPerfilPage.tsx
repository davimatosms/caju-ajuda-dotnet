import React, { useState, useEffect } from 'react';
import PerfilService from 'services/PerfilService';
import styles from './MeuPerfilPage.module.css'; 
import { Button } from '../../components/UI';

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
        return <div>Carregando perfil...</div>;
    }

    return (
        <div className={styles.pageContainer}>
            <h1>Meu Perfil</h1>
            
            <form onSubmit={handleUpdatePerfil} className={styles.formSection}>
                <h3>Dados Pessoais</h3>
                <div className={styles.inputGroup}>
                    <label htmlFor="nome">Nome Completo</label>
                    <input id="nome" type="text" value={nome} onChange={(e) => setNome(e.target.value)} required />
                </div>
                <div className={styles.inputGroup}>
                    <label htmlFor="email">E-mail</label>
                    <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                {error && <p style={{color: 'red'}}>{error}</p>}
                {success && <p style={{color: 'green'}}>{success}</p>}
                <Button type="submit" className={styles.submitButton} disabled={isLoading}>
                    {isLoading ? 'Salvando...' : 'Salvar Alterações'}
                </Button>
            </form>

            <form onSubmit={handleUpdateSenha} className={styles.formSection}>
                <h3>Alterar Senha</h3>
                <div className={styles.inputGroup}>
                    <label htmlFor="senhaAtual">Senha Atual</label>
                    <input id="senhaAtual" type="password" value={senhaAtual} onChange={(e) => setSenhaAtual(e.target.value)} required />
                </div>
                <div className={styles.inputGroup}>
                    <label htmlFor="novaSenha">Nova Senha</label>
                    <input id="novaSenha" type="password" value={novaSenha} onChange={(e) => setNovaSenha(e.target.value)} required minLength={6} />
                </div>
                {errorSenha && <p style={{color: 'red'}}>{errorSenha}</p>}
                {successSenha && <p style={{color: 'green'}}>{successSenha}</p>}
                <Button type="submit" className={styles.submitButton} disabled={isSavingSenha}>
                    {isSavingSenha ? 'Alterando...' : 'Alterar Senha'}
                </Button>
            </form>
        </div>
    );
}

export default MeuPerfilPage;