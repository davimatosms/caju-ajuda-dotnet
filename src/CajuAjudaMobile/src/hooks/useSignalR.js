// src/hooks/useSignalR.js
import { useState, useEffect, useRef, useCallback } from 'react';
import * as signalR from "@microsoft/signalr";
// Importa a URL COMPLETA do Hub (incluindo o caminho)
import { SIGNALR_HUB_URL } from '../config';

/**
 * Hook customizado para gerenciar uma conexão SignalR.
 * @param {object} handlers - Objeto com métodos a serem ouvidos do servidor.
 * Ex: { "ReceiveNewMessage": (message) => console.log(message) }
 * @returns {{ connection: signalR.HubConnection | null, isConnected: boolean, invoke: Function }}
 */
export const useSignalR = (handlers) => { // Remove hubPath dos parâmetros
    const [connection, setConnection] = useState(null);
    const [isConnected, setIsConnected] = useState(false);
    const connectionRef = useRef(null);

    // Efeito para criar, iniciar e limpar a conexão
    useEffect(() => {
        // --- URL CORRIGIDA ---
        // Usa diretamente a URL completa importada do config.js
        console.log(`--- [SignalR Hook] Criando conexão para: ${SIGNALR_HUB_URL}`);
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(SIGNALR_HUB_URL, { // <- USA APENAS A URL COMPLETA
                // Mantém a tentativa de forçar WebSockets, pode ajudar com emuladores
                transport: signalR.HttpTransportType.WebSockets,
                skipNegotiation: false
            })
            .withAutomaticReconnect()
            .build();

        connectionRef.current = newConnection;
        setConnection(newConnection);

        const startConnection = async () => {
            try {
                console.log(`--- [SignalR Hook] Tentando iniciar conexão (WebSockets)...`);
                await newConnection.start();
                console.log(`--- [SignalR Hook] Conectado! Estado: ${newConnection.state}`);
                setIsConnected(true);
            } catch (err) {
                console.error(`--- [SignalR Hook] Falha ao conectar:`, err);
                setIsConnected(false);
            }
        };

        startConnection();

        newConnection.onclose((error) => {
            console.warn('--- [SignalR Hook] Conexão fechada:', error || 'Desconexão normal.');
            setIsConnected(false);
        });

        // Limpeza
        return () => {
             console.log("--- [SignalR Hook] Parando conexão...");
             newConnection.stop()
                 .then(() => console.log("--- [SignalR Hook] Conexão parada."))
                 .catch(err => console.error("--- [SignalR Hook] Erro ao parar conexão:", err));
             connectionRef.current = null;
             setConnection(null);
             setIsConnected(false);
        };

    // Efeito roda apenas uma vez na montagem, pois não depende de parâmetros externos
    }, []);


    // Efeito para registrar/atualizar os handlers
    useEffect(() => {
        if (connectionRef.current && handlers && typeof handlers === 'object') {
             console.log(`--- [SignalR Hook] Registrando/Atualizando handlers`);
             Object.keys(handlers).forEach((methodName) => {
                connectionRef.current.off(methodName);
                connectionRef.current.on(methodName, handlers[methodName]);
                console.log(`--- [SignalR Hook] Ouvindo o método: ${methodName}`);
             });
             return () => { // Limpeza dos handlers
                 if (connectionRef.current) {
                    console.log(`--- [SignalR Hook] Desregistrando handlers`);
                     Object.keys(handlers).forEach((methodName) => {
                         connectionRef.current.off(methodName);
                     });
                 }
             };
         }
    // Depende da instância da conexão e dos handlers
    }, [connection, handlers]);


    // Função invoke (igual a antes)
    const invoke = useCallback(async (methodName, ...args) => {
        if (connectionRef.current && connectionRef.current.state === signalR.HubConnectionState.Connected) {
            try {
                console.log(`--- [SignalR Hook] Invocando método '${methodName}' com args:`, args);
                await connectionRef.current.invoke(methodName, ...args);
                console.log(`--- [SignalR Hook] Método '${methodName}' invocado com sucesso.`);
            } catch (err) {
                console.error(`--- [SignalR Hook] Erro ao invocar '${methodName}':`, err);
            }
        } else {
            console.warn(`--- [SignalR Hook] Não foi possível invocar '${methodName}'. Conexão não está ativa. Estado: ${connectionRef.current?.state}`);
        }
    }, []);

    return { connection: connectionRef.current, isConnected, invoke };
};