import React from 'react';
import { TicketStatusBadge, TicketStatus } from './TicketStatusBadge';
import { TicketPriorityBadge, TicketPriority } from './TicketPriorityBadge';

export interface TicketHeaderData {
    id: number;
    titulo: string;
    descricao: string;
    status: TicketStatus;
    prioridade: TicketPriority;
    dataCriacao: string;
    clienteNome?: string;
    tecnicoResponsavel?: string;
}

interface TicketHeaderProps {
    ticket: TicketHeaderData;
    className?: string;
}

export const TicketHeader: React.FC<TicketHeaderProps> = ({ ticket, className = '' }) => {
    return (
        <div className={`card flex-shrink-0 ${className}`}>
            <div className="flex items-start justify-between mb-4">
                <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                        <span className="text-sm font-semibold text-primary-600">#{ticket.id}</span>
                        <TicketStatusBadge status={ticket.status} />
                        <TicketPriorityBadge priority={ticket.prioridade} />
                    </div>
                    <h1 className="text-2xl font-bold text-gray-900 mb-2">{ticket.titulo}</h1>
                    <p className="text-sm text-gray-500">
                        Aberto em {new Date(ticket.dataCriacao).toLocaleString('pt-BR')}
                    </p>
                </div>
            </div>

            <div className="border-t pt-4">
                <h3 className="text-sm font-semibold text-gray-700 mb-2">Descrição do Problema</h3>
                <p className="text-gray-600 whitespace-pre-wrap">{ticket.descricao}</p>
            </div>

            {(ticket.clienteNome || ticket.tecnicoResponsavel) && (
                <div className="border-t pt-4 mt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
                    {ticket.clienteNome && (
                        <div>
                            <strong className="text-gray-700">Cliente:</strong>{' '}
                            <span className="text-gray-900">{ticket.clienteNome}</span>
                        </div>
                    )}
                    {ticket.tecnicoResponsavel && (
                        <div>
                            <strong className="text-gray-700">Técnico:</strong>{' '}
                            <span className="text-gray-900">{ticket.tecnicoResponsavel}</span>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};
