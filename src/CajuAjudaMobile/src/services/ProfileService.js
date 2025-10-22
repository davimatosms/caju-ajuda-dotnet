// src/services/ProfileService.js
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../config'; // Importa a URL base da API

// URL base específica para as rotas de cliente/perfil
const API_CLIENTE_PERFIL_URL = `${API_BASE_URL}/cliente/perfil`;

/**
 * Obtém o token de autenticação armazenado.
 * @returns {Promise<string|null>} O token ou null se não encontrado.
 */
const getToken = async () => {
    return await SecureStore.getItemAsync('userToken');
};

/**
 * Cria os headers padrão para requisições autenticadas.
 * @param {string} token - O token JWT.
 * @returns {object} Objeto de headers.
 */
const createAuthHeaders = (token) => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
});

/**
 * Lida com respostas não-OK da API, tentando extrair uma mensagem de erro.
 * @param {Response} response - A resposta do fetch.
 * @param {string} defaultMessage - Mensagem padrão em caso de erro.
 */
const handleApiError = async (response, defaultMessage) => {
    try {
        const errorText = await response.text();
         try {
            const errorJson = JSON.parse(errorText);
            return errorJson.message || errorJson.title || errorText || defaultMessage;
        } catch {
            return errorText || defaultMessage;
        }
    } catch {
        return defaultMessage;
    }
};


/**
 * Busca os dados do perfil do cliente logado.
 * @returns {Promise<object>} Um objeto com os dados do perfil (nome, email, etc.).
 */
export const getProfileAsync = async () => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(API_CLIENTE_PERFIL_URL, {
            headers: createAuthHeaders(token),
        });

        if (response.ok) {
            return await response.json(); // Retorna os dados do perfil
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao carregar dados do perfil.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('Erro em getProfileAsync:', error);
        throw error instanceof Error ? error : new Error('Erro de conexão ao buscar perfil.');
    }
};

/**
 * Altera a senha do cliente logado.
 * @param {string} senhaAtual - A senha atual do usuário.
 * @param {string} novaSenha - A nova senha desejada.
 * @returns {Promise<object>} Um objeto indicando sucesso (depende da resposta da API).
 */
export const changePasswordAsync = async (senhaAtual, novaSenha) => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(`${API_CLIENTE_PERFIL_URL}/alterar-senha`, {
            method: 'POST',
            headers: createAuthHeaders(token),
            body: JSON.stringify({ senhaAtual, novaSenha }),
        });

        if (response.ok) {
            // API pode retornar 200 OK com corpo ou 204 No Content
             try {
                // Tenta ler o corpo, se houver
                 return await response.json();
             } catch {
                 // Se não houver corpo (204) ou não for JSON, retorna sucesso genérico
                 return { success: true };
             }
        } else {
            const errorMessage = await handleApiError(response, 'Erro ao alterar senha.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('Erro em changePasswordAsync:', error);
        throw error instanceof Error ? error : new Error('Erro de conexão ao alterar senha.');
    }
};