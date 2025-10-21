// src/config.js

// --- IMPORTANTE: MANTENHA ESTE IP ATUALIZADO ---
// Este deve ser o IP da sua máquina onde a API está rodando,
// visível pelo seu emulador/celular na mesma rede Wi-Fi.
const DEV_API_IP = '192.168.15.12';

// URLs base para desenvolvimento
export const API_BASE_URL = `http://${DEV_API_IP}:8080/api`;
export const SOCKET_URL = `http://${DEV_API_IP}:9092`;

// Você pode adicionar URLs de produção aqui depois:
// const PROD_API_URL = 'https://sua-api.com/api';
// const PROD_SOCKET_URL = 'https://seu-socket.com';
// export const API_BASE_URL = __DEV__ ? `http://${DEV_API_IP}:8080/api` : PROD_API_URL;
// export const SOCKET_URL = __DEV__ ? `http://${DEV_API_IP}:9092` : PROD_SOCKET_URL;