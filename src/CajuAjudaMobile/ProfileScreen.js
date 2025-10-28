// ProfileScreen.js
import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, Alert, ActivityIndicator, ScrollView } from 'react-native';
import * as SecureStore from 'expo-secure-store'; // Necessário para Logout
import Ionicons from '@expo/vector-icons/Ionicons';
// Importa as funções do serviço
import { getProfileAsync, changePasswordAsync } from './src/services/ProfileService'; // <- Usa o ProfileService

export default function ProfileScreen({ navigation }) {
    const [user, setUser] = useState({ nome: '', email: '' });
    const [senhaAtual, setSenhaAtual] = useState('');
    const [novaSenha, setNovaSenha] = useState('');
    const [confirmaSenha, setConfirmaSenha] = useState('');
    const [isLoading, setIsLoading] = useState(true); // Loading inicial
    const [isUpdating, setIsUpdating] = useState(false); // Loading do botão de salvar senha

    // Função para buscar dados do perfil, usando useCallback para otimização
    const fetchUserData = useCallback(async () => {
        // Mostra loading apenas na primeira carga ou se user estiver vazio
        if (!user.nome) setIsLoading(true);

        try {
            console.log("--- ProfileScreen: Chamando getProfileAsync...");
            // Chama a função do serviço
            const data = await getProfileAsync();
            console.log("--- ProfileScreen: Dados do perfil recebidos.");
            setUser(data);
        } catch (error) {
            console.error("--- ProfileScreen: Erro ao buscar perfil:", error);
            Alert.alert("Erro", error.message || "Não foi possível carregar os dados do perfil.");
            setUser({ nome: 'Erro', email: 'Erro' }); // Indica erro na UI
        } finally {
            setIsLoading(false); // Sempre desativa o loading inicial
        }
    }, [user.nome]); // Depende de user.nome para não setar isLoading se já tiver dados

    // Efeito para carregar dados ao focar na tela e na montagem inicial
    useEffect(() => {
        console.log("--- ProfileScreen: Montando ou Focando ---");
        const unsubscribe = navigation.addListener('focus', fetchUserData);
        // fetchUserData(); // O 'addListener' já chama na primeira vez
        return unsubscribe; // Limpa o listener ao desmontar
    }, [navigation, fetchUserData]); // Depende da navegação e da função memoizada

    // Função para lidar com a alteração de senha
    const handleChangePassword = async () => {
        // Validações
        if (!senhaAtual || !novaSenha || !confirmaSenha) {
            Alert.alert('Atenção', 'Preencha todos os campos de senha.');
            return;
        }
        if (novaSenha.length < 6) { // Validação mínima
             Alert.alert('Erro', 'A nova senha deve ter no mínimo 6 caracteres.');
             return;
        }
        if (novaSenha !== confirmaSenha) {
            Alert.alert('Erro', 'As novas senhas não coincidem.');
            return;
        }

        setIsUpdating(true); // Ativa loading do botão
        try {
            console.log("--- ProfileScreen: Chamando changePasswordAsync...");
            // Chama a função do serviço
            await changePasswordAsync(senhaAtual, novaSenha);
            console.log("--- ProfileScreen: Senha alterada com sucesso.");

            Alert.alert('Sucesso', 'Senha alterada com sucesso!');
            // Limpa os campos
            setSenhaAtual('');
            setNovaSenha('');
            setConfirmaSenha('');

        } catch (error) {
            console.error("--- ProfileScreen: Erro ao alterar senha:", error);
            // Mostra o erro vindo do serviço (ex: "A senha atual está incorreta.")
            Alert.alert('Erro ao Alterar Senha', error.message || 'Ocorreu um erro.');
        } finally {
            setIsUpdating(false); // Desativa loading do botão
        }
    };

    // Função de Logout (permanece igual)
    const handleLogout = async () => {
        try {
            await SecureStore.deleteItemAsync('userToken');
            // Reseta a navegação para a tela de Login
            navigation.reset({ index: 0, routes: [{ name: 'Login' }] });
        } catch (error) {
            console.error("Erro ao fazer logout:", error);
            Alert.alert("Erro", "Não foi possível sair da conta.");
        }
    };

    // --- Renderização Condicional ---
    if (isLoading && !user.nome) {
        return <ActivityIndicator style={styles.loader} size="large" color="#f97316" />;
    }

    // --- Renderização Principal ---
    return (
        <ScrollView
            style={styles.container}
            keyboardShouldPersistTaps="handled" // Fecha teclado ao tocar fora
            showsVerticalScrollIndicator={false} // Oculta barra de rolagem
        >
            {/* Card de Informações */}
            <View style={styles.card}>
                <Text style={styles.cardTitle}>Informações da Conta</Text>
                <View style={styles.infoRow}>
                    <Ionicons name="person-outline" size={20} color="#6c757d" style={styles.icon} />
                    <Text style={styles.infoText}>{user.nome || 'Carregando...'}</Text>
                </View>
                <View style={styles.infoRow}>
                    <Ionicons name="mail-outline" size={20} color="#6c757d" style={styles.icon} />
                    <Text style={styles.infoText}>{user.email || 'Carregando...'}</Text>
                </View>
            </View>

            {/* Card de Alterar Senha */}
            <View style={styles.card}>
                <Text style={styles.cardTitle}>Alterar Senha</Text>
                <TextInput
                    style={styles.input}
                    placeholder="Senha Atual"
                    secureTextEntry
                    value={senhaAtual}
                    onChangeText={setSenhaAtual}
                    autoComplete="password"
                />
                <TextInput
                    style={styles.input}
                    placeholder="Nova Senha (mín. 6 caracteres)"
                    secureTextEntry
                    value={novaSenha}
                    onChangeText={setNovaSenha}
                    autoComplete="new-password"
                />
                <TextInput
                    style={styles.input}
                    placeholder="Confirmar Nova Senha"
                    secureTextEntry
                    value={confirmaSenha}
                    onChangeText={setConfirmaSenha}
                    autoComplete="new-password"
                />
                <TouchableOpacity style={styles.button} onPress={handleChangePassword} disabled={isUpdating}>
                    {isUpdating
                        ? <ActivityIndicator color="#fff" />
                        : <Text style={styles.buttonText}>Salvar Nova Senha</Text>}
                </TouchableOpacity>
            </View>

            {/* Botão de Logout */}
            <TouchableOpacity style={[styles.button, styles.logoutButton]} onPress={handleLogout}>
                <Text style={styles.buttonText}>Sair da Conta</Text>
            </TouchableOpacity>
        </ScrollView>
    );
}

// --- Estilos ---
const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#f0f2f5', padding: 16 },
    loader: { flex: 1, justifyContent: 'center', alignItems: 'center' },
    card: {
        backgroundColor: '#fff', borderRadius: 8, padding: 20, marginBottom: 20,
        shadowColor: "#000", shadowOffset: { width: 0, height: 1 }, shadowOpacity: 0.1, shadowRadius: 2, elevation: 3,
    },
    cardTitle: {
        fontSize: 18, fontWeight: 'bold', color: '#1a202c', marginBottom: 20,
        borderBottomWidth: 1, borderBottomColor: '#e2e8f0', paddingBottom: 10,
    },
    infoRow: { flexDirection: 'row', alignItems: 'center', marginBottom: 15, },
    icon: { marginRight: 10, },
    infoText: { fontSize: 16, color: '#4a5568', },
    input: {
        width: '100%', height: 50, backgroundColor: '#f8f9fa', borderWidth: 1, borderColor: '#dee2e6',
        borderRadius: 8, paddingHorizontal: 15, marginBottom: 15, fontSize: 16,
    },
    button: {
        width: '100%', height: 50, backgroundColor: '#f97316', borderRadius: 8,
        alignItems: 'center', justifyContent: 'center', marginTop: 10,
    },
    buttonText: { color: '#ffffff', fontSize: 16, fontWeight: 'bold', },
    logoutButton: { backgroundColor: '#ef4444', marginTop: 10, }
});