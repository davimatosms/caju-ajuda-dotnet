import React, { useState, useEffect } from 'react';
import ChamadoService, { Chamado } from '../../services/ChamadoService';
import styles from './DashboardPage.module.css';
import { useNavigate } from 'react-router-dom';

function DashboardPage() {
    const [chamados, setChamados] = useState<Chamado[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchChamados = async () => {
            try {
                // A resposta da API agora é um objeto, não um array direto
                const response: any = await ChamadoService.getMeusChamados();
                
                // MUDANÇA AQUI: Extraímos o array da propriedade '$values'
                if (response && response.$values) {
                    setChamados(response.$values);
                } else {
                    // Se não vier no formato esperado, tratamos como lista vazia
                    setChamados([]); 
                }

            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar seus chamados.");
                }
            } finally {
                setIsLoading(false);
            }
        };

        fetchChamados();
    }, [navigate]);

    const getStatusClass = (status: string) => {
        switch (status) {
            case 'ABERTO': return styles.statusAberto;
            case 'EM_ANDAMENTO': return styles.statusAndamento;
            case 'PENDENTE': return styles.statusPendente;
            case 'RESOLVIDO': return styles.statusResolvido;
            default: return '';
        }
    };

    if (isLoading) {
        return <div className={styles.loading}>Carregando chamados...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }

    return (
        <div className={styles.dashboardContainer}>
            <div className={styles.header}>
                <h1>Meus Chamados</h1>
                <button 
                    className={styles.newTicketButton} 
                    onClick={() => navigate('/chamados/novo')}
                >
                    Abrir Novo Chamado
                </button>
            </div>

            {chamados.length === 0 ? (
                <div className={styles.noTickets}>Você ainda não abriu nenhum chamado.</div>
            ) : (
                <table className={styles.chamadosTable}>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Título</th>
                            <th>Status</th>
                            <th>Prioridade</th>
                            <th>Data de Abertura</th>
                        </tr>
                    </thead>
                    <tbody>
                        {chamados.map(chamado => (
                            <tr key={chamado.id}>
                                <td>#{chamado.id}</td>
                                <td>{chamado.titulo}</td>
                                <td>
                                    <span className={`${styles.statusBadge} ${getStatusClass(chamado.status)}`}>
                                        {chamado.status.replace('_', ' ')}
                                    </span>
                                </td>
                                <td>{chamado.prioridade}</td>
                                <td>{new Date(chamado.dataCriacao).toLocaleDateString('pt-BR')}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default DashboardPage;