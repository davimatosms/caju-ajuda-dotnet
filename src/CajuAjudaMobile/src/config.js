// src/config.js

// --- Configurações de Desenvolvimento ---

// IP da sua máquina onde o backend está rodando (confirmado via ipconfig)
const DEV_IP = '192.168.15.11';

// Porta onde sua API ASP.NET Core está rodando (confirmada pelos logs de inicialização)
const DEV_API_PORT = 5205;

// !!! ATENÇÃO: Verifique se esta é a porta correta do seu servidor Socket.IO/SignalR Hub !!!
// (Se o Hub SignalR estiver rodando junto com a API ASP.NET Core, esta porta será a mesma da API, ou seja, 5205)
// Se você não configurou um servidor Socket.IO separado na porta 9092 e está usando apenas o SignalR
// integrado ao ASP.NET Core, você pode remover ou comentar a linha SOCKET_URL abaixo.
const DEV_SOCKET_PORT = 9092; // <- PENDENTE DE CONFIRMAÇÃO

// --- URLs Exportadas ---

// URL base completa para a API REST em ambiente de desenvolvimento
// Ex: http://192.168.15.11:5205/api
export const API_BASE_URL = `http://${DEV_IP}:${DEV_API_PORT}/api`;

// URL completa para o Hub SignalR em ambiente de desenvolvimento
// Ex: http://192.168.15.11:5205/notificacaoHub
// (Note que usa a mesma porta da API, pois está mapeado no Program.cs)
export const SIGNALR_HUB_URL = `http://${DEV_IP}:${DEV_API_PORT}/notificacaoHub`;

// (Comentado - Exemplo de como era para Socket.IO, pode remover se não usar)
// export const SOCKET_URL = `http://${DEV_IP}:${DEV_SOCKET_PORT}`;


// --- Configurações de Produção (Exemplo - Preencher depois) ---
// const PROD_API_URL = 'https://sua-api-de-producao.azurewebsites.net/api';
// const PROD_SIGNALR_HUB_URL = 'https://sua-api-de-producao.azurewebsites.net/notificacaoHub';


// --- Exportação Condicional (Opcional) ---
// Descomente as linhas abaixo se quiser alternar automaticamente entre DEV e PROD
// (A variável global __DEV__ é definida automaticamente pelo React Native/Expo)

// export const API_BASE_URL = __DEV__ ? `http://${DEV_IP}:${DEV_API_PORT}/api` : PROD_API_URL;
// export const SIGNALR_HUB_URL = __DEV__ ? `http://${DEV_IP}:${DEV_API_PORT}/notificacaoHub` : PROD_SIGNALR_HUB_URL;

// Exporta as portas individualmente (pode ser útil para debug ou configurações)
export const API_PORT = DEV_API_PORT;
// export const SOCKET_PORT = DEV_SOCKET_PORT; // Comentado pois provavelmente não será usado

console.log(`--- Config: API rodando em ${API_BASE_URL}`);
console.log(`--- Config: SignalR Hub em ${SIGNALR_HUB_URL}`);