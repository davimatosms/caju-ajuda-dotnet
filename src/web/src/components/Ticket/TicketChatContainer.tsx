import React, { useEffect, useRef, ReactNode } from 'react';

interface TicketChatContainerProps {
    children: ReactNode;
    autoScroll?: boolean;
    isLoading?: boolean;
    className?: string;
}

export const TicketChatContainer: React.FC<TicketChatContainerProps> = ({ 
    children, 
    autoScroll = true,
    isLoading = false,
    className = '' 
}) => {
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (autoScroll && containerRef.current) {
            containerRef.current.scrollTop = containerRef.current.scrollHeight;
        }
    }, [children, autoScroll]);

    return (
        <div className={`card p-0 flex flex-col relative shadow-sm ${className}`}>
            <div className="px-6 py-4 border-b bg-gray-50">
                <h2 className="text-lg font-semibold text-gray-900">Conversa</h2>
                <p className="text-sm text-gray-600">Comunique-se com a equipe de suporte</p>
            </div>

            <div 
                ref={containerRef}
                className="flex-1 overflow-y-auto px-6 py-4 space-y-4"
            >
                {isLoading ? (
                    <div className="flex justify-center items-center h-full">
                        <svg className="animate-spin h-12 w-12 text-primary-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        <span className="sr-only">Carregando...</span>
                    </div>
                ) : (
                    <>
                        {children}
                    </>
                )}
            </div>
        </div>
    );
};
