import React from 'react';

export type TicketPriority = 'BAIXA' | 'MEDIA' | 'ALTA' | 'URGENTE';

interface TicketPriorityBadgeProps {
    priority: TicketPriority;
    className?: string;
}

const priorityConfig: Record<TicketPriority, { text: string; className: string }> = {
    BAIXA: {
        text: 'Baixa',
        className: 'badge bg-gray-100 text-gray-600'
    },
    MEDIA: {
        text: 'Média',
        className: 'badge bg-blue-100 text-blue-700'
    },
    ALTA: {
        text: 'Alta',
        className: 'badge bg-orange-100 text-orange-700'
    },
    URGENTE: {
        text: 'Urgente',
        className: 'badge badge-danger'
    }
};

export const TicketPriorityBadge: React.FC<TicketPriorityBadgeProps> = ({ priority, className = '' }) => {
    const config = priorityConfig[priority] || priorityConfig.MEDIA;
    
    return (
        <span className={`${config.className} ${className}`}>
            {config.text}
        </span>
    );
};
