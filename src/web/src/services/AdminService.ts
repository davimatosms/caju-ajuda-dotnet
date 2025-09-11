import axios from 'axios';

const API_URL = 'http://localhost:5205/api/admin';

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

const AdminService = {
    getTecnicos,
    createTecnico,
};

export default AdminService;