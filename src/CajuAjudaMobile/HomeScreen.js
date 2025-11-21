import React from 'react';
import { View, Text, StyleSheet, Button } from 'react-native';
import * as SecureStore from 'expo-secure-store'; // Importa SecureStore

// Uma tela simples que será o nosso dashboard (ou ponto de partida após login)
export default function HomeScreen({ navigation }) {
  // A função para "fazer logout"
  const handleLogout = async () => {
    try {
        // Limpa o token guardado
        await SecureStore.deleteItemAsync('userToken');
        // Navega de volta para a tela de Login e reseta a pilha
        navigation.reset({
            index: 0,
            routes: [{ name: 'Login' }],
        });
    } catch (error) {
        console.error("Erro ao fazer logout:", error);
        Alert.alert("Erro", "Não foi possível sair da conta.");
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Bem-vindo!</Text>
      <Text style={styles.subtitle}>Você está logado no Caju Ajuda.</Text>
      {/* Botão de Logout */}
      <View style={styles.buttonContainer}>
        <Button title="Sair" onPress={handleLogout} color="#ef4444" />
      </View>
    </View>
  );
}

// Estilos permanecem os mesmos
const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
    backgroundColor: '#f8f9fa',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 10,
    color: '#1a202c',
  },
  subtitle: {
    fontSize: 18,
    color: '#718096',
    marginBottom: 30,
    textAlign: 'center',
  },
   buttonContainer: { // Adicionado para dar margem ao botão
    marginTop: 20,
    width: '80%', // Limita a largura do botão
  }
});