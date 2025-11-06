import * as signalR from '@microsoft/signalr';

const API_URL = 'http://localhost:5205';

class SignalRService {
    private connection: signalR.HubConnection | null = null;
    private isConnecting = false;

    async connect(token: string): Promise<signalR.HubConnection> {
        if (this.connection?.state === signalR.HubConnectionState.Connected) {
            console.log('[SignalR] ✅ Já conectado');
            return this.connection;
        }

        if (this.isConnecting) {
            // Aguarda a conexão atual terminar
            await new Promise(resolve => setTimeout(resolve, 100));
            return this.connect(token);
        }

        this.isConnecting = true;
        
        console.log('[SignalR] 🔌 Iniciando conexão...');
        console.log('[SignalR] 📍 URL:', `${API_URL}/notificacaoHub`);
        console.log('[SignalR] 🔑 Token presente:', !!token);

        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${API_URL}/notificacaoHub`, {
                    accessTokenFactory: () => {
                        console.log('[SignalR] 🔐 Fornecendo token para autenticação...');
                        return token;
                    },
                    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Debug) // Mudou para Debug
                .build();

            // Eventos de reconexão
            this.connection.onreconnecting((error) => {
                console.warn('[SignalR] ⚠️ Tentando reconectar...', error);
            });

            this.connection.onreconnected((connectionId) => {
                console.log('[SignalR] ✅ Reconectado! ConnectionId:', connectionId);
            });

            this.connection.onclose((error) => {
                console.error('[SignalR] ❌ Conexão fechada:', error);
                // Auto-reconectar após 5 segundos
                console.log('[SignalR] 🔄 Tentando reconectar em 5 segundos...');
                setTimeout(() => {
                    this.connect(token).catch(err => 
                        console.error('[SignalR] Falha na reconexão automática:', err)
                    );
                }, 5000);
            });

            await this.connection.start();
            console.log('[SignalR] ✅ Conectado com sucesso!');
            console.log('[SignalR] 🆔 ConnectionId:', this.connection.connectionId);

            return this.connection;
        } catch (error: any) {
            console.error('[SignalR] ❌ Erro ao conectar:', error);
            console.error('[SignalR] 📋 Tipo do erro:', error.constructor.name);
            console.error('[SignalR] 📋 Mensagem:', error.message);
            console.error('[SignalR] 📋 Status:', error.statusCode);
            throw error;
        } finally {
            this.isConnecting = false;
        }
    }

    async joinRoom(chamadoId: number): Promise<void> {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            console.warn('[SignalR] Não conectado. Não foi possível entrar na sala.');
            return;
        }

        const roomName = `chamado_${chamadoId}`;
        console.log(`[SignalR] 📥 Tentando entrar na sala: ${roomName}`);
        try {
            await this.connection.invoke('JoinRoom', roomName);
            console.log(`[SignalR] ✅ Entrou na sala: ${roomName}`);
        } catch (error) {
            console.error(`[SignalR] ❌ Erro ao entrar na sala ${roomName}:`, error);
        }
    }

    async leaveRoom(chamadoId: number): Promise<void> {
        if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        const roomName = `chamado_${chamadoId}`;
        try {
            await this.connection.invoke('LeaveRoom', roomName);
            console.log(`[SignalR] ✅ Saiu da sala: ${roomName}`);
        } catch (error) {
            console.error(`[SignalR] ❌ Erro ao sair da sala ${roomName}:`, error);
        }
    }

    onNewMessage(callback: (message: any) => void): void {
        if (!this.connection) {
            console.warn('[SignalR] Conexão não estabelecida');
            return;
        }

        this.connection.on('ReceiveNewMessage', callback);
        console.log('[SignalR] ✅ Listener para novas mensagens registrado');
    }

    offNewMessage(): void {
        if (!this.connection) return;
        this.connection.off('ReceiveNewMessage');
    }

    async disconnect(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.connection = null;
            console.log('[SignalR] ✅ Desconectado');
        }
    }

    getConnectionState(): signalR.HubConnectionState | null {
        return this.connection?.state ?? null;
    }
}

export const signalRService = new SignalRService();
export default signalRService;
