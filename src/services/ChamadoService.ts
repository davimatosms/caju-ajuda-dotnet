import axios from 'axios';

// A URL base da nossa API de chamados
const API_URL = 'http://localhost:5205/api/chamados';

// --- INTERFACES (Nossos "Contratos" de Dados) ---

export interface Chamado {
    id: number;
    titulo: string;
    status: string;
    prioridade: string;
    dataCriacao: string;
}

export interface Mensagem {
    id: number;
    texto: string;
    dataEnvio: string;
    autorNome: string;
    isNotaInterna: boolean;
}

export interface ChamadoDetail extends Chamado {
    descricao: string;
    nomeCliente: string;
    nomeTecnicoResponsavel?: string;
    dataFechamento?: string;
    mensagens: { $values: Mensagem[] };
}

export interface ChamadoCreateData {
    Titulo: string;
    Descricao: string;
}

// NOVA INTERFACE: Define o formato para enviar uma nova mensagem
export interface MensagemCreateData {
    Texto: string;
}

// --- FUNÇÕES DO SERVIÇO ---

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

const getMeusChamados = async (): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/meus`, config);
    return response.data;
};

const getChamadoById = async (id: number): Promise<ChamadoDetail> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/${id}`, config);
    return response.data;
};

const createChamado = async (data: ChamadoCreateData): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.post(API_URL, data, config);
    return response.data;
};

/**
 * NOVA FUNÇÃO: Adiciona uma nova mensagem a um chamado existente.
 */
const addMensagem = async (chamadoId: number, data: MensagemCreateData): Promise<Mensagem> => {
    const config = getAuthHeaders();
    const response = await axios.post(`${API_URL}/${chamadoId}/mensagens`, data, config);
    return response.data;
};

const ChamadoService = {
    getMeusChamados,
    getChamadoById,
    createChamado,
    addMensagem, // Exportamos a nova função
};

export default ChamadoService;