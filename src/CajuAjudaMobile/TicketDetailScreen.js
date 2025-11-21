// TicketDetailScreen.js
import React, { useState, useEffect, useRef, useCallback } from 'react'; // <-- useCallback foi adicionado aqui
import {
    View,
    Text,
    StyleSheet,
    FlatList,
    ActivityIndicator,
    TextInput,
    TouchableOpacity,
    KeyboardAvoidingView,
    Platform,
    Alert
} from 'react-native';
import Ionicons from '@expo/vector-icons/Ionicons'; // Para o ícone de envio
import { useSignalR } from './src/hooks/useSignalR'; // Importa o hook SignalR corrigido
import { getChamadoDetalhesAsync, sendMessageAsync } from './src/services/TicketService'; // Importa funções do serviço

export default function TicketDetailScreen({ route, navigation }) {
    const { chamadoId } = route.params;
    const [chamadoInfo, setChamadoInfo] = useState(null);
    const [mensagens, setMensagens] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [novaMensagem, setNovaMensagem] = useState('');
    const flatListRef = useRef(null); // Ref para controlar a FlatList

    // --- Configuração do SignalR ---

    // Define o handler para receber novas mensagens via SignalR
    const handlers = {
        // !!! ATENÇÃO: Substitua "ReceiveNewMessage" pelo nome EXATO do método no seu NotificacaoHub.cs !!!
        "ReceiveNewMessage": useCallback((message) => {
            console.log("--- [TicketDetailScreen] Mensagem SignalR Recebida:", message);
            // Adiciona a nova mensagem à lista, prevenindo duplicatas
            setMensagens(prevMensagens => {
                if (prevMensagens.some(msg => msg.id === message.id)) {
                    return prevMensagens;
                }
                return [...prevMensagens, message];
            });
            // Rola para o fim um pouco depois para garantir que a UI atualizou
            setTimeout(() => flatListRef.current?.scrollToEnd({ animated: true }), 100);
        }, []) // useCallback para memoizar a função e evitar recriações
    };

    // Usa o hook SignalR, passando APENAS os handlers
    // A URL completa do Hub é obtida do config.js dentro do hook
    const { connection, isConnected, invoke } = useSignalR(handlers);

    // --- Fim da Configuração do SignalR ---


    // Efeito para buscar detalhes iniciais do chamado via API REST
    useEffect(() => {
        const fetchDetalhes = async () => {
            setIsLoading(true);
            setChamadoInfo(null); // Limpa info anterior
            setMensagens([]); // Limpa mensagens anteriores
            try {
                // Chama a função do TicketService para buscar detalhes
                const data = await getChamadoDetalhesAsync(chamadoId);
                setChamadoInfo(data); // Armazena título, status, etc.
                setMensagens(data.mensagens || []); // Armazena mensagens iniciais
            } catch (error) {
                console.error("Erro ao buscar detalhes do chamado:", error);
                Alert.alert(
                    "Erro",
                    error.message || "Não foi possível carregar os detalhes do chamado.",
                    // Botão que volta para a tela anterior em caso de erro
                    [{ text: 'OK', onPress: () => navigation.goBack() }]
                );
            } finally {
                setIsLoading(false);
            }
        };
        fetchDetalhes();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [chamadoId, navigation]); // Depende do ID do chamado e da navegação


    // Efeito para entrar na sala SignalR específica deste chamado
    // Executa sempre que a conexão SignalR fica ativa (isConnected) ou o ID do chamado muda
    useEffect(() => {
        if (isConnected && chamadoId) {
            const roomName = `chamado_${chamadoId}`;
            console.log(`--- [TicketDetailScreen] Entrando na sala SignalR: ${roomName}`);

            // !!! ATENÇÃO: Substitua "JoinRoom" pelo nome EXATO do método no seu NotificacaoHub.cs para entrar na sala !!!
            // Chama o método 'JoinRoom' no Hub do backend
            invoke('JoinRoom', roomName);

            // Não é necessário um 'leaveRoom' explícito aqui, pois a conexão
            // inteira é encerrada quando o componente é desmontado (limpeza do hook useSignalR)
        }
    }, [isConnected, chamadoId, invoke]); // Depende do estado da conexão, ID e da função invoke


    // Função para ENVIAR uma nova mensagem (via API REST)
    const handleSendMessage = async () => {
        const trimmedMessage = novaMensagem.trim();
        // Não envia se a mensagem estiver vazia ou se não estiver conectado ao SignalR
        if (!trimmedMessage || !isConnected) return;

        const messageToSend = novaMensagem; // Guarda o texto antes de limpar
        setNovaMensagem(''); // Limpa o input otimisticamente

        try {
            // Chama a função do TicketService para enviar a mensagem via POST
            await sendMessageAsync(chamadoId, messageToSend);
            // Sucesso! A mensagem foi enviada para a API.
            // Agora esperamos que o backend processe, salve no banco,
            // e envie a mensagem de volta para todos na sala via SignalR (incluindo nós).
        } catch (error) {
            console.error("Erro ao enviar mensagem:", error);
            Alert.alert('Erro', error.message || 'Não foi possível enviar a sua mensagem.');
            setNovaMensagem(messageToSend); // Restaura o texto no input em caso de erro
        }
    };


    // Função para renderizar cada item (mensagem) na FlatList
    const renderMensagem = ({ item }) => {
        // Determina se a mensagem é do técnico ou do cliente para estilização
        const isTecnico = item.autor && item.autor.role === 'TECNICO';
        // Nome do autor ou 'Desconhecido'
        const authorName = item.autor ? item.autor.nome : 'Desconhecido';
        // Formata a data/hora
        const messageDate = item.dataEnvio ? new Date(item.dataEnvio) : null;
        const timeString = messageDate
            ? messageDate.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
            : 'Enviando...';

        return (
            // Aplica estilos diferentes para cliente vs técnico
            <View style={[ styles.messageBubble, isTecnico ? styles.tecnicoBubble : styles.clienteBubble ]}>
                <Text style={styles.authorText}>{authorName}</Text>
                <Text style={styles.messageText}>{item.texto}</Text>
                <Text style={styles.timestampText}>{timeString}</Text>
            </View>
        );
     };


    // --- Renderização Condicional ---

    // Se estiver carregando os dados iniciais
    if (isLoading) {
        return <ActivityIndicator style={styles.loader} size="large" color="#f97316" />;
    }

    // Se ocorreu um erro ao carregar (chamadoInfo ainda é null após isLoading ser false)
    if (!chamadoInfo) {
         return (
             <View style={styles.loader}>
                 <Text style={styles.errorText}>Não foi possível carregar o chamado.</Text>
             </View>
         );
    }

    // --- Renderização Principal ---
    return (
        // KeyboardAvoidingView ajusta a tela quando o teclado abre
        <KeyboardAvoidingView
            behavior={Platform.OS === "ios" ? "padding" : "height"} // Comportamento diferente por plataforma
            style={styles.container}
            // Ajuste fino da distância que a tela sobe
            keyboardVerticalOffset={Platform.OS === "ios" ? 90 : 0}
        >
            {/* Cabeçalho com Título e Status */}
            <View style={styles.header}>
                <Text style={styles.title} numberOfLines={1}>{chamadoInfo.titulo}</Text>
                {/* Mostra 'N/A' se o status não estiver definido */}
                <Text style={styles.status}>{chamadoInfo.status?.replace('_', ' ') ?? 'N/A'}</Text>
            </View>

            {/* A lista de mensagens */}
            <FlatList
                ref={flatListRef} // Passa a ref para podermos controlar o scroll
                data={mensagens}
                renderItem={renderMensagem}
                // Chave única para cada mensagem (usa ID ou index como fallback)
                keyExtractor={(item, index) => item.id?.toString() || `msg-${index}`}
                contentContainerStyle={styles.chatContainer} // Estilo para o container interno da lista
                // Tenta rolar para o fim automaticamente quando o conteúdo da lista muda
                onContentSizeChange={() => flatListRef.current?.scrollToEnd({ animated: true })}
                // Tenta rolar para o fim quando o layout da lista é calculado pela primeira vez
                onLayout={() => flatListRef.current?.scrollToEnd({ animated: false })}
            />

            {/* Container para o input de texto e botão de enviar */}
            <View style={styles.inputContainer}>
                <TextInput
                    style={styles.textInput}
                    value={novaMensagem}
                    onChangeText={setNovaMensagem}
                    // Placeholder muda se o SignalR não estiver conectado
                    placeholder={isConnected ? "Digite sua mensagem..." : "Conectando ao chat..."}
                    // Só permite edição se conectado
                    editable={isConnected}
                    multiline // Permite que o input cresça verticalmente
                />
                <TouchableOpacity
                    style={[
                        styles.sendButton,
                        // Estilo desabilitado se não conectado ou se a mensagem (sem espaços) estiver vazia
                        (!isConnected || !novaMensagem.trim()) && styles.sendButtonDisabled
                    ]}
                    onPress={handleSendMessage}
                    // Desabilita o toque também sob as mesmas condições
                    disabled={!isConnected || !novaMensagem.trim()}
                >
                    {/* Usa um ícone em vez de texto */}
                    <Ionicons name="send" size={18} color="#fff" />
                </TouchableOpacity>
            </View>
        </KeyboardAvoidingView>
    );
}

// --- Estilos ---
const styles = StyleSheet.create({
    container: {
        flex: 1, // Ocupa toda a tela
        backgroundColor: '#f0f2f5' // Fundo cinza claro
    },
    loader: {
        flex: 1, // Centraliza o loader na tela inteira
        justifyContent: 'center',
        alignItems: 'center'
    },
    errorText: { // Estilo para mensagens de erro
        color: 'red',
        fontSize: 16,
        textAlign: 'center'
    },
    header: { // Cabeçalho da tela
        padding: 16,
        backgroundColor: '#fff', // Fundo branco
        borderBottomWidth: 1, // Linha divisória
        borderColor: '#e2e8f0' // Cinza claro
    },
    title: { // Título do chamado
        fontSize: 20,
        fontWeight: 'bold',
        color: '#1a202c', // Cinza escuro
        marginBottom: 4
    },
    status: { // Status do chamado
        fontSize: 14,
        color: '#4a5568', // Cinza médio
        textTransform: 'capitalize' // Ex: Aberto, Em_Andamento -> Em Andamento
    },
    chatContainer: { // Container interno da FlatList
        paddingVertical: 10, // Espaço acima e abaixo das mensagens
        paddingHorizontal: 16 // Espaço nas laterais
    },
    messageBubble: { // Estilo base para os balões de mensagem
        borderRadius: 12,
        paddingVertical: 8, // Espaço vertical dentro do balão
        paddingHorizontal: 12, // Espaço horizontal dentro do balão
        marginBottom: 10, // Espaço entre balões
        maxWidth: '80%' // Largura máxima para não ocupar a tela inteira
    },
    clienteBubble: { // Mensagens do cliente
        backgroundColor: '#e2e8f0', // Fundo cinza claro
        alignSelf: 'flex-start' // Alinha à esquerda
    },
    tecnicoBubble: { // Mensagens do técnico
        backgroundColor: '#fde68a', // Fundo amarelo claro (cor do Caju?)
        alignSelf: 'flex-end' // Alinha à direita
    },
    authorText: { // Nome do autor da mensagem
        fontWeight: 'bold',
        marginBottom: 4,
        color: '#4a5568', // Cinza médio
        fontSize: 12 // Tamanho menor
    },
    messageText: { // Texto da mensagem
        fontSize: 16,
        color: '#1a202c' // Cinza escuro
    },
    timestampText: { // Hora/Data da mensagem
        fontSize: 10,
        color: '#718096', // Cinza mais claro
        alignSelf: 'flex-end', // Alinha à direita dentro do balão
        marginTop: 4 // Pequeno espaço acima
    },
    inputContainer: { // Container do TextInput e botão Enviar
        flexDirection: 'row', // Itens lado a lado
        padding: 10,
        backgroundColor: '#fff', // Fundo branco
        borderTopWidth: 1, // Linha divisória acima
        borderColor: '#e2e8f0', // Cinza claro
        alignItems: 'flex-end' // Alinha itens na base (bom para multiline)
    },
    textInput: { // Caixa de texto para digitar a mensagem
        flex: 1, // Ocupa o espaço disponível
        maxHeight: 100, // Altura máxima antes de scroll interno (se multiline)
        borderWidth: 1,
        borderColor: '#dee2e6', // Cinza
        borderRadius: 20, // Bordas arredondadas
        paddingHorizontal: 15,
        paddingVertical: 10, // Espaçamento interno
        backgroundColor: '#f8f9fa', // Fundo levemente cinza
        fontSize: 16,
        paddingTop: 10, // Ajuste necessário para multiline no iOS
    },
    sendButton: { // Botão de Enviar
        marginLeft: 10, // Espaço à esquerda do botão
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: '#f97316', // Laranja Caju
        borderRadius: 20, // Botão redondo
        width: 40, // Largura fixa
        height: 40, // Altura fixa (igual à largura)
    },
    sendButtonDisabled: { // Estilo do botão quando desabilitado
        backgroundColor: '#fdba74' // Laranja mais claro
    }
});