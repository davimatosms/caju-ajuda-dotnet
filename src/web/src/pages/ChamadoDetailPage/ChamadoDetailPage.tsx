import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ChamadoService, { ChamadoDetail, Mensagem, Anexo } from '../../services/ChamadoService';
import signalRService from '../../services/signalRService';
import {
    TicketHeader,
    TicketChatContainer,
    ChatMessage,
    MessageInput,
    TicketAttachmentList,
    TicketStatusBadge,
    TicketPriorityBadge,
    type TicketHeaderData,
    type ChatMessageData,
    type AttachmentData
} from '../../components/Ticket';

function ChamadoDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const [chamado, setChamado] = useState<ChamadoDetail | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isSending, setIsSending] = useState(false);
    const [isUploadingFile, setIsUploadingFile] = useState(false);
    const [uploadSuccess, setUploadSuccess] = useState<string | null>(null);
    const [anexos, setAnexos] = useState<Anexo[]>([]);

    // Esconder scroll do body quando o componente montar
    useEffect(() => {
        // Não esconde mais o overflow do body
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, []);

    useEffect(() => {
        if (!id) return;

        const fetchChamado = async () => {
            try {
                const data = await ChamadoService.getChamadoById(Number(id));
                console.log('🔍 [DEBUG] Dados recebidos do backend:', data);
                console.log('🔍 [DEBUG] Mensagens:', data.mensagens);
                console.log('🔍 [DEBUG] É array?:', Array.isArray(data.mensagens));
                console.log('🔍 [DEBUG] Quantidade de mensagens:', 
                    Array.isArray(data.mensagens) 
                        ? data.mensagens.length 
                        : (data.mensagens as any)?.$values?.length ?? 0
                );
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

        const fetchAnexos = async () => {
            try {
                const anexosData = await ChamadoService.getAnexosByChamado(Number(id));
                setAnexos(anexosData);
            } catch (err) {
                console.error("Erro ao carregar anexos:", err);
            }
        };

        fetchChamado();
        fetchAnexos();
    }, [id, navigate]);

    // 🔥 SignalR - Conexão em tempo real para receber novas mensagens
    useEffect(() => {
        if (!id) return;

        const token = localStorage.getItem('user_token'); // Corrigido: era 'token', agora 'user_token'
        if (!token) {
            console.warn('[SignalR] ⚠️ Token não encontrado no localStorage');
            return;
        }

        let isSubscribed = true;

        const setupSignalR = async () => {
            try {
                console.log('[SignalR] 🚀 Iniciando setup para chamado', id);
                
                // Conectar ao SignalR
                const connection = await signalRService.connect(token);
                
                if (!connection) {
                    console.error('[SignalR] ❌ Falha ao obter conexão');
                    return;
                }
                
                console.log('[SignalR] ✅ Conexão obtida, entrando na sala...');
                
                // Entrar na sala do chamado
                await signalRService.joinRoom(Number(id));

                // Registrar listener para novas mensagens
                signalRService.onNewMessage((novaMensagemSignalR: any) => {
                    console.log('[SignalR] 🔔 Nova mensagem recebida:', novaMensagemSignalR);
                    console.log('[SignalR] 📊 Detalhes:', {
                        id: novaMensagemSignalR.id,
                        autor: novaMensagemSignalR.autorNome,
                        texto: novaMensagemSignalR.texto.substring(0, 50)
                    });

                    if (!isSubscribed) return;

                    // Atualizar o estado com a nova mensagem
                    setChamado(chamadoAnterior => {
                        if (!chamadoAnterior) return null;

                        const existentes = Array.isArray(chamadoAnterior.mensagens)
                            ? chamadoAnterior.mensagens
                            : (chamadoAnterior.mensagens?.$values ?? []);

                        // Verificar se a mensagem já existe (evitar duplicatas)
                        const jaExiste = existentes.some((msg: Mensagem) => msg.id === novaMensagemSignalR.id);
                        if (jaExiste) {
                            console.log('[SignalR] ⚠️ Mensagem já existe, ignorando duplicata');
                            return chamadoAnterior;
                        }

                        // Adicionar nova mensagem
                        const novaMensagemFormatada: Mensagem = {
                            id: novaMensagemSignalR.id,
                            texto: novaMensagemSignalR.texto,
                            dataEnvio: novaMensagemSignalR.dataEnvio,
                            autorNome: novaMensagemSignalR.autorNome,
                            autorId: novaMensagemSignalR.autorId,
                            isNotaInterna: novaMensagemSignalR.isNotaInterna,
                            lidoPeloCliente: novaMensagemSignalR.lidoPeloCliente ?? false
                        };

                        return {
                            ...chamadoAnterior,
                            mensagens: [...existentes, novaMensagemFormatada] as any
                        };
                    });
                });

                console.log('[SignalR] ✅ Setup completo para o chamado', id);
            } catch (error) {
                console.error('[SignalR] ❌ Erro ao configurar:', error);
            }
        };

        setupSignalR();

        // Cleanup ao desmontar
        return () => {
            isSubscribed = false;
            if (id) {
                signalRService.leaveRoom(Number(id));
            }
            signalRService.offNewMessage();
        };
    }, [id]);

    // Auto-scroll to bottom when messages change
    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [chamado?.mensagens]);

    const handleSendMessage = async (message: string, isInternalNote: boolean) => {
        if (!message.trim() || !id) return;

        setIsSending(true);
        try {
            const mensagemEnviada = await ChamadoService.addMensagem(Number(id), { Texto: message });

            // Atualiza o estado local adicionando a nova mensagem
            setChamado(chamadoAnterior => {
                if (!chamadoAnterior) return null;
                
                // Suporta tanto array direto quanto estrutura $values
                const existentes = Array.isArray(chamadoAnterior.mensagens)
                    ? chamadoAnterior.mensagens
                    : (chamadoAnterior.mensagens?.$values ?? []);
                
                // Converte a resposta do backend para o formato esperado
                const novaMensagemFormatada: Mensagem = {
                    id: mensagemEnviada.id,
                    texto: mensagemEnviada.texto,
                    dataEnvio: mensagemEnviada.dataEnvio,
                    autorNome: mensagemEnviada.autorNome,
                    autorId: mensagemEnviada.autorId,
                    isNotaInterna: isInternalNote,
                    lidoPeloCliente: mensagemEnviada.lidoPeloCliente ?? true
                };
                
                // Retorna no mesmo formato que veio do backend (array direto)
                return { 
                    ...chamadoAnterior, 
                    mensagens: [...existentes, novaMensagemFormatada] as any
                };
            });
        } catch (error) {
            console.error("Erro ao enviar mensagem", error);
            alert('Erro ao enviar mensagem. Tente novamente.');
        } finally {
            setIsSending(false);
        }
    };

    const uploadFile = async (files: File[]) => {
        if (!id || files.length === 0) return;

        const file = files[0]; // Pega apenas o primeiro arquivo

        // Validar tamanho (máximo 10MB)
        if (file.size > 10 * 1024 * 1024) {
            alert('O arquivo deve ter no máximo 10MB');
            return;
        }

        setIsUploadingFile(true);
        setUploadSuccess(null);
        
        try {
            await ChamadoService.uploadAnexo(Number(id), file);
            setUploadSuccess(`Arquivo "${file.name}" enviado com sucesso!`);
            
            // Recarregar lista de anexos
            const anexosData = await ChamadoService.getAnexosByChamado(Number(id));
            setAnexos(anexosData);
            
            // Limpar mensagem de sucesso após 5 segundos
            setTimeout(() => setUploadSuccess(null), 5000);
        } catch (error: any) {
            console.error("Erro ao enviar arquivo", error);
            alert(error.response?.data?.message || 'Erro ao enviar arquivo. Tente novamente.');
        } finally {
            setIsUploadingFile(false);
        }
    };

    const handleDownloadAnexo = async (anexo: AttachmentData) => {
        try {
            const blob = await ChamadoService.downloadAnexo(anexo.id);
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = anexo.nomeArquivo;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (error) {
            console.error("Erro ao baixar anexo:", error);
            alert('Erro ao baixar arquivo. Tente novamente.');
        }
    };

    // Converter dados do chamado para o formato do componente TicketHeader
    const getTicketHeaderData = (): TicketHeaderData | null => {
        if (!chamado) return null;
        
        return {
            id: chamado.id,
            titulo: chamado.titulo,
            descricao: chamado.descricao,
            status: chamado.status as any,
            prioridade: chamado.prioridade as any,
            dataCriacao: chamado.dataCriacao,
            clienteNome: chamado.nomeCliente,
            tecnicoResponsavel: chamado.nomeTecnicoResponsavel
        };
    };

    // Converter mensagens para o formato do componente ChatMessage
    const getChatMessages = (): ChatMessageData[] => {
        if (!chamado) return [];
        
        const mensagensArray = Array.isArray(chamado.mensagens) 
            ? chamado.mensagens 
            : (chamado.mensagens?.$values ?? []);
        
        return mensagensArray.map((msg: any) => ({
            id: msg.id,
            conteudo: msg.texto,
            dataEnvio: msg.dataEnvio,
            autorNome: msg.autorNome,
            autorTipo: msg.autorNome === chamado.nomeCliente ? 'CLIENTE' : 'TECNICO',
            isNotaInterna: msg.isNotaInterna,
            lidoPeloCliente: msg.lidoPeloCliente
        }));
    };

    // Converter anexos para o formato do componente TicketAttachmentList
    const getAttachments = (): AttachmentData[] => {
        return anexos.map(anexo => ({
            id: anexo.id,
            nomeArquivo: anexo.nomeArquivo,
            caminhoArquivo: '', // Backend não retorna esse campo
            dataUpload: new Date().toISOString() // Usar data atual como fallback
        }));
    };
    
    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <svg className="animate-spin h-12 w-12 text-primary-500 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <p className="text-gray-600">Carregando chamado...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="card max-w-md bg-red-50 border border-red-200">
                    <div className="flex items-center text-red-700">
                        <svg className="w-6 h-6 mr-2" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                        </svg>
                        <span>{error}</span>
                    </div>
                </div>
            </div>
        );
    }

    if (!chamado) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="card max-w-md text-center">
                    <p className="text-gray-600">Chamado não encontrado.</p>
                </div>
            </div>
        );
    }

    const ticketData = getTicketHeaderData();
    const messages = getChatMessages();
    const attachments = getAttachments();
    const currentUserName = localStorage.getItem('user_name') || chamado.nomeCliente;

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            <div className="w-full mx-auto px-4 py-4 flex-1 flex flex-col">
                {/* Header */}
                <div className="mb-4 flex-shrink-0">
                    <button
                        onClick={() => navigate('/dashboard')}
                        className="flex items-center text-gray-600 hover:text-gray-900 mb-4 transition-colors"
                    >
                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                        Voltar para Meus Chamados
                    </button>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 flex-1">
                    {/* Left Column - Chat (50% em telas grandes) */}
                    <div className="flex flex-col gap-4 max-h-[calc(100vh-8rem)] lg:max-h-[calc(100vh-6rem)]">
                        {/* Ticket Header - Componente Semântico */}
                        {ticketData && <TicketHeader ticket={ticketData} />}

                        {/* Chat Container - Componente Semântico */}
                        <div className="flex-1 min-h-0 overflow-y-auto custom-scrollbar">
                            <TicketChatContainer isLoading={false} autoScroll={true} className="h-full">
                                {messages.length === 0 ? (
                                    <div className="text-center py-12">
                                        <svg className="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                                        </svg>
                                        <p className="text-gray-500">Nenhuma mensagem ainda. Inicie a conversa!</p>
                                    </div>
                                ) : (
                                    messages.map(msg => (
                                        <ChatMessage
                                            key={msg.id}
                                            message={msg}
                                            isCurrentUser={msg.autorNome === currentUserName}
                                        />
                                    ))
                                )}
                            </TicketChatContainer>
                        </div>

                        {/* Success message for file upload */}
                        {uploadSuccess && (
                            <div className="p-3 bg-green-50 border border-green-200 rounded-lg flex items-center gap-2 flex-shrink-0">
                                <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                                <span className="text-sm text-green-700">{uploadSuccess}</span>
                            </div>
                        )}

                        {/* Message Input - Componente Semântico com Anexo Integrado */}
                        <div className="flex-shrink-0">
                            <MessageInput
                                onSendMessage={handleSendMessage}
                                onFileSelect={(file) => uploadFile([file])}
                                disabled={isSending}
                                isUploadingFile={isUploadingFile}
                                showInternalNoteOption={false}
                                placeholder="Digite sua mensagem..."
                            />
                        </div>
                    </div>

                    {/* Right Column - Details (50% em telas grandes) */}
                    <div className="flex flex-col gap-4 max-h-[calc(100vh-8rem)] lg:max-h-[calc(100vh-6rem)] overflow-y-auto custom-scrollbar">
                        {/* Details Card */}
                        <div className="card flex-shrink-0">
                            <h3 className="text-lg font-semibold text-gray-900 mb-4">Detalhes do Chamado</h3>
                            <div className="space-y-4">
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Cliente</p>
                                    <p className="text-sm text-gray-900 font-medium">{chamado.nomeCliente}</p>
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Técnico Responsável</p>
                                    <p className="text-sm text-gray-900 font-medium">
                                        {chamado.nomeTecnicoResponsavel || (
                                            <span className="text-gray-400 italic">Não atribuído</span>
                                        )}
                                    </p>
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Status</p>
                                    <TicketStatusBadge status={chamado.status as any} />
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Prioridade</p>
                                    <TicketPriorityBadge priority={chamado.prioridade as any} />
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Criado em</p>
                                    <p className="text-sm text-gray-900">{new Date(chamado.dataCriacao).toLocaleString('pt-BR')}</p>
                                </div>
                            </div>
                        </div>

                        {/* Anexos - Componente Semântico */}
                        <TicketAttachmentList
                            attachments={attachments}
                            onDownload={handleDownloadAnexo}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ChamadoDetailPage;