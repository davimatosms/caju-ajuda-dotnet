import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5205';
const API_URL = `${API_BASE}/api/admin`;

// --- INTERFACES ---

export interface Tecnico {
    id: number;
    nome: string;
    email: string;
    enabled: boolean;
}
export interface TecnicoCreateData {
    Nome?: string;
    Email?: string;
    Senha?: string;
}
export interface TecnicoUpdateData {
    Nome: string;
    Email: string;
}
export interface Cliente {
    id: number;
    nome: string;
    email: string;
    enabled: boolean;
}

export interface ChartDataPoint {
    name: string;
    total: number;
}
export interface DailyStat {
    dia: string;
    criados: number;
    fechados: number;
}
export interface DashboardMetrics {
    totalChamados: number;
    chamadosAbertos: number;
    chamadosEmAndamento: number;
    chamadosFechados: number;
    percentualResolvidos: number;
    chamadosPorPrioridade: ChartDataPoint[];
    tempoMedioPrimeiraRespostaHoras: number;
    tempoMedioResolucaoHoras: number;
    statsUltimos7Dias: DailyStat[];
}

// --- FUNÇÃO AUXILIAR ---

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

// --- FUNÇÕES DO SERVIÇO ---

const getTecnicos = async (): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/tecnicos`, config);
    return response.data;
};

const createTecnico = async (data: TecnicoCreateData): Promise<Tecnico> => {
    const config = getAuthHeaders();
    const response = await axios.post(`${API_URL}/tecnicos`, data, config);
    return response.data;
};

const toggleTecnicoStatus = async (id: number): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.patch(`${API_URL}/tecnicos/${id}/status`, null, config);
    return response.data;
};

const updateTecnico = async (id: number, data: TecnicoUpdateData): Promise<Tecnico> => {
    const config = getAuthHeaders();
    const response = await axios.put(`${API_URL}/tecnicos/${id}`, data, config);
    return response.data;
};

const resetPassword = async (id: number): Promise<{ temporaryPassword: string }> => {
    const config = getAuthHeaders();
    const response = await axios.post(`${API_URL}/tecnicos/${id}/reset-password`, null, config);
    return response.data;
};

const getClientes = async (): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/clientes`, config);
    return response.data;
};

const toggleClienteStatus = async (id: number): Promise<any> => {
    const config = getAuthHeaders();
    const response = await axios.patch(`${API_URL}/clientes/${id}/status`, null, config);
    return response.data;
};

const getDashboardMetrics = async (): Promise<DashboardMetrics> => {
    const config = getAuthHeaders();
    const response = await axios.get(`${API_URL}/dashboard`, config);
    return response.data;
};

const AdminService = {
    getTecnicos,
    createTecnico,
    toggleTecnicoStatus,
    updateTecnico,
    resetPassword,
    getClientes,
    toggleClienteStatus,
    getDashboardMetrics,
};

export default AdminService;