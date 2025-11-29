import React from 'react';

export interface ChatMessageData {
    id: number;
    conteudo: string;
    dataEnvio: string;
    autorNome: string;
    autorTipo: 'CLIENTE' | 'TECNICO';
    isNotaInterna?: boolean;
    lidoPeloCliente?: boolean;
}

interface ChatMessageProps {
    message: ChatMessageData;
    isCurrentUser: boolean;
    className?: string;
}

export const ChatMessage: React.FC<ChatMessageProps> = ({ message, isCurrentUser, className = '' }) => {
    const isInternalNote = message.isNotaInterna;
    const isUnread = !message.lidoPeloCliente && message.autorTipo === 'TECNICO';

    const messageAlignment = isCurrentUser ? 'justify-end' : 'justify-start';
    const messageBgClass = isInternalNote 
        ? 'bg-yellow-50 border border-yellow-300'
        : isCurrentUser 
            ? 'bg-primary-500 text-white' 
            : 'bg-gray-100 text-gray-900';

    return (
        <div className={`flex mb-3 ${messageAlignment} ${className}`}>
            <div className={`w-full max-w-[85%] lg:max-w-[70%] ${messageBgClass} rounded-lg px-4 py-3 shadow-sm`}>
                <div className="flex justify-between items-start mb-1 gap-2">
                    <strong className={`text-xs font-semibold ${isCurrentUser && !isInternalNote ? 'text-white' : 'text-gray-900'}`}>
                        {message.autorNome}
                    </strong>
                    <div className="flex gap-1 flex-shrink-0">
                        {isInternalNote && (
                            <span className="text-xs bg-yellow-500 text-white px-2 py-0.5 rounded whitespace-nowrap">
                                Nota Interna
                            </span>
                        )}
                        {isUnread && (
                            <span className="text-xs bg-red-500 text-white px-2 py-0.5 rounded whitespace-nowrap">
                                Não lido
                            </span>
                        )}
                    </div>
                </div>
                <p className={`text-sm whitespace-pre-wrap mb-1 break-words ${isCurrentUser && !isInternalNote ? 'text-white' : 'text-gray-900'}`}>
                    {message.conteudo}
                </p>
                <small className={`text-xs ${isCurrentUser && !isInternalNote ? 'text-white opacity-75' : 'text-gray-500'}`}>
                    {new Date(message.dataEnvio).toLocaleString('pt-BR')}
                </small>
            </div>
        </div>
    );
};
