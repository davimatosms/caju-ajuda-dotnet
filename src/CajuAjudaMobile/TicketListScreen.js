import React, { useState, useEffect, useCallback } from 'react'; // Adiciona useCallback
import { View, Text, StyleSheet, FlatList, ActivityIndicator, TouchableOpacity, Alert, RefreshControl } from 'react-native'; // Adiciona Alert e RefreshControl
import * as SecureStore from 'expo-secure-store';
import Ionicons from '@expo/vector-icons/Ionicons';
import { getChamadosAsync } from './src/services/TicketService'; // <- Importa a função do serviço

export default function TicketListScreen({ navigation, route }) { // Adiciona route
    const [chamados, setChamados] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isRefreshing, setIsRefreshing] = useState(false); // Estado para o RefreshControl

    // Usa useCallback para memoizar a função e evitar recriações
    const fetchChamados = useCallback(async (showRefreshControl = false) => {
        if (showRefreshControl) {
            setIsRefreshing(true);
        } else {
            setIsLoading(true); // Só mostra o loading grande na primeira carga
        }
        try {
            // Chama a função do serviço
            const data = await getChamadosAsync();
            setChamados(data);
        } catch (error) {
            console.error('Erro ao buscar chamados:', error);
            Alert.alert('Erro', error.message || 'Não foi possível carregar os chamados.');
        } finally {
             if (showRefreshControl) {
                setIsRefreshing(false);
            } else {
                setIsLoading(false);
            }
        }
    }, []); // Array de dependências vazio, pois a função não depende de props/state externos

    useEffect(() => {
        // Listener para recarregar quando a tela ganha foco
        const unsubscribeFocus = navigation.addListener('focus', () => {
             // Verifica se veio da tela de Novo Chamado com o parâmetro 'refresh'
             if (route.params?.refresh) {
                fetchChamados();
                navigation.setParams({ refresh: false }); // Reseta o parâmetro
             } else if (chamados.length === 0) { // Carrega apenas se a lista estiver vazia (evita recarregar sempre)
                 fetchChamados();
             }
         });

        // Carrega os dados na montagem inicial
         fetchChamados();

        // Limpa o listener ao desmontar
        return unsubscribeFocus;
    }, [navigation, fetchChamados, route.params?.refresh]); // Adiciona fetchChamados e route.params?.refresh como dependências

    const renderItem = ({ item }) => (
        <TouchableOpacity
            style={styles.card}
            // Navega para Detalhes passando o ID
            onPress={() => navigation.navigate('TicketDetail', { chamadoId: item.id })}
        >
            <View style={styles.cardHeader}>
                <Text style={styles.cardTitle} numberOfLines={2}>#{item.id} - {item.titulo}</Text> {/* Permite 2 linhas */}
                <Text style={styles.cardDate}>
                    {item.dataCriacao ? new Date(item.dataCriacao).toLocaleDateString('pt-BR') : ''}
                </Text>
            </View>
            <View style={styles.cardBody}>
                <View style={styles.badgeContainer}>
                    {/* Adiciona verificação para status e prioridade antes de usar replace/toUpperCase */}
                    <Text style={[styles.badge, styles.statusBadge]}>
                        {item.status ? item.status.replace('_', ' ') : 'N/A'}
                    </Text>
                    <Text style={[styles.badge, styles.priorityBadge(item.prioridade)]}>
                        {item.prioridade || 'N/A'}
                    </Text>
                </View>
                <Ionicons name="chevron-forward" size={24} color="#cccccc" />
            </View>
        </TouchableOpacity>
    );

    // Mostra o ActivityIndicator grande apenas na carga inicial
    if (isLoading && !isRefreshing) {
        return <ActivityIndicator style={styles.loader} size="large" color="#f97316" />;
    }

    return (
        <View style={styles.container}>
            <FlatList
                data={chamados}
                renderItem={renderItem}
                keyExtractor={item => item.id.toString()}
                contentContainerStyle={styles.list}
                ListEmptyComponent={<Text style={styles.emptyText}>Nenhum chamado aberto encontrado.</Text>}
                // Adiciona a funcionalidade de "Puxar para atualizar" (Pull-to-Refresh)
                refreshControl={
                    <RefreshControl
                        refreshing={isRefreshing}
                        onRefresh={() => fetchChamados(true)} // Passa true para indicar que é um refresh manual
                        colors={["#f97316"]} // Cor do indicador no Android
                        tintColor={"#f97316"} // Cor do indicador no iOS
                    />
                }
            />
        </View>
    );
}

// Estilos permanecem os mesmos (com pequenas adições de segurança)
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
        flex: 1, // Permite que o título ocupe o espaço disponível
        marginRight: 8, // Espaço antes da data
    },
    cardDate: {
        fontSize: 12,
        color: '#718096',
        // marginLeft: 8, // Removido, marginRight no título cuida disso
    },
    cardBody: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: 8, // Adiciona espaço após o header
    },
    badgeContainer: {
        flexDirection: 'row',
        flexShrink: 1, // Permite encolher se necessário
        marginRight: 8, // Espaço antes do ícone de seta
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
        backgroundColor: '#e6f7ff', // Azul claro
        color: '#1d6fa5' // Azul escuro
    },
    priorityBadge: (priority) => {
        let colors = { backgroundColor: '#f3f4f6', color: '#4b5563' }; // Cinza (Baixa/Default)
        // Usa toUpperCase para comparar de forma insensível a maiúsculas/minúsculas
        switch (String(priority).toUpperCase()) {
            case 'MEDIA':
                colors = { backgroundColor: '#fffbe6', color: '#b45309' }; // Amarelo
                break;
            case 'ALTA':
                colors = { backgroundColor: '#fee2e2', color: '#b91c1c' }; // Vermelho
                break;
            // Adicionar case para 'URGENTE' se necessário
        }
        return colors;
    },
    emptyText: {
        textAlign: 'center',
        marginTop: 50,
        color: '#718096',
        fontSize: 16, // Aumenta um pouco o texto
    }
});