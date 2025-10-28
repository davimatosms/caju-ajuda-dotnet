// NewTicketScreen.js
import React, { useState } from 'react';
import { View, Text, TextInput, StyleSheet, TouchableOpacity, Alert, ActivityIndicator } from 'react-native';
// Não precisa mais do SecureStore aqui, pois o serviço cuida do token
import { createChamadoAsync } from './src/services/TicketService'; // <- Importa a função do serviço

export default function NewTicketScreen({ navigation }) {
    const [titulo, setTitulo] = useState('');
    const [descricao, setDescricao] = useState('');
    const [prioridade, setPrioridade] = useState('BAIXA'); // Padrão
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async () => {
        // Validação simples
        if (!titulo.trim() || !descricao.trim()) {
            Alert.alert('Erro', 'Por favor, preencha o título e a descrição.');
            return;
        }
        setIsLoading(true);

        try {
            // Chama a função do serviço passando os dados
            await createChamadoAsync(titulo, descricao, prioridade);

            // Se a função não lançou erro, consideramos sucesso
            Alert.alert('Sucesso', 'Seu chamado foi aberto com sucesso!');

            // Limpa os campos
            setTitulo('');
            setDescricao('');
            setPrioridade('BAIXA'); // Reseta para o padrão

            // Volta para a lista de chamados, sinalizando para atualizar a lista
            navigation.navigate('Meus Chamados', { refresh: true });

        } catch (error) {
            // Captura o erro lançado pelo serviço
            console.error('Erro ao criar chamado:', error);
            Alert.alert('Erro', error.message || 'Ocorreu um erro ao enviar seu chamado.'); // Mostra o erro vindo do serviço
        } finally {
            setIsLoading(false);
        }
    };

    // --- Renderização ---
    return (
        <View style={styles.container}>
            <Text style={styles.label}>Título do Chamado</Text>
            <TextInput
                style={styles.input}
                value={titulo}
                onChangeText={setTitulo}
                placeholder="Ex: Problema ao acessar a fatura"
                maxLength={150} // Adiciona limite baseado no modelo
            />

            <Text style={styles.label}>Descrição do Problema</Text>
            <TextInput
                style={[styles.input, styles.textArea]}
                value={descricao}
                onChangeText={setDescricao}
                placeholder="Descreva o problema com o máximo de detalhes possível."
                multiline
            />

            <Text style={styles.label}>Prioridade (Definida pela IA, mas selecione uma base)</Text>
            {/* Componente de seleção de prioridade */}
            <View style={styles.priorityContainer}>
                {/* Mapeia as prioridades disponíveis (excluindo URGENTE, se for só da IA) */}
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

            {/* Botão de Enviar */}
            <TouchableOpacity style={styles.submitButton} onPress={handleSubmit} disabled={isLoading}>
                {isLoading
                    ? <ActivityIndicator color="#fff" />
                    : <Text style={styles.submitButtonText}>Abrir Chamado</Text>}
            </TouchableOpacity>
        </View>
    );
}

// --- Estilos --- (Sem alterações significativas)
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
    textArea: { height: 120, textAlignVertical: 'top' },
    priorityContainer: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 30, },
    priorityButton: {
        flex: 1,
        paddingVertical: 12,
        borderRadius: 8,
        borderWidth: 1,
        borderColor: '#dee2e6',
        alignItems: 'center',
        marginHorizontal: 4,
    },
    prioritySelected: { backgroundColor: '#f97316', borderColor: '#f97316', },
    priorityText: { color: '#4a5568', fontWeight: '500', textTransform: 'capitalize', },
    priorityTextSelected: { color: '#fff', },
    submitButton: {
        backgroundColor: '#f97316',
        paddingVertical: 15,
        borderRadius: 8,
        alignItems: 'center',
    },
    submitButtonText: { color: '#fff', fontSize: 16, fontWeight: 'bold' },
});