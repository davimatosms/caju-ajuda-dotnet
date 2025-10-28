import { API_BASE_URL } from '../config';

/**
 * Tenta fazer login na API.
 * @param {string} email - O email do usuário.
 * @param {string} senha - A senha do usuário.
 * @returns {Promise<object|null>} - Retorna o objeto de resposta da API (com token) ou null em caso de falha.
 */
export const loginAsync = async (email, senha) => {
    const loginUrl = `${API_BASE_URL}/auth/login`;
    try {
        const response = await fetch(loginUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: email, senha: senha }),
        });

        if (response.ok) {
            const data = await response.json(); // Assumindo que a API retorna JSON com { token, ... }
            return data;
        } else {
            // Tenta ler a mensagem de erro do corpo da resposta
            const errorText = await response.text();
            console.error('Falha no Login - Resposta da API:', errorText);
            throw new Error(errorText || 'E-mail ou senha inválidos.');
        }
    } catch (error) {
        console.error('Erro de Conexão no Login:', error);
        // Re-lança o erro para que o componente possa tratá-lo (ex: mostrar Alert)
        throw new Error('Não foi possível conectar ao servidor.');
    }
};

/**
 * Tenta registrar um novo usuário e envia o email de verificação.
 * @param {string} nome - O nome do usuário.
 * @param {string} email - O email do usuário.
 * @param {string} senha - A senha do usuário.
 * @returns {Promise<boolean>} - Retorna true se o registro e envio de email foram bem-sucedidos.
 */
export const registerAndSendVerificationAsync = async (nome, email, senha) => {
    const registerUrl = `${API_BASE_URL}/auth/register`;
    const resendUrl = `${API_BASE_URL}/reenviar-verificacao`; // Ajuste se a rota for diferente

    try {
        // --- PASSO 1: Tenta registar o utilizador ---
        const registerResponse = await fetch(registerUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ nome, email, senha }),
        });

        if (!registerResponse.ok) {
            const errorText = await registerResponse.text();
            console.error('Falha no Registro - Resposta da API:', errorText);
            throw new Error(errorText || 'Não foi possível criar a sua conta.');
        }

        // --- PASSO 2: Se o registo funcionou, dispara o e-mail ---
        // Poderia haver um pequeno delay aqui se necessário
        const resendResponse = await fetch(resendUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: email }),
        });

        if (!resendResponse.ok) {
            // Mesmo que o registro tenha funcionado, o envio do email pode falhar.
            // Decida como lidar com isso (talvez só logar o erro, ou informar o usuário).
            const errorText = await resendResponse.text();
            console.warn('Registro OK, mas falha ao enviar email de verificação:', errorText);
            // Mesmo com falha no email, consideramos o registro um sucesso parcial
        }

        return true; // Sucesso no registro (e tentativa de envio de email)

    } catch (error) {
        console.error('Erro durante Registro/Envio de Email:', error);
        // Re-lança o erro original para o componente tratar
        throw error;
    }
};