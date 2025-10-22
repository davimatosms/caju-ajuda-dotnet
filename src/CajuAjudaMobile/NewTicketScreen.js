import React, { useState } from 'react';
import { View, Text, TextInput, StyleSheet, TouchableOpacity, Alert, ActivityIndicator } from 'react-native';
import * as SecureStore from 'expo-secure-store'; // SecureStore ainda pode ser necessário se o serviço não lidar com o token
import { createChamadoAsync } from './src/services/TicketService'; // <- Importa a função do serviço

export default function NewTicketScreen({ navigation }) {
    const [titulo, setTitulo] = useState('');
    const [descricao, setDescricao] = useState('');
    const [prioridade, setPrioridade] = useState('BAIXA'); // Padrão
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async () => {
        if (!titulo || !descricao) {
            Alert.alert('Erro', 'Por favor, preencha o título e a descrição.');
            return;
        }
        setIsLoading(true);

        try {
            // Chama a função do serviço passando os dados
            await createChamadoAsync(titulo, descricao, prioridade);

            // Se a função não lançou erro, consideramos sucesso
            Alert.alert('Sucesso', 'Seu chamado foi aberto com sucesso!');

            // Limpa os campos e volta para a lista de chamados, sinalizando para atualizar
            setTitulo('');
            setDescricao('');
            setPrioridade('BAIXA'); // Reseta para o padrão
            navigation.navigate('Meus Chamados', { refresh: true });

        } catch (error) {
            console.error('Erro ao criar chamado:', error);
            Alert.alert('Erro', error.message || 'Ocorreu um erro ao enviar seu chamado.'); // Mostra o erro vindo do serviço
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.label}>Título do Chamado</Text>
            <TextInput
                style={styles.input}
                value={titulo}
                onChangeText={setTitulo}
                placeholder="Ex: Problema ao acessar a fatura"
            />

            <Text style={styles.label}>Descrição do Problema</Text>
            <TextInput
                style={[styles.input, styles.textArea]}
                value={descricao}
                onChangeText={setDescricao}
                placeholder="Descreva o problema com o máximo de detalhes possível."
                multiline
            />

            <Text style={styles.label}>Prioridade</Text>
            {/* Componente de seleção de prioridade */}
            <View style={styles.priorityContainer}>
                {/* Mapeia as prioridades disponíveis */}
                {['BAIXA', 'MEDIA', 'ALTA'].map((p) => (
                    <TouchableOpacity
                        key={p}
                        style={[
                            styles.priorityButton,
                            prioridade === p && styles.prioritySelected // Estilo condicional
                        ]}
                        onPress={() => setPrioridade(p)} // Atualiza o estado
                    >
                        <Text style={[
                            styles.priorityText,
                            prioridade === p && styles.priorityTextSelected // Estilo condicional
                        ]}>{p}</Text>
                    </TouchableOpacity>
                ))}
            </View>

            <TouchableOpacity style={styles.submitButton} onPress={handleSubmit} disabled={isLoading}>
                {isLoading ? <ActivityIndicator color="#fff" /> : <Text style={styles.submitButtonText}>Abrir Chamado</Text>}
            </TouchableOpacity>
        </View>
    );
}

// Estilos permanecem os mesmos
const styles = StyleSheet.create({
    container: { flex: 1, padding: 20, backgroundColor: '#f8f9fa' },
    label: { fontSize: 16, fontWeight: '500', color: '#4a5568', marginBottom: 8, },
    input: {
        backgroundColor: '#fff',
        borderWidth: 1,
        borderColor: '#dee2e6',
        borderRadius: 8,
        paddingHorizontal: 15,
        paddingVertical: 12,
        fontSize: 16,
        marginBottom: 20,
    },
    textArea: { height: 120, textAlignVertical: 'top' }, // Permite texto longo
    priorityContainer: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 30, },
    priorityButton: {
        flex: 1, // Faz os botões ocuparem espaço igual
        paddingVertical: 12,
        borderRadius: 8,
        borderWidth: 1,
        borderColor: '#dee2e6',
        alignItems: 'center',
        marginHorizontal: 4, // Pequeno espaço entre botões
    },
    prioritySelected: { backgroundColor: '#f97316', borderColor: '#f97316', },
    priorityText: { color: '#4a5568', fontWeight: '500', textTransform: 'capitalize', }, // Capitaliza BAIXA -> Baixa
    priorityTextSelected: { color: '#fff', },
    submitButton: {
        backgroundColor: '#f97316',
        paddingVertical: 15,
        borderRadius: 8,
        alignItems: 'center',
    },
    submitButtonText: { color: '#fff', fontSize: 16, fontWeight: 'bold' },
});