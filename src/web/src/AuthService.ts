import axios from 'axios';

// A URL base da nossa API.
const API_URL = 'http://localhost:5205/api/auth';

// DTO para os dados de registo
interface RegisterData {
    Nome?: string;
    Email?: string;
    Senha?: string;
}

// DTO para os dados de login
interface LoginData {
    Email?: string;
    Senha?: string;
}

const register = (data: RegisterData) => {
    return axios.post(`${API_URL}/register/cliente`, data);
};

// Nova função de login
const login = async (data: LoginData) => {
    // Faz a chamada para o endpoint de login
    const response = await axios.post(`${API_URL}/login`, data);
    
    // Se a resposta contiver um token, salva no localStorage
    if (response.data.token) {
        localStorage.setItem('user_token', JSON.stringify(response.data.token));
    }
    
    return response.data;
};

const AuthService = {
    register,
    login, // Exportamos a nova função
};

export default AuthService;