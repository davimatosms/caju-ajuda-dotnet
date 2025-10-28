import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    TouchableOpacity,
    StyleSheet,
    Alert,
    ActivityIndicator,
    Image
} from 'react-native';
import { registerAndSendVerificationAsync } from './src/services/AuthService'; // <- Importa do serviço

export default function RegisterScreen({ navigation }) {
    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [senha, setSenha] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // Função que será chamada ao pressionar o botão "Registar"
    const handleRegister = async () => {
        if (!nome || !email || !senha) {
            Alert.alert('Erro', 'Por favor, preencha todos os campos.');
            return;
        }
        setIsLoading(true);
        try {
            // Chama a função do serviço
            await registerAndSendVerificationAsync(nome, email, senha);

            // Se chegou aqui, o registro (e tentativa de envio de email) foi bem-sucedido
            Alert.alert(
                'Sucesso!',
                'Sua conta foi criada. Enviámos um e-mail de verificação para você.',
                [{ text: 'OK', onPress: () => navigation.goBack() }] // Volta para a tela de login
            );

        } catch (error) {
            // O erro lançado pelo AuthService será capturado aqui
            Alert.alert('Erro no Registo', error.message);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Image source={{ uri: 'https://i.imgur.com/yyFJDsb.png' }} style={styles.logo} />
            <Text style={styles.title}>Crie sua Conta</Text>

            <TextInput
                style={styles.input}
                placeholder="Nome Completo"
                value={nome}
                onChangeText={setNome}
                autoCapitalize="words"
            />
            <TextInput
                style={styles.input}
                placeholder="E-mail"
                value={email}
                onChangeText={setEmail}
                keyboardType="email-address"
                autoCapitalize="none"
            />
            <TextInput
                style={styles.input}
                placeholder="Senha"
                value={senha}
                onChangeText={setSenha}
                secureTextEntry
            />

            <TouchableOpacity style={styles.button} onPress={handleRegister} disabled={isLoading}>
                {isLoading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Registar</Text>}
            </TouchableOpacity>

            <TouchableOpacity onPress={() => navigation.goBack()}>
                <Text style={styles.backLink}>Já tem uma conta? Faça o login</Text>
            </TouchableOpacity>
        </View>
    );
}

// Estilos permanecem os mesmos
const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#f8f9fa',
        alignItems: 'center',
        justifyContent: 'center',
        padding: 20,
    },
    logo: {
        width: 80,
        height: 80,
        marginBottom: 20,
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
        color: '#1a202c',
        marginBottom: 30,
    },
    input: {
        width: '100%',
        height: 50,
        backgroundColor: '#ffffff',
        borderWidth: 1,
        borderColor: '#dee2e6',
        borderRadius: 8,
        paddingHorizontal: 15,
        marginBottom: 15,
        fontSize: 16,
    },
    button: {
        width: '100%',
        height: 50,
        backgroundColor: '#f97316',
        borderRadius: 8,
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: 10,
    },
    buttonText: {
        color: '#ffffff',
        fontSize: 16,
        fontWeight: 'bold',
    },
    backLink: {
        marginTop: 20,
        color: '#f97316',
        fontWeight: '600',
    }
});