// src/services/TicketService.js
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../config'; // Importa a URL base da API

// URL base específica para as rotas de cliente/chamados
const API_CLIENTE_CHAMADOS_URL = `${API_BASE_URL}/cliente/chamados`;

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
        // Tenta parsear como JSON se a API retornar um objeto de erro estruturado
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
 * Busca a lista de chamados do cliente logado.
 * @returns {Promise<Array>} Uma lista de objetos de chamado.
 */
export const getChamadosAsync = async () => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(API_CLIENTE_CHAMADOS_URL, {
            headers: createAuthHeaders(token),
        });

        if (response.ok) {
            return await response.json(); // Retorna a lista de chamados
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao buscar chamados.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('Erro em getChamadosAsync:', error);
        // Garante que sempre lança um Error object
        throw error instanceof Error ? error : new Error('Erro de conexão ao buscar chamados.');
    }
};

/**
 * Busca os detalhes completos de um chamado específico (incluindo mensagens).
 * @param {number|string} chamadoId - O ID do chamado.
 * @returns {Promise<object>} Um objeto contendo os detalhes do chamado e suas mensagens.
 */
export const getChamadoDetalhesAsync = async (chamadoId) => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(`${API_CLIENTE_CHAMADOS_URL}/${chamadoId}`, {
            headers: createAuthHeaders(token),
        });

        if (response.ok) {
            return await response.json(); // Retorna o objeto com detalhes e mensagens
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao carregar detalhes do chamado.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error(`Erro em getChamadoDetalhesAsync (ID: ${chamadoId}):`, error);
        throw error instanceof Error ? error : new Error('Erro de conexão ao buscar detalhes do chamado.');
    }
};

/**
 * Cria um novo chamado para o cliente logado.
 * @param {string} titulo - Título do chamado.
 * @param {string} descricao - Descrição do chamado.
 * @param {string} prioridade - Prioridade ('BAIXA', 'MEDIA', 'ALTA').
 * @returns {Promise<object>} O objeto do chamado recém-criado (depende da resposta da API).
 */
export const createChamadoAsync = async (titulo, descricao, prioridade) => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(API_CLIENTE_CHAMADOS_URL, {
            method: 'POST',
            headers: createAuthHeaders(token),
            body: JSON.stringify({ titulo, descricao, prioridade }),
        });

        if (response.ok) {
            // Se a API retornar o objeto criado no corpo da resposta (status 201 ou 200)
             if (response.status === 201 || response.status === 200) {
                 try {
                     return await response.json();
                 } catch {
                     // Se não houver corpo ou não for JSON, retorna um sucesso genérico
                     return { success: true };
                 }
             }
             return { success: true }; // Para status 204 No Content
        } else {
            const errorMessage = await handleApiError(response, 'Não foi possível abrir o chamado.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('Erro em createChamadoAsync:', error);
        throw error instanceof Error ? error : new Error('Erro de conexão ao criar chamado.');
    }
};

/**
 * Envia uma nova mensagem em um chamado existente.
 * @param {number|string} chamadoId - O ID do chamado.
 * @param {string} texto - O conteúdo da mensagem.
 * @returns {Promise<object>} A objeto da mensagem recém-criada (depende da resposta da API).
 */
export const sendMessageAsync = async (chamadoId, texto) => {
    const token = await getToken();
    if (!token) throw new Error('Usuário não autenticado.');

    try {
        const response = await fetch(`${API_CLIENTE_CHAMADOS_URL}/${chamadoId}/mensagens`, {
            method: 'POST',
            headers: createAuthHeaders(token),
            body: JSON.stringify({ texto }),
        });

        if (response.ok) {
            // Assumindo que a API retorna a mensagem criada
             if (response.status === 201 || response.status === 200) {
                 try {
                     return await response.json();
                 } catch {
                     return { success: true };
                 }
             }
            return { success: true };
        } else {
            const errorMessage = await handleApiError(response, 'Não foi possível enviar a mensagem.');
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error(`Erro em sendMessageAsync (Chamado ID: ${chamadoId}):`, error);
        throw error instanceof Error ? error : new Error('Erro de conexão ao enviar mensagem.');
    }
};