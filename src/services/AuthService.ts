import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const API_URL = 'http://localhost:5205/api/auth';

interface RegisterData {
    Nome?: string;
    Email?: string;
    Senha?: string;
}

interface LoginData {
    Email?: string;
    Senha?: string;
}

interface DecodedToken {
    email: string;
    role: 'CLIENTE' | 'TECNICO' | 'ADMIN';
    // Adicione outras propriedades do token se houver (exp, iat, etc.)
}

const register = (data: RegisterData) => {
    return axios.post(`${API_URL}/register/cliente`, data);
};

const login = async (data: LoginData) => {
    const response = await axios.post(`${API_URL}/login`, data);
    if (response.data.token) {
        localStorage.setItem('user_token', JSON.stringify(response.data.token));
    }
    return response.data;
};

const logout = () => {
    localStorage.removeItem('user_token');
};

const getCurrentUser = (): DecodedToken | null => {
    const tokenString = localStorage.getItem('user_token');
    if (!tokenString) {
        return null;
    }

    try {
        const token = JSON.parse(tokenString);
        const decodedToken: DecodedToken = jwtDecode(token);
        return decodedToken;
    } catch (error) {
        console.error("Erro ao decodificar o token", error);
        return null;
    }
};

const AuthService = {
    register,
    login,
    logout,
    getCurrentUser,
};

export default AuthService;