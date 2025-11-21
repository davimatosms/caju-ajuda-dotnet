import React, { useState, useRef, KeyboardEvent } from 'react';

interface MessageInputProps {
    onSendMessage: (message: string, isInternalNote: boolean) => void;
    onFileSelect?: (file: File) => void;
    disabled?: boolean;
    showInternalNoteOption?: boolean;
    placeholder?: string;
    className?: string;
    isUploadingFile?: boolean;
}

export const MessageInput: React.FC<MessageInputProps> = ({ 
    onSendMessage,
    onFileSelect,
    disabled = false,
    showInternalNoteOption = false,
    placeholder = 'Digite sua mensagem...',
    className = '',
    isUploadingFile = false
}) => {
    const [message, setMessage] = useState('');
    const [isInternalNote, setIsInternalNote] = useState(false);
    const textareaRef = useRef<HTMLTextAreaElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleSend = () => {
        if (message.trim() && !disabled) {
            onSendMessage(message.trim(), isInternalNote);
            setMessage('');
            setIsInternalNote(false);
            textareaRef.current?.focus();
        }
    };

    const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file && onFileSelect) {
            onFileSelect(file);
            if (fileInputRef.current) {
                fileInputRef.current.value = '';
            }
        }
    };

    return (
        <div className={`card shadow-sm ${className}`}>
            <div className="p-4">
                <div className="mb-3 relative">
                    <textarea
                        ref={textareaRef}
                        className="flex-1 input-field resize-none w-full pr-12"
                        rows={3}
                        placeholder={placeholder}
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        onKeyDown={handleKeyDown}
                        disabled={disabled}
                    />
                    
                    {/* Botão de anexo no canto direito do textarea */}
                    {onFileSelect && (
                        <>
                            <input
                                ref={fileInputRef}
                                type="file"
                                onChange={handleFileChange}
                                disabled={disabled || isUploadingFile}
                                className="hidden"
                                accept=".pdf,.doc,.docx,.txt,.png,.jpg,.jpeg,.gif"
                            />
                            <button
                                type="button"
                                onClick={() => fileInputRef.current?.click()}
                                disabled={disabled || isUploadingFile}
                                className="absolute right-3 bottom-3 p-2 text-gray-400 hover:text-primary-500 hover:bg-gray-50 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                title="Anexar arquivo"
                            >
                                {isUploadingFile ? (
                                    <svg className="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                ) : (
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                                    </svg>
                                )}
                            </button>
                        </>
                    )}
                </div>

                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
                    <div className="flex items-center gap-4">
                        {showInternalNoteOption && (
                            <div className="flex items-center gap-2">
                                <input
                                    type="checkbox"
                                    id="internalNote"
                                    checked={isInternalNote}
                                    onChange={(e) => setIsInternalNote(e.target.checked)}
                                    disabled={disabled}
                                    className="w-4 h-4 text-primary-600 rounded focus:ring-primary-500"
                                />
                                <label htmlFor="internalNote" className="text-sm text-gray-700">
                                    Nota Interna (visível apenas para técnicos)
                                </label>
                            </div>
                        )}
                        <small className="text-gray-500 text-xs">
                            Enter para enviar • Shift+Enter para nova linha
                        </small>
                    </div>

                    <button 
                        onClick={handleSend}
                        disabled={disabled || !message.trim()}
                        className="btn-primary px-6 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                    >
                        Enviar
                    </button>
                </div>
            </div>
        </div>
    );
};
