import axios from 'axios';
import API_CONFIG from '../config';

// A URL base da nossa API de chamados
const API_BASE = API_CONFIG.baseURL;
const API_URL = `${API_BASE}/api/chamados`;

console.log('[ChamadoService] API_BASE:', API_BASE);
console.log('[ChamadoService] API_URL:', API_URL);

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
    autorId: number;
    isNotaInterna: boolean;
    lidoPeloCliente: boolean;
}

export interface ChamadoDetail extends Chamado {
    descricao: string;
    nomeCliente: string;
    nomeTecnicoResponsavel?: string;
    dataFechamento?: string;
    mensagens: Mensagem[] | { $values: Mensagem[] }; // Suporta ambos os formatos
    sugestaoIA?: string; // Sugestão gerada pela IA (opcional)
}

export interface Anexo {
    id: number;
    nomeArquivo: string;
    tipoArquivo: string;
    chamadoId: number;
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
        console.warn('[ChamadoService] Token não encontrado');
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

/**
 * NOVA FUNÇÃO: Faz upload de um anexo para um chamado existente.
 */
const uploadAnexo = async (chamadoId: number, file: File): Promise<any> => {
    const token = JSON.parse(localStorage.getItem('user_token') || 'null');
    if (!token) {
        throw new Error("Nenhum token encontrado.");
    }

    const formData = new FormData();
    formData.append('file', file);

    const response = await axios.post(`${API_URL}/${chamadoId}/anexos`, formData, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
};

/**
 * NOVA FUNÇÃO: Busca todos os anexos de um chamado.
 */
const getAnexosByChamado = async (chamadoId: number): Promise<Anexo[]> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/${chamadoId}/anexos`, config);
    return response.data;
};

/**
 * NOVA FUNÇÃO: Faz download de um anexo.
 */
const downloadAnexo = async (anexoId: number): Promise<Blob> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/anexos/${anexoId}/download`, {
        ...config,
        responseType: 'blob'
    });
    return response.data;
};

const ChamadoService = {
    getMeusChamados,
    getChamadoById,
    createChamado,
    addMensagem, // Exportamos a nova função
    uploadAnexo, // Exportamos a função de upload
    getAnexosByChamado, // Exportamos a função de listar anexos
    downloadAnexo, // Exportamos a função de download
};

export default ChamadoService;