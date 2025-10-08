import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ChamadoService, { ChamadoDetail, Mensagem } from '../../services/ChamadoService';
import styles from './ChamadoDetailPage.module.css';
import { Button } from '../../components/UI';

function ChamadoDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [chamado, setChamado] = useState<ChamadoDetail | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    
    // NOVOS ESTADOS para o formulário de mensagem
    const [novaMensagem, setNovaMensagem] = useState('');
    const [isSending, setIsSending] = useState(false);

    useEffect(() => {
        if (!id) return;

        const fetchChamado = async () => {
            try {
                const data = await ChamadoService.getChamadoById(Number(id));
                setChamado(data);
            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar os detalhes do chamado.");
                }
            } finally {
                setIsLoading(false);
            }
        };

        fetchChamado();
    }, [id, navigate]);

    // NOVA FUNÇÃO para lidar com o envio da mensagem
    const handleSendMessage = async (event: React.FormEvent) => {
        event.preventDefault();
        if (!novaMensagem.trim() || !id) return;

        setIsSending(true);
        try {
            const mensagemEnviada = await ChamadoService.addMensagem(Number(id), { Texto: novaMensagem });
            
            // Atualiza o estado local para exibir a nova mensagem instantaneamente
            setChamado(chamadoAnterior => {
                if (!chamadoAnterior) return null;
                const novasMensagens = [...chamadoAnterior.mensagens.$values, mensagemEnviada];
                return { ...chamadoAnterior, mensagens: { $values: novasMensagens } };
            });

            setNovaMensagem(''); // Limpa o campo de texto
        } catch (error) {
            // Poderíamos adicionar uma mensagem de erro específica para o envio
            console.error("Erro ao enviar mensagem", error);
        } finally {
            setIsSending(false);
        }
    };

    const getStatusClass = (status: string) => {
        return styles.statusAberto; // Placeholder
    };
    
    if (isLoading) {
        return <div className={styles.loading}>Carregando detalhes do chamado...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }

    if (!chamado) {
        return <div className={styles.noTickets}>Chamado não encontrado.</div>;
    }

    return (
        <div className={styles.pageContainer}>
            <div className={styles.chamadoContainer}>
                {/* ... (código do header e contentGrid continua igual) ... */}
                <div className={styles.header}>
                    <h1>{chamado.titulo}</h1>
                    <p>Aberto em: {new Date(chamado.dataCriacao).toLocaleString('pt-BR')}</p>
                    <div className={styles.statusContainer}>
                        <span className={`${styles.statusBadge} ${styles.statusAberto}`}>{chamado.status}</span>
                        <span className={`${styles.statusBadge} ${styles.prioridadeUrgente}`}>{chamado.prioridade}</span>
                    </div>
                </div>

                <div className={styles.contentGrid}>
                    <div className={styles.descricao}>
                        <h3>Descrição do Problema</h3>
                        <p>{chamado.descricao}</p>
                    </div>
                    <div className={styles.metadata}>
                        <h3>Detalhes</h3>
                        <p><strong>Cliente:</strong> {chamado.nomeCliente}</p>
                        <p><strong>Técnico:</strong> {chamado.nomeTecnicoResponsavel || 'Não atribuído'}</p>
                        <p><strong>Status:</strong> {chamado.status}</p>
                        <p><strong>Prioridade:</strong> {chamado.prioridade}</p>
                    </div>
                </div>

                <div className={styles.historicoContainer}>
                    <h2>Histórico da Conversa</h2>
                    {chamado.mensagens.$values.map(msg => (
                        <div key={msg.id} className={`${styles.mensagem} ${msg.autorNome === chamado.nomeCliente ? styles.mensagemCliente : styles.mensagemTecnico}`}>
                            <p className={styles.autor}>{msg.autorNome}</p>
                            <p>{msg.texto}</p>
                            <p className={styles.data}>{new Date(msg.dataEnvio).toLocaleString('pt-BR')}</p>
                        </div>
                    ))}
                </div>

                {/* NOVO FORMULÁRIO para adicionar mensagem */}
                <form onSubmit={handleSendMessage} className={styles.addMessageForm}>
                    <textarea
                        value={novaMensagem}
                        onChange={(e) => setNovaMensagem(e.target.value)}
                        placeholder="Digite sua mensagem..."
                        className={styles.messageTextarea}
                        disabled={isSending}
                    />
                    <Button type="submit" className={styles.sendMessageButton} disabled={isSending}>
                        {isSending ? 'Enviando...' : 'Enviar Mensagem'}
                    </Button>
                </form>
            </div>
        </div>
    );
}

export default ChamadoDetailPage;