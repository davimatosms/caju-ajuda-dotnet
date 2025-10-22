import React, { useState, useEffect, useRef, useCallback } from 'react';
import { View, Text, StyleSheet, FlatList, ActivityIndicator, TextInput, TouchableOpacity, KeyboardAvoidingView, Platform, Alert } from 'react-native';
import * as SecureStore from 'expo-secure-store'; // Ainda necessário para o serviço pegar o token
import { useSocket } from './useSocket'; // <- Importa o hook do socket
import { getChamadoDetalhesAsync, sendMessageAsync } from './src/services/TicketService'; // <- Importa funções do serviço

export default function TicketDetailScreen({ route, navigation }) { // Adiciona navigation
    const { chamadoId } = route.params;
    const [chamadoInfo, setChamadoInfo] = useState(null);
    const [mensagens, setMensagens] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [novaMensagem, setNovaMensagem] = useState('');
    const flatListRef = useRef(null);

    // Callback para adicionar novas mensagens recebidas via Socket
    const handleMessageReceived = useCallback((newMessage) => {
         setMensagens(prevMensagens => {
            // Previne duplicatas caso a mensagem chegue via socket antes da confirmação da API (embora não busquemos mais após enviar)
            if (prevMensagens.some(msg => msg.id === newMessage.id)) {
                return prevMensagens;
            }
            return [...prevMensagens, newMessage];
        });
        // Rolar para o fim quando uma nova mensagem chega
        setTimeout(() => flatListRef.current?.scrollToEnd({ animated: true }), 100);
    }, []);

    // Hook para gerenciar a conexão Socket.IO para esta sala
    const { isConnected } = useSocket(`chamado_${chamadoId}`, handleMessageReceived);

    // Efeito para buscar os detalhes iniciais do chamado e mensagens
    useEffect(() => {
        const fetchDetalhes = async () => {
            setIsLoading(true);
            try {
                // Chama a função do serviço
                const data = await getChamadoDetalhesAsync(chamadoId);
                setChamadoInfo(data); // Guarda as infos do chamado
                setMensagens(data.mensagens || []); // Guarda as mensagens iniciais
            } catch (error) {
                console.error("Erro ao buscar detalhes:", error);
                Alert.alert(
                    "Erro",
                    error.message || "Não foi possível carregar os detalhes do chamado.",
                    [{ text: 'OK', onPress: () => navigation.goBack() }] // Volta para a lista se der erro
                );
            } finally {
                setIsLoading(false);
            }
        };
        fetchDetalhes();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [chamadoId, navigation]); // Adiciona navigation como dependência

    // Função para enviar uma nova mensagem
    const handleSendMessage = async () => {
        const trimmedMessage = novaMensagem.trim();
        if (!trimmedMessage || !isConnected) return; // Não envia se vazio ou desconectado

        const tempText = novaMensagem; // Guarda o texto antes de limpar
        setNovaMensagem(''); // Limpa o input imediatamente

        try {
            // Chama a função do serviço
            await sendMessageAsync(chamadoId, trimmedMessage);
            // Sucesso! A mensagem deve chegar via socket.
        } catch (error) {
            console.error("Erro ao enviar mensagem:", error);
            Alert.alert('Erro', error.message || 'Não foi possível enviar a sua mensagem.');
            setNovaMensagem(tempText); // Restaura o texto no input em caso de erro
        }
        // Não precisamos de `finally` aqui
    };

    // Renderiza cada balão de mensagem
    const renderMensagem = ({ item }) => {
        const isTecnico = item.autor && item.autor.role === 'TECNICO';
        const authorName = item.autor ? item.autor.nome : 'Desconhecido';
        const messageDate = item.dataEnvio ? new Date(item.dataEnvio) : null;
        const timeString = messageDate ? messageDate.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }) : 'Enviando...';

        return (
            <View style={[
                styles.messageBubble,
                isTecnico ? styles.tecnicoBubble : styles.clienteBubble
            ]}>
                <Text style={styles.authorText}>{authorName}</Text>
                <Text style={styles.messageText}>{item.texto}</Text>
                <Text style={styles.timestampText}>{timeString}</Text>
            </View>
        );
     };

    // Mostra loading se ainda não carregou os detalhes iniciais
    if (isLoading) {
        return <ActivityIndicator style={styles.loader} size="large" color="#f97316" />;
    }

    // Se após carregar, chamadoInfo ainda for nulo (erro no fetch), mostra mensagem
    if (!chamadoInfo) {
         return (
             <View style={styles.loader}>
                 <Text style={styles.errorText}>Não foi possível carregar o chamado.</Text>
                 {/* Adicionar um botão para tentar novamente ou voltar? */}
             </View>
         );
    }


    return (
        <KeyboardAvoidingView
            behavior={Platform.OS === "ios" ? "padding" : "height"}
            style={styles.container}
            keyboardVerticalOffset={Platform.OS === "ios" ? 90 : 0}
        >
            {/* Cabeçalho com Título e Status */}
            <View style={styles.header}>
                <Text style={styles.title} numberOfLines={1}>{chamadoInfo.titulo}</Text>
                <Text style={styles.status}>{chamadoInfo.status?.replace('_', ' ') ?? 'N/A'}</Text>
            </View>

            {/* Lista de Mensagens */}
            <FlatList
                ref={flatListRef}
                data={mensagens}
                renderItem={renderMensagem}
                keyExtractor={(item, index) => item.id?.toString() || `msg-${index}`}
                contentContainerStyle={styles.chatContainer}
                 // Rolar para o fim quando o teclado aparece ou o conteúdo muda
                onContentSizeChange={() => flatListRef.current?.scrollToEnd({ animated: true })}
                onLayout={() => flatListRef.current?.scrollToEnd({ animated: false })} // Tenta rolar no layout inicial
            />

            {/* Input de Nova Mensagem */}
            <View style={styles.inputContainer}>
                <TextInput
                    style={styles.textInput}
                    value={novaMensagem}
                    onChangeText={setNovaMensagem}
                    placeholder={isConnected ? "Digite sua mensagem..." : "Conectando ao chat..."}
                    editable={isConnected}
                    multiline
                />
                <TouchableOpacity
                    style={[styles.sendButton, (!isConnected || !novaMensagem.trim()) && styles.sendButtonDisabled]}
                    onPress={handleSendMessage}
                    disabled={!isConnected || !novaMensagem.trim()}
                >
                    <Ionicons name="send" size={18} color="#fff" /> {/* Ícone de Enviar */}
                </TouchableOpacity>
            </View>
        </KeyboardAvoidingView>
    );
}

// Estilos (adicionei errorText e ajustei sendButton/sendButtonText)
const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#f0f2f5' },
    loader: { flex: 1, justifyContent: 'center', alignItems: 'center' },
    errorText: { color: 'red', fontSize: 16 }, // Estilo para erro
    header: { padding: 16, backgroundColor: '#fff', borderBottomWidth: 1, borderColor: '#e2e8f0' },
    title: { fontSize: 20, fontWeight: 'bold', color: '#1a202c', marginBottom: 4 },
    status: { fontSize: 14, color: '#4a5568', textTransform: 'capitalize' },
    chatContainer: { paddingVertical: 10, paddingHorizontal: 16 },
    messageBubble: { borderRadius: 12, paddingVertical: 8, paddingHorizontal: 12, marginBottom: 10, maxWidth: '80%' }, // Ajustei padding
    clienteBubble: { backgroundColor: '#e2e8f0', alignSelf: 'flex-start' },
    tecnicoBubble: { backgroundColor: '#fde68a', alignSelf: 'flex-end' },
    authorText: { fontWeight: 'bold', marginBottom: 4, color: '#4a5568', fontSize: 12 },
    messageText: { fontSize: 16, color: '#1a202c' },
    timestampText: { fontSize: 10, color: '#718096', alignSelf: 'flex-end', marginTop: 4 }, // Menos margem
    inputContainer: { flexDirection: 'row', padding: 10, backgroundColor: '#fff', borderTopWidth: 1, borderColor: '#e2e8f0', alignItems: 'flex-end' }, // Alinha no fim para multiline
    textInput: {
        flex: 1,
        maxHeight: 100, // Limita altura máxima para multiline
        borderWidth: 1, borderColor: '#dee2e6', borderRadius: 20,
        paddingHorizontal: 15, paddingVertical: 10, // Padding vertical
        backgroundColor: '#f8f9fa', fontSize: 16,
        paddingTop: 10, // Ajuste para iOS multiline
     },
    sendButton: {
        marginLeft: 10, justifyContent: 'center', alignItems: 'center',
        backgroundColor: '#f97316', borderRadius: 20,
        width: 40, height: 40, // Botão redondo
     },
    sendButtonText: { color: '#fff', fontWeight: 'bold' }, // Removido, usando ícone
    sendButtonDisabled: { backgroundColor: '#fdba74' }
});