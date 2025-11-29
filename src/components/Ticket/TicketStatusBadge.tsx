import React from 'react';

export type TicketStatus = 'ABERTO' | 'EM_ANDAMENTO' | 'AGUARDANDO_CLIENTE' | 'RESOLVIDO' | 'FECHADO';

interface TicketStatusBadgeProps {
    status: TicketStatus;
    className?: string;
}

const statusConfig: Record<TicketStatus, { text: string; className: string }> = {
    ABERTO: {
        text: 'Aberto',
        className: 'badge badge-warning'
    },
    EM_ANDAMENTO: {
        text: 'Em Andamento',
        className: 'badge badge-info'
    },
    AGUARDANDO_CLIENTE: {
        text: 'Aguardando Cliente',
        className: 'badge bg-purple-100 text-purple-700'
    },
    RESOLVIDO: {
        text: 'Resolvido',
        className: 'badge badge-success'
    },
    FECHADO: {
        text: 'Fechado',
        className: 'badge bg-gray-100 text-gray-700'
    }
};

export const TicketStatusBadge: React.FC<TicketStatusBadgeProps> = ({ status, className = '' }) => {
    const config = statusConfig[status] || statusConfig.ABERTO;
    
    return (
        <span className={`${config.className} ${className}`}>
            {config.text}
        </span>
    );
};
