import axios from 'axios';
import AuthService from './AuthService'; 

const API_URL = 'http://localhost:5205/api/perfil';

// --- INTERFACES ---

export interface Perfil {
    nome: string;
    email: string;
}

export interface PerfilUpdateData {
    Nome: string;
    Email: string;
}

export interface SenhaUpdateData {
    SenhaAtual: string;
    NovaSenha: string;
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

const getPerfil = async (): Promise<Perfil> => {
    const config = getAuthHeaders();
    const response = await axios.get(API_URL, config);
    return response.data;
};

const updatePerfil = async (data: PerfilUpdateData): Promise<void> => {
    const config = getAuthHeaders();
    await axios.put(API_URL, data, config);
};

const updateSenha = async (data: SenhaUpdateData): Promise<void> => {
    const config = getAuthHeaders();
    await axios.patch(`${API_URL}/senha`, data, config);
};


const PerfilService = {
    getPerfil,
    updatePerfil,
    updateSenha,
};

export default PerfilService;