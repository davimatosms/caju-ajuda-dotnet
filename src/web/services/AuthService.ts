import axios from 'axios';

// A URL base da nossa API. Verifique a porta no seu launchSettings.json do backend.
const API_URL = 'https://localhost:7113/api/auth';

// DTO para os dados de registo
interface RegisterData {
    nome?: string;
    email?: string;
    senha?: string;
}

const register = (data: RegisterData) => {
    return axios.post(`${API_URL}/register/cliente`, data);
};

const AuthService = {
    register,
    // Futuramente, adicionaremos aqui a função de login
};

export default AuthService;