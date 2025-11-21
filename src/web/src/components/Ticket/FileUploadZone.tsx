import React, { useState, useRef, DragEvent, ChangeEvent } from 'react';

interface FileUploadZoneProps {
    onFilesSelected: (files: File[]) => void;
    acceptedFileTypes?: string;
    maxFiles?: number;
    disabled?: boolean;
    className?: string;
}

export const FileUploadZone: React.FC<FileUploadZoneProps> = ({ 
    onFilesSelected,
    acceptedFileTypes = '*',
    maxFiles = 10,
    disabled = false,
    className = '' 
}) => {
    const [isDragging, setIsDragging] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleDragEnter = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        if (!disabled) {
            setIsDragging(true);
        }
    };

    const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);
    };

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);

        if (disabled) return;

        const files = Array.from(e.dataTransfer.files).slice(0, maxFiles);
        if (files.length > 0) {
            onFilesSelected(files);
        }
    };

    const handleFileSelect = (e: ChangeEvent<HTMLInputElement>) => {
        if (disabled) return;

        const files = e.target.files ? Array.from(e.target.files).slice(0, maxFiles) : [];
        if (files.length > 0) {
            onFilesSelected(files);
        }
        
        // Reset input so same file can be selected again
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    const handleClick = () => {
        if (!disabled) {
            fileInputRef.current?.click();
        }
    };

    const dropZoneClass = isDragging 
        ? 'border-primary bg-primary bg-opacity-10' 
        : 'border-secondary';

    const cursorClass = disabled ? 'cursor-not-allowed' : 'cursor-pointer';

    return (
        <div className={`card shadow-sm ${className}`}>
            <div 
                className={`text-center border-2 border-dashed rounded-lg transition-colors ${dropZoneClass} ${cursorClass} p-8`}
                onDragEnter={handleDragEnter}
                onDragLeave={handleDragLeave}
                onDragOver={handleDragOver}
                onDrop={handleDrop}
                onClick={handleClick}
            >
                <input
                    ref={fileInputRef}
                    type="file"
                    multiple
                    accept={acceptedFileTypes}
                    onChange={handleFileSelect}
                    disabled={disabled}
                    className="hidden"
                />

                <div className="mb-2">
                    <svg className="w-12 h-12 text-primary-500 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                    </svg>
                </div>

                <p className="mb-1 font-bold text-gray-700">
                    {isDragging ? 'Solte os arquivos aqui' : 'Clique ou arraste arquivos'}
                </p>

                <small className="text-gray-500 text-sm">
                    Máximo de {maxFiles} arquivo(s)
                </small>
            </div>
        </div>
    );
};
