// src/services/TicketService.js
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../config'; // Importa a URL base da API (ex: http://192.168.15.11:5205/api)

// --- Constantes de Rota (baseadas nos Controllers do Backend) ---
// Define a constante no escopo do módulo para que todas as funções possam usá-la
const CHAMADOS_API_URL = `${API_BASE_URL}/Chamados`; // Rota base do ChamadosController

/**
 * Obtém o token de autenticação armazenado.
 * @returns {Promise<string|null>} O token ou null se não encontrado.
 */
const getToken = async () => {
    // console.log("--- TicketService (getToken): Tentando obter token...");
    const token = await SecureStore.getItemAsync('userToken');
    // console.log("--- TicketService (getToken): Token obtido?", token ? 'Sim' : 'Não');
    return token;
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
        console.log(`--- TicketService (handleApiError): Status ${response.status}, Texto: ${errorText}`); // Log do erro bruto
        try {
            const errorJson = JSON.parse(errorText);
            // Prioriza mensagens específicas da API, se existirem (message, title, ou error)
            return errorJson.message || errorJson.title || errorJson.error || errorText || defaultMessage;
        } catch {
            // Se não for JSON, retorna o texto ou a mensagem padrão
            return errorText || defaultMessage;
        }
    } catch (e) {
        console.error("--- TicketService (handleApiError): Erro ao processar resposta de erro:", e);
        return defaultMessage;
    }
};

/**
 * Busca a lista de chamados do cliente logado. (GET /api/Chamados/meus)
 * @returns {Promise<Array>} Uma lista de objetos de chamado.
 */
export const getChamadosAsync = async () => {
    console.log("--- TicketService: Iniciando getChamadosAsync ---"); // Log 1
    let token = null;
    try {
        token = await getToken();
        console.log("--- TicketService: Token obtido:", token ? 'Sim' : 'Não'); // Log 2
    } catch (tokenError) {
        console.error("--- TicketService: Erro ao obter token:", tokenError); // Log Erro Token
        throw new Error('Erro ao acessar armazenamento seguro.');
    }

    if (!token) {
        console.error("--- TicketService: Token nulo ou vazio."); // Log Token Nulo
        throw new Error('Usuário não autenticado.');
    }

    // --- URL CORRIGIDA ---
    const url = `${CHAMADOS_API_URL}/meus`; // Usa a constante definida no topo

    try {
        console.log(`--- TicketService: Fazendo fetch para ${url}`); // Log 3
        const response = await fetch(url, {
            headers: createAuthHeaders(token),
        });
        console.log("--- TicketService: Resposta do fetch recebida. Status:", response.status); // Log 4

        if (response.ok) {
            const data = await response.json();
            console.log("--- TicketService: Chamados recebidos:", data ? data.length : 0); // Log 5 (com verificação)
            return data || []; // Retorna data ou array vazio se data for null/undefined
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao buscar chamados.');
            console.error("--- TicketService: Erro da API:", errorMessage); // Log Erro API
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('--- TicketService: Erro CRÍTICO em getChamadosAsync:', error); // Log Erro Catch
        // Melhora a mensagem de erro de rede
        if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para buscar chamados.');
        }
        // Garante que sempre lança um Error object
        throw error instanceof Error ? error : new Error('Erro de conexão ao buscar chamados.');
    }
};


/**
 * Busca os detalhes completos de um chamado específico (incluindo mensagens). (GET /api/Chamados/{id})
 * @param {number|string} chamadoId - O ID do chamado.
 * @returns {Promise<object>} Um objeto contendo os detalhes do chamado e suas mensagens.
 */
export const getChamadoDetalhesAsync = async (chamadoId) => {
    console.log(`--- TicketService: Iniciando getChamadoDetalhesAsync para ID: ${chamadoId} ---`);
    const token = await getToken();
    if (!token) {
        console.error("--- TicketService (Detalhes): Token nulo ou vazio.");
        throw new Error('Usuário não autenticado.');
    }

    // --- URL CORRIGIDA ---
    const url = `${CHAMADOS_API_URL}/${chamadoId}`; // Usa a constante definida no topo

    try {
        console.log(`--- TicketService (Detalhes): Fazendo fetch para ${url}`);
        const response = await fetch(url, {
            headers: createAuthHeaders(token),
        });
        console.log("--- TicketService (Detalhes): Resposta do fetch recebida. Status:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("--- TicketService (Detalhes): Detalhes recebidos.");
            return data;
        } else {
            const errorMessage = await handleApiError(response, 'Falha ao carregar detalhes do chamado.');
            console.error("--- TicketService (Detalhes): Erro da API:", errorMessage);
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error(`--- TicketService (Detalhes): Erro CRÍTICO em getChamadoDetalhesAsync (ID: ${chamadoId}):`, error);
        if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para buscar detalhes.');
        }
        throw error instanceof Error ? error : new Error('Erro de conexão ao buscar detalhes do chamado.');
    }
};

/**
 * Cria um novo chamado para o cliente logado. (POST /api/Chamados)
 * @param {string} titulo - Título do chamado.
 * @param {string} descricao - Descrição do chamado.
 * @param {string} prioridade - Prioridade ('BAIXA', 'MEDIA', 'ALTA').
 * @returns {Promise<object>} O objeto do chamado recém-criado (depende da resposta da API).
 */
export const createChamadoAsync = async (titulo, descricao, prioridade) => {
    console.log("--- TicketService: Iniciando createChamadoAsync ---");
    const token = await getToken();
    if (!token) {
         console.error("--- TicketService (Create): Token nulo ou vazio.");
         throw new Error('Usuário não autenticado.');
    }

    // --- URL CORRIGIDA ---
    const url = CHAMADOS_API_URL; // Usa a constante definida no topo

    try {
        console.log(`--- TicketService (Create): Fazendo POST para ${url}`);
        const response = await fetch(url, {
            method: 'POST',
            headers: createAuthHeaders(token),
            body: JSON.stringify({ titulo, descricao, prioridade }),
        });
         console.log("--- TicketService (Create): Resposta do POST recebida. Status:", response.status);

        if (response.ok) {
             console.log("--- TicketService (Create): Chamado criado com sucesso.");
             // Trata resposta 201 (Created) ou 200 (OK) que podem ter corpo
             if (response.status === 201 || response.status === 200) {
                 try {
                     // Tenta parsear o JSON retornado pela API
                     return await response.json();
                 } catch {
                     // Se não houver corpo ou não for JSON, retorna sucesso genérico
                     return { success: true };
                 }
             }
             // Para outros status de sucesso (ex: 204 No Content)
             return { success: true };
        } else {
            const errorMessage = await handleApiError(response, 'Não foi possível abrir o chamado.');
            console.error("--- TicketService (Create): Erro da API:", errorMessage);
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error('--- TicketService (Create): Erro CRÍTICO em createChamadoAsync:', error);
         if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para criar chamado.');
        }
        throw error instanceof Error ? error : new Error('Erro de conexão ao criar chamado.');
    }
};

/**
 * Envia uma nova mensagem em um chamado existente. (POST /api/Chamados/{id}/mensagens)
 * @param {number|string} chamadoId - O ID do chamado.
 * @param {string} texto - O conteúdo da mensagem.
 * @returns {Promise<object>} A objeto da mensagem recém-criada (depende da resposta da API).
 */
export const sendMessageAsync = async (chamadoId, texto) => {
    console.log(`--- TicketService: Iniciando sendMessageAsync para Chamado ID: ${chamadoId} ---`);
    const token = await getToken();
    if (!token) {
        console.error("--- TicketService (SendMsg): Token nulo ou vazio.");
        throw new Error('Usuário não autenticado.');
    }

    // --- URL CORRIGIDA ---
    const url = `${CHAMADOS_API_URL}/${chamadoId}/mensagens`; // Usa a constante definida no topo

    try {
        console.log(`--- TicketService (SendMsg): Fazendo POST para ${url}`);
        const response = await fetch(url, {
            method: 'POST',
            headers: createAuthHeaders(token),
            body: JSON.stringify({ texto }),
        });
        console.log("--- TicketService (SendMsg): Resposta do POST recebida. Status:", response.status);

        if (response.ok) {
            console.log("--- TicketService (SendMsg): Mensagem enviada com sucesso.");
             // Trata resposta 201 (Created) ou 200 (OK) que podem ter corpo
             if (response.status === 201 || response.status === 200) {
                 try {
                     // Tenta parsear o JSON retornado pela API (a nova mensagem)
                     return await response.json();
                 } catch {
                     return { success: true };
                 }
             }
            // Para outros status de sucesso (ex: 204 No Content)
            return { success: true };
        } else {
            const errorMessage = await handleApiError(response, 'Não foi possível enviar a mensagem.');
            console.error("--- TicketService (SendMsg): Erro da API:", errorMessage);
            throw new Error(errorMessage);
        }
    } catch (error) {
        console.error(`--- TicketService (SendMsg): Erro CRÍTICO em sendMessageAsync (Chamado ID: ${chamadoId}):`, error);
        if (error instanceof TypeError && error.message.includes('Network request failed')) {
             throw new Error('Não foi possível conectar ao servidor para enviar mensagem.');
        }
        throw error instanceof Error ? error : new Error('Erro de conexão ao enviar mensagem.');
    }
};