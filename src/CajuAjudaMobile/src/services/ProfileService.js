// src/services/ProfileService.js
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../config'; // Importa a URL base da API (ex: http://192.168.15.11:5205/api)

// --- Constantes de Rota (baseadas no PerfilController.cs) ---
const PERFIL_API_URL = `${API_BASE_URL}/Perfil`; // Rota base do PerfilController

/**
 * Obtém o token de autenticação armazenado.
 */
const getToken = async () => {
    return await SecureStore.getItemAsync('userToken');
};

/**
 * Cria os headers padrão para requisições autenticadas.
 */
const createAuthHeaders = (token) => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
});

/**
 * Lida com respostas não-OK da API.
 */
const handleApiError = async (response, defaultMessage) => {
    try {
        const errorText = await response.text();
        console.log(`--- ProfileService (handleApiError): Status ${response.status}, Texto: ${errorText}`);
        try {
            const errorJson = JSON.parse(errorText);
            return errorJson.message || errorJson.title || errorJson.error || errorText || defaultMessage;
        } catch {
            return errorText || defaultMessage;
        }
    } catch (e) {
        console.error("--- ProfileService (handleApiError): Erro ao processar resposta de erro:", e);
        return defaultMessage;
    }
};

/**
 * Busca os dados do perfil do cliente logado. (GET /api/Perfil)
 * @returns {Promise<object>} Um objeto com os dados do perfil (nome, email).
 */
export const getProfileAsync = async () => {
    console.log("--- ProfileService: Iniciando getProfileAsync ---");
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        console.log(`--- ProfileService: Fazendo fetch para ${PERFIL_API_URL}`);
        const response = await fetch(PERFIL_API_URL, {
            headers: createAuthHeaders(token),
        });
        console.log("--- ProfileService: Resposta do fetch recebida. Status:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("--- ProfileService: Perfil recebido.");
            return data;
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao carregar dados do perfil.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('--- ProfileService: Erro CRÍTICO em getProfileAsync:', error);
        if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para buscar perfil.');
        }
        throw error;
    }
};

/**
 * Altera a senha do cliente logado. (PATCH /api/Perfil/update-senha)
 * @param {string} senhaAtual - A senha atual do usuário.
 * @param {string} novaSenha - A nova senha desejada.
 * @returns {Promise<object>} Um objeto indicando sucesso.
 */
export const changePasswordAsync = async (senhaAtual, novaSenha) => {
    console.log("--- ProfileService: Iniciando changePasswordAsync ---");
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    const url = `${PERFIL_API_URL}/update-senha`; // Endpoint do PerfilController

    try {
        console.log(`--- ProfileService: Fazendo PATCH para ${url}`);
        const response = await fetch(url, {
            method: 'PATCH', // --- CORRIGIDO: Usando PATCH como no PerfilController.cs ---
            headers: createAuthHeaders(token),
            body: JSON.stringify({ senhaAtual, novaSenha }),
        });
        console.log("--- ProfileService (ChangePass): Resposta do PATCH recebida. Status:", response.status);

        if (response.ok) {
             console.log("--- ProfileService (ChangePass): Senha alterada com sucesso.");
             // API retorna 204 No Content, então não há corpo para parsear
             return { success: true };
        } else {
            // Se falhar (ex: 400 Bad Request se a senha atual estiver errada), lê a mensagem de erro
            const errorMessage = await handleApiError(response, 'Erro ao alterar senha.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('--- ProfileService (ChangePass): Erro CRÍTICO em changePasswordAsync:', error);
         if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para alterar senha.');
        }
        throw error;
    }
};