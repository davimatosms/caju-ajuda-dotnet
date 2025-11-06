import React from 'react';

export interface AttachmentData {
    id: number;
    nomeArquivo: string;
    caminhoArquivo: string;
    tamanho?: number;
    dataUpload: string;
}

interface TicketAttachmentListProps {
    attachments: AttachmentData[];
    onDownload?: (attachment: AttachmentData) => void;
    className?: string;
}

const formatFileSize = (bytes?: number): string => {
    if (!bytes) return 'N/A';
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(2)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
};

const getFileIcon = (fileName: string): React.ReactElement => {
    const extension = fileName.split('.').pop()?.toLowerCase();
    
    const iconMap: Record<string, { color: string; path: string }> = {
        pdf: { 
            color: 'text-red-500',
            path: 'M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z'
        },
        doc: { 
            color: 'text-blue-500',
            path: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
        },
        docx: { 
            color: 'text-blue-500',
            path: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
        },
        jpg: { 
            color: 'text-green-500',
            path: 'M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z'
        },
        jpeg: { 
            color: 'text-green-500',
            path: 'M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z'
        },
        png: { 
            color: 'text-green-500',
            path: 'M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z'
        },
    };

    const icon = iconMap[extension || ''] || { 
        color: 'text-gray-500',
        path: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
    };

    return (
        <svg className={`w-8 h-8 ${icon.color}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={icon.path} />
        </svg>
    );
};

export const TicketAttachmentList: React.FC<TicketAttachmentListProps> = ({ 
    attachments, 
    onDownload,
    className = '' 
}) => {
    if (!attachments || attachments.length === 0) {
        return null;
    }

    return (
        <div className={`card flex-shrink-0 ${className}`}>
            <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                </svg>
                Anexos ({attachments.length})
            </h3>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {attachments.map((attachment) => (
                    <div key={attachment.id} className="border rounded-lg p-3 hover:bg-gray-50 transition-colors">
                        <div className="flex flex-col h-full">
                            <div className="text-center mb-2">
                                {getFileIcon(attachment.nomeArquivo)}
                            </div>

                            <h6 className="text-sm font-medium text-gray-900 truncate mb-2" title={attachment.nomeArquivo}>
                                {attachment.nomeArquivo}
                            </h6>

                            <div className="mb-2 text-xs text-gray-500 space-y-1">
                                <div>Tamanho: {formatFileSize(attachment.tamanho)}</div>
                                <div>Data: {new Date(attachment.dataUpload).toLocaleDateString('pt-BR')}</div>
                            </div>

                            {onDownload && (
                                <button 
                                    className="mt-auto btn-secondary text-sm py-1.5 px-3 flex items-center justify-center gap-1 w-full"
                                    onClick={() => onDownload(attachment)}
                                >
                                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                                    </svg>
                                    Download
                                </button>
                            )}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};
