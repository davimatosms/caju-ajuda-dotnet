// TicketListScreen.js
import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, FlatList, ActivityIndicator, TouchableOpacity, Alert, RefreshControl } from 'react-native';
import * as SecureStore from 'expo-secure-store'; // Necessário para getToken no serviço
import Ionicons from '@expo/vector-icons/Ionicons';
import { getChamadosAsync } from './src/services/TicketService'; // Importa a função do serviço

export default function TicketListScreen({ navigation, route }) {
    const [chamados, setChamados] = useState([]);
    const [isLoading, setIsLoading] = useState(true); // Controla o loading inicial grande
    const [isRefreshing, setIsRefreshing] = useState(false); // Controla o loading do "Pull-to-Refresh"

    // Usa useCallback para memoizar a função fetchChamados
    // Evita recriações desnecessárias a cada renderização
    const fetchChamados = useCallback(async (isManualRefresh = false) => {
        console.log("--- TicketListScreen: Iniciando fetchChamados. Refresh manual:", isManualRefresh); // Log A
        // Define o estado de loading apropriado
        if (isManualRefresh) {
            setIsRefreshing(true);
        } else if (!isRefreshing) { // Só ativa loading grande se não for refresh manual
            setIsLoading(true);
        }

        try {
            console.log("--- TicketListScreen: Chamando getChamadosAsync..."); // Log B
            // Chama a função do serviço para buscar os dados
            const data = await getChamadosAsync();
            console.log("--- TicketListScreen: getChamadosAsync retornou. Qtd chamados:", data ? data.length : 'null/undefined'); // Log C
            setChamados(data || []); // Garante que seja sempre um array
        } catch (error) {
            // Captura erros lançados pelo TicketService
            console.error('--- TicketListScreen: Erro capturado em fetchChamados:', error); // Log D
            // Mostra um alerta para o usuário
            Alert.alert('Erro ao Carregar', error.message || 'Não foi possível carregar os chamados.');
            setChamados([]); // Limpa chamados em caso de erro para evitar mostrar dados antigos
        } finally {
            // Garante que ambos os loadings sejam desativados
             if (isManualRefresh) {
                setIsRefreshing(false);
            }
             setIsLoading(false); // Sempre desativa o loading inicial após a tentativa
             console.log("--- TicketListScreen: fetchChamados finalizado."); // Log Fim
        }
    }, []); // Array de dependências vazio, pois a função não depende diretamente de props/state

    // Efeito para carregar dados:
    // 1. Na montagem inicial do componente.
    // 2. Quando a tela recebe foco (navegação de volta).
    useEffect(() => {
        // Listener que executa a função quando a tela ganha foco
        const unsubscribeFocus = navigation.addListener('focus', () => {
             console.log("--- TicketListScreen: Tela focada. Route params:", route.params); // Log E
             // Verifica se a navegação anterior (ex: NewTicketScreen) pediu um refresh
             if (route.params?.refresh) {
                console.log("--- TicketListScreen: Detectado refresh=true, chamando fetchChamados."); // Log F
                fetchChamados(false); // Chama fetchChamados (sem ativar RefreshControl visual)
                navigation.setParams({ refresh: false }); // Limpa o parâmetro para não recarregar de novo
             }
             // Se não veio com parâmetro refresh, podemos decidir se recarregamos ou não.
             // Recarregar sempre ao focar pode ser pesado. Talvez só se a lista estiver vazia.
             // else if (chamados.length === 0 && !isLoading) {
             //     console.log("--- TicketListScreen: Lista vazia e não carregando, chamando fetchChamados ao focar.");
             //     fetchChamados(false);
             // }
         });

        // Executa o fetchChamados na montagem inicial do componente
        console.log("--- TicketListScreen: Montando componente, chamando fetchChamados inicial."); // Log H
        fetchChamados(false); // Primeira carga, sem ativar RefreshControl visual

        // Função de limpeza: remove o listener quando o componente é desmontado
        return unsubscribeFocus;
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [navigation, route.params?.refresh]); // Depende da navegação e do parâmetro refresh

    // Função para renderizar cada item (chamado) na FlatList
    const renderItem = ({ item }) => (
        <TouchableOpacity
            style={styles.card}
            // Navega para a tela de detalhes passando o ID do chamado como parâmetro
            onPress={() => navigation.navigate('TicketDetail', { chamadoId: item.id })}
        >
            {/* Cabeçalho do Card */}
            <View style={styles.cardHeader}>
                {/* Título do chamado (permite quebrar em até 2 linhas) */}
                <Text style={styles.cardTitle} numberOfLines={2}>#{item.id} - {item.titulo}</Text>
                {/* Data de criação formatada */}
                <Text style={styles.cardDate}>
                    {item.dataCriacao ? new Date(item.dataCriacao).toLocaleDateString('pt-BR') : ''}
                </Text>
            </View>
            {/* Corpo do Card */}
            <View style={styles.cardBody}>
                {/* Badges de Status e Prioridade */}
                <View style={styles.badgeContainer}>
                    {/* Status (com tratamento para valor nulo/undefined) */}
                    <Text style={[styles.badge, styles.statusBadge]}>
                        {item.status ? String(item.status).replace('_', ' ') : 'N/A'}
                    </Text>
                    {/* Prioridade (com tratamento e estilo dinâmico) */}
                    <Text style={[styles.badge, styles.priorityBadge(item.prioridade)]}>
                        {item.prioridade || 'N/A'}
                    </Text>
                     {/* Opcional: Indicador de mensagens não lidas */}
                    {/* {item.HasUnreadMessages && <Ionicons name="ellipse" size={10} color="red" style={{ marginLeft: 5, alignSelf: 'center' }} />} */}
                </View>
                {/* Ícone de seta indicando que é clicável */}
                <Ionicons name="chevron-forward" size={24} color="#cccccc" />
            </View>
        </TouchableOpacity>
    );

    // Renderização condicional: Mostra ActivityIndicator grande APENAS na carga inicial
    if (isLoading && !isRefreshing) {
        return <ActivityIndicator style={styles.loader} size="large" color="#f97316" />;
    }

    // Renderização principal: A FlatList com os chamados
    return (
        <View style={styles.container}>
            <FlatList
                data={chamados}
                renderItem={renderItem}
                keyExtractor={item => item.id.toString()} // Usa o ID do chamado como chave
                contentContainerStyle={styles.list} // Estilo para o container interno da lista
                // Componente mostrado se a lista 'chamados' estiver vazia
                ListEmptyComponent={<Text style={styles.emptyText}>Nenhum chamado aberto encontrado.</Text>}
                // Habilita a funcionalidade "Puxar para atualizar" (Pull-to-Refresh)
                refreshControl={
                    <RefreshControl
                        refreshing={isRefreshing} // Controla se o indicador de refresh está visível
                        onRefresh={() => fetchChamados(true)} // Função chamada ao puxar (passa true)
                        colors={["#f97316"]} // Cor do indicador no Android
                        tintColor={"#f97316"} // Cor do indicador no iOS
                    />
                }
            />
        </View>
    );
}

// Estilos (sem alterações significativas em relação à versão anterior com logs)
const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#f0f2f5' },
    list: { padding: 16 },
    loader: { flex: 1, justifyContent: 'center', alignItems: 'center' },
    card: {
        backgroundColor: '#fff',
        borderRadius: 8,
        padding: 16,
        marginBottom: 12,
        shadowColor: "#000",
        shadowOffset: { width: 0, height: 1 },
        shadowOpacity: 0.1,
        shadowRadius: 2,
        elevation: 3,
    },
    cardHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginBottom: 12,
        alignItems: 'center',
    },
    cardTitle: {
        fontSize: 16,
        fontWeight: 'bold',
        color: '#1a202c',
        flex: 1,
        marginRight: 8,
    },
    cardDate: {
        fontSize: 12,
        color: '#718096',
    },
    cardBody: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: 8,
    },
    badgeContainer: {
        flexDirection: 'row',
        flexShrink: 1,
        marginRight: 8,
    },
    badge: {
        borderRadius: 12,
        paddingVertical: 4,
        paddingHorizontal: 10,
        fontSize: 12,
        fontWeight: '500',
        marginRight: 8,
        overflow: 'hidden',
        textTransform: 'capitalize',
    },
    statusBadge: {
        backgroundColor: '#e6f7ff',
        color: '#1d6fa5'
    },
    priorityBadge: (priority) => {
        let colors = { backgroundColor: '#f3f4f6', color: '#4b5563' }; // Cinza (Baixa/Default)
        switch (String(priority).toUpperCase()) {
            case 'MEDIA':
                colors = { backgroundColor: '#fffbe6', color: '#b45309' }; // Amarelo
                break;
            case 'ALTA':
                colors = { backgroundColor: '#fee2e2', color: '#b91c1c' }; // Vermelho
                break;
            case 'URGENTE': // Adicionando URGENTE se existir
                colors = { backgroundColor: '#374151', color: '#f9fafb' }; // Cinza escuro com texto claro
                break;
        }
        return colors;
    },
    emptyText: {
        textAlign: 'center',
        marginTop: 50,
        color: '#718096',
        fontSize: 16,
    }
});