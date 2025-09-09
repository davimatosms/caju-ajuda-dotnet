import axios from 'axios';

// A URL base da API de chamados
const API_URL = 'http://localhost:5205/api/chamados';

// --- INTERFACES (Nossos "Contratos" de Dados) ---

// Define a estrutura de um chamado na lista do dashboard
export interface Chamado {
    id: number;
    titulo: string;
    status: string;
    prioridade: string;
    dataCriacao: string;
}

// Define a estrutura dos dados para criar um novo chamado
export interface ChamadoCreateData {
    Titulo: string;
    Descricao: string;
}

// --- FUNÇÕES DO SERVIÇO ---

/**
 * Pega o token do localStorage e monta o cabeçalho de autorização.
 * Criamos uma função auxiliar para não repetir o código.
 */
const getAuthHeaders = () => {
    const token = JSON.parse(localStorage.getItem('user_token') || 'null');
    if (!token) {
        throw new Error("Nenhum token encontrado.");
    }
    return {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    };
};

/**
 * Busca os chamados do usuário logado.
 */
const getMeusChamados = async (): Promise<Chamado[]> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/meus`, config);
    return response.data;
};

/**
 * Cria um novo chamado.
 */
const createChamado = async (data: ChamadoCreateData): Promise<any> => {
    const config = getAuthHeaders();
    // Faz a chamada POST para a raiz do controller de chamados
    const response = await axios.post(API_URL, data, config);
    return response.data;
};


const ChamadoService = {
    getMeusChamados,
    createChamado, // Exportamos a nova função
};

export default ChamadoService;