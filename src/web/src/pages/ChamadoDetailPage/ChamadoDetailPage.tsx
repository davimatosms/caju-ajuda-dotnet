import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ChamadoService, { ChamadoDetail, Mensagem, Anexo } from '../../services/ChamadoService';

function ChamadoDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const [chamado, setChamado] = useState<ChamadoDetail | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [novaMensagem, setNovaMensagem] = useState('');
    const [isSending, setIsSending] = useState(false);
    const [isUploadingFile, setIsUploadingFile] = useState(false);
    const [uploadSuccess, setUploadSuccess] = useState<string | null>(null);
    const [anexos, setAnexos] = useState<Anexo[]>([]);
    const [isLoadingAnexos, setIsLoadingAnexos] = useState(false);
    const [isDragging, setIsDragging] = useState(false);

    // Esconder scroll do body quando o componente montar
    useEffect(() => {
        document.body.style.overflow = 'hidden';
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
            setIsLoadingAnexos(true);
            try {
                const anexosData = await ChamadoService.getAnexosByChamado(Number(id));
                setAnexos(anexosData);
            } catch (err) {
                console.error("Erro ao carregar anexos:", err);
            } finally {
                setIsLoadingAnexos(false);
            }
        };

        fetchChamado();
        fetchAnexos();
    }, [id, navigate]);

    // Auto-scroll to bottom when messages change
    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [chamado?.mensagens]);

    const handleSendMessage = async (event: React.FormEvent) => {
        event.preventDefault();
        if (!novaMensagem.trim() || !id) return;

        setIsSending(true);
        try {
            const mensagemEnviada = await ChamadoService.addMensagem(Number(id), { Texto: novaMensagem });

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
                    isNotaInterna: mensagemEnviada.isNotaInterna,
                    lidoPeloCliente: mensagemEnviada.lidoPeloCliente ?? true
                };
                
                // Retorna no mesmo formato que veio do backend (array direto)
                return { 
                    ...chamadoAnterior, 
                    mensagens: [...existentes, novaMensagemFormatada] as any
                };
            });

            setNovaMensagem('');
        } catch (error) {
            console.error("Erro ao enviar mensagem", error);
            alert('Erro ao enviar mensagem. Tente novamente.');
        } finally {
            setIsSending(false);
        }
    };

    const uploadFile = async (file: File) => {
        if (!id) return;

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

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            await uploadFile(file);
            event.target.value = ''; // Limpar o input
        }
    };

    const handleDragEnter = (e: React.DragEvent) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(true);
    };

    const handleDragLeave = (e: React.DragEvent) => {
        e.preventDefault();
        e.stopPropagation();
        // Só desativa se estiver saindo do container principal
        if (e.currentTarget === e.target) {
            setIsDragging(false);
        }
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
        e.stopPropagation();
    };

    const handleDrop = async (e: React.DragEvent) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);

        const files = Array.from(e.dataTransfer.files);
        if (files.length > 0) {
            await uploadFile(files[0]); // Upload apenas o primeiro arquivo
        }
    };

    const handleDownloadAnexo = async (anexoId: number, nomeArquivo: string) => {
        try {
            const blob = await ChamadoService.downloadAnexo(anexoId);
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = nomeArquivo;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (error) {
            console.error("Erro ao baixar anexo:", error);
            alert('Erro ao baixar arquivo. Tente novamente.');
        }
    };

    const isImageFile = (tipoArquivo: string) => {
        return tipoArquivo.startsWith('image/');
    };

    const getFileIcon = (tipoArquivo: string) => {
        if (tipoArquivo.startsWith('image/')) {
            return (
                <svg className="w-8 h-8 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
            );
        } else if (tipoArquivo.includes('pdf')) {
            return (
                <svg className="w-8 h-8 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                </svg>
            );
        } else {
            return (
                <svg className="w-8 h-8 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
            );
        }
    };

    const getStatusBadge = (status: string) => {
        const badges: { [key: string]: { class: string; text: string } } = {
            'ABERTO': { class: 'badge badge-info', text: 'Aberto' },
            'EM_ANDAMENTO': { class: 'badge badge-warning', text: 'Em Andamento' },
            'PENDENTE': { class: 'badge bg-purple-100 text-purple-800', text: 'Pendente' },
            'RESOLVIDO': { class: 'badge badge-success', text: 'Resolvido' },
        };
        return badges[status] || { class: 'badge', text: status };
    };

    const getPrioridadeBadge = (prioridade: string) => {
        const badges: { [key: string]: string } = {
            'BAIXA': 'badge bg-gray-100 text-gray-700',
            'MEDIA': 'badge badge-warning',
            'ALTA': 'badge badge-danger',
            'CRITICA': 'badge bg-red-600 text-white',
        };
        return badges[prioridade] || 'badge';
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

    const statusInfo = getStatusBadge(chamado.status);

    return (
        <div className="h-screen bg-gray-50 overflow-hidden flex flex-col">
            <div className="container mx-auto px-4 max-w-7xl flex-1 flex flex-col py-4">
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

                <div className="grid lg:grid-cols-3 gap-6 flex-1 overflow-hidden">
                    {/* Left Column - Chat */}
                    <div className="lg:col-span-2 flex flex-col gap-6 overflow-y-auto">
                        {/* Ticket Header Card */}
                        <div className="card flex-shrink-0">
                            <div className="flex items-start justify-between mb-4">
                                <div className="flex-1">
                                    <div className="flex items-center gap-2 mb-2">
                                        <span className="text-sm font-semibold text-primary-600">#{chamado.id}</span>
                                        <span className={statusInfo.class}>{statusInfo.text}</span>
                                        <span className={getPrioridadeBadge(chamado.prioridade)}>{chamado.prioridade}</span>
                                    </div>
                                    <h1 className="text-2xl font-bold text-gray-900 mb-2">{chamado.titulo}</h1>
                                    <p className="text-sm text-gray-500">
                                        Aberto em {new Date(chamado.dataCriacao).toLocaleString('pt-BR')}
                                    </p>
                                </div>
                            </div>

                            <div className="border-t pt-4">
                                <h3 className="text-sm font-semibold text-gray-700 mb-2">Descrição do Problema</h3>
                                <p className="text-gray-600 whitespace-pre-wrap">{chamado.descricao}</p>
                            </div>
                        </div>

                        {/* Chat Messages */}
                        <div 
                            className={`card p-0 flex flex-col relative flex-shrink-0 ${isDragging ? 'ring-4 ring-primary-500 ring-opacity-50' : ''}`}
                            style={{ height: '500px' }}
                            onDragEnter={handleDragEnter}
                            onDragLeave={handleDragLeave}
                            onDragOver={handleDragOver}
                            onDrop={handleDrop}
                        >
                            {/* Drag overlay */}
                            {isDragging && (
                                <div className="absolute inset-0 bg-primary-500 bg-opacity-10 z-10 flex items-center justify-center border-4 border-dashed border-primary-500 rounded-xl">
                                    <div className="text-center">
                                        <svg className="w-16 h-16 text-primary-500 mx-auto mb-4 animate-bounce" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                                        </svg>
                                        <p className="text-lg font-semibold text-primary-600">Solte o arquivo aqui</p>
                                        <p className="text-sm text-primary-500 mt-1">Máximo 10MB</p>
                                    </div>
                                </div>
                            )}
                            
                            <div className="px-6 py-4 border-b bg-gray-50">
                                <h2 className="text-lg font-semibold text-gray-900">Conversa</h2>
                                <p className="text-sm text-gray-600">Comunique-se com a equipe de suporte</p>
                            </div>

                            {/* Messages Area */}
                            <div className="flex-1 overflow-y-auto px-6 py-4 space-y-4">
                                {/* Normalizar mensagens para array */}
                                {(() => {
                                    // Suporta tanto array direto quanto estrutura $values
                                    const mensagensArray = Array.isArray(chamado.mensagens) 
                                        ? chamado.mensagens 
                                        : (chamado.mensagens?.$values ?? []);
                                    
                                    return mensagensArray.length === 0 && anexos.length === 0 ? (
                                    <div className="text-center py-12">
                                        <svg className="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                                        </svg>
                                        <p className="text-gray-500">Nenhuma mensagem ainda. Inicie a conversa!</p>
                                    </div>
                                ) : (
                                    <>
                                        {/* Renderizar mensagens de texto */}
                                        {mensagensArray.map((msg: any) => {
                                            const isCliente = msg.autorNome === chamado.nomeCliente;
                                            const isAI = msg.autorNome === "Assistente IA Caju";
                                            
                                            return (
                                                <div key={msg.id} className={`flex ${isCliente ? 'justify-end' : 'justify-start'}`}>
                                                    <div className={`max-w-md ${isCliente ? 'bg-primary-500 text-white' : 'bg-gray-100 text-gray-900'} rounded-lg px-4 py-3 shadow-sm`}>
                                                        <p className="text-xs font-semibold mb-1 opacity-75">
                                                            {msg.autorNome}
                                                            {isAI && (
                                                                <span className="ml-2 text-xs bg-blue-500 text-white px-1.5 py-0.5 rounded">
                                                                    IA
                                                                </span>
                                                            )}
                                                        </p>
                                                        <p className="text-sm whitespace-pre-wrap">{msg.texto}</p>
                                                        <p className="text-xs mt-1 opacity-75">
                                                            {new Date(msg.dataEnvio).toLocaleTimeString('pt-BR', { 
                                                                hour: '2-digit', 
                                                                minute: '2-digit' 
                                                            })}
                                                        </p>
                                                    </div>
                                                </div>
                                            );
                                        })}
                                    </>
                                );
                                })()}
                                <div ref={messagesEndRef} />
                            </div>

                            {/* Message Input */}
                            <div className="border-t px-6 py-4 bg-gray-50">
                                {/* Success message for file upload */}
                                {uploadSuccess && (
                                    <div className="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg flex items-center gap-2">
                                        <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                        </svg>
                                        <span className="text-sm text-green-700">{uploadSuccess}</span>
                                    </div>
                                )}
                                
                                <form onSubmit={handleSendMessage} className="flex gap-3">
                                    <textarea
                                        value={novaMensagem}
                                        onChange={(e) => setNovaMensagem(e.target.value)}
                                        placeholder="Digite sua mensagem..."
                                        disabled={isSending}
                                        rows={2}
                                        className="flex-1 input-field resize-none"
                                        onKeyDown={(e) => {
                                            if (e.key === 'Enter' && !e.shiftKey) {
                                                e.preventDefault();
                                                handleSendMessage(e);
                                            }
                                        }}
                                    />
                                    <button
                                        type="submit"
                                        disabled={isSending || !novaMensagem.trim()}
                                        className="btn-primary px-6 self-end disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                                    >
                                        {isSending ? (
                                            <svg className="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                            </svg>
                                        ) : (
                                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                                            </svg>
                                        )}
                                        Enviar
                                    </button>
                                </form>
                                
                                <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-2 mt-3">
                                    <div className="flex items-center gap-2 flex-wrap">
                                        <p className="text-xs text-gray-500">Pressione Enter para enviar, Shift+Enter para nova linha</p>
                                        <span className="hidden sm:inline text-xs text-gray-400">•</span>
                                        <p className="text-xs text-gray-400 flex items-center gap-1">
                                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                                            </svg>
                                            Arraste arquivos aqui
                                        </p>
                                    </div>
                                    
                                    {/* File upload button */}
                                    <div className="relative">
                                        <input
                                            type="file"
                                            id="fileUpload"
                                            onChange={handleFileUpload}
                                            disabled={isUploadingFile}
                                            className="hidden"
                                            accept=".pdf,.doc,.docx,.txt,.png,.jpg,.jpeg,.gif"
                                        />
                                        <label
                                            htmlFor="fileUpload"
                                            className={`inline-flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors ${
                                                isUploadingFile ? 'opacity-50 cursor-not-allowed' : ''
                                            }`}
                                        >
                                            {isUploadingFile ? (
                                                <>
                                                    <svg className="animate-spin h-4 w-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                                    </svg>
                                                    <span>Enviando...</span>
                                                </>
                                            ) : (
                                                <>
                                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                                                    </svg>
                                                    <span>Anexar arquivo</span>
                                                </>
                                            )}
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Right Column - Details */}
                    <div className="flex flex-col gap-6 overflow-y-auto">
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
                                    <span className={statusInfo.class}>{statusInfo.text}</span>
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Prioridade</p>
                                    <span className={getPrioridadeBadge(chamado.prioridade)}>{chamado.prioridade}</span>
                                </div>
                                <div>
                                    <p className="text-xs font-medium text-gray-500 uppercase mb-1">Criado em</p>
                                    <p className="text-sm text-gray-900">{new Date(chamado.dataCriacao).toLocaleString('pt-BR')}</p>
                                </div>
                            </div>
                        </div>

                        {/* Anexos Card */}
                        <div className="card flex-shrink-0">
                            <div className="flex items-center justify-between mb-4">
                                <h3 className="text-lg font-semibold text-gray-900">Anexos</h3>
                                <span className="text-xs text-gray-500">{anexos.length} arquivo{anexos.length !== 1 ? 's' : ''}</span>
                            </div>
                            
                            {isLoadingAnexos ? (
                                <div className="text-center py-4">
                                    <svg className="animate-spin h-6 w-6 text-primary-500 mx-auto" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                </div>
                            ) : anexos.length === 0 ? (
                                <div className="text-center py-8 text-gray-400">
                                    <svg className="w-12 h-12 mx-auto mb-2 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                                    </svg>
                                    <p className="text-sm">Nenhum anexo enviado</p>
                                </div>
                            ) : (
                                <div className="space-y-2 max-h-64 overflow-y-auto pr-2">
                                    {anexos.map((anexo) => (
                                        <div key={anexo.id} className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors group">
                                            {/* Preview ou Ícone */}
                                            <div className="flex-shrink-0">
                                                {isImageFile(anexo.tipoArquivo) ? (
                                                    <img 
                                                        src={`http://localhost:5205/api/chamados/anexos/${anexo.id}/download`} 
                                                        alt={anexo.nomeArquivo}
                                                        className="w-12 h-12 object-cover rounded border border-gray-200"
                                                        onError={(e) => {
                                                            // Fallback se a imagem não carregar
                                                            e.currentTarget.style.display = 'none';
                                                            e.currentTarget.nextElementSibling!.classList.remove('hidden');
                                                        }}
                                                    />
                                                ) : null}
                                                <div className={isImageFile(anexo.tipoArquivo) ? 'hidden' : ''}>
                                                    {getFileIcon(anexo.tipoArquivo)}
                                                </div>
                                            </div>

                                            {/* Nome do arquivo */}
                                            <div className="flex-1 min-w-0">
                                                <p className="text-sm font-medium text-gray-900 truncate">{anexo.nomeArquivo}</p>
                                                <p className="text-xs text-gray-500">
                                                    {anexo.tipoArquivo.split('/')[1]?.toUpperCase() || 'Arquivo'}
                                                </p>
                                            </div>

                                            {/* Botão de download */}
                                            <button
                                                onClick={() => handleDownloadAnexo(anexo.id, anexo.nomeArquivo)}
                                                className="flex-shrink-0 p-2 text-gray-400 hover:text-primary-500 hover:bg-white rounded-lg transition-colors"
                                                title="Baixar arquivo"
                                            >
                                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                                                </svg>
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ChamadoDetailPage;