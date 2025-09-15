import axios from 'axios';

const API_URL = 'http://localhost:5205/api/knowledgebase';

// --- INTERFACES ---

export interface Artigo {
    id: number;
    titulo: string;
    conteudo: string;
}

export interface Categoria {
    id: number;
    nome: string;
    artigos: { $values: Artigo[] }; // Lidando com a referência de ciclo do backend
}

/**
 * Busca todas as categorias com seus respectivos artigos.
 */
const getCategoriasComArtigos = async (): Promise<any> => {
    const response = await axios.get(`${API_URL}/categorias`);
    return response.data;
};

/**
 * Pesquisa artigos por um termo específico.
 */
const searchArtigos = async (termo: string): Promise<Artigo[]> => {
    const response = await axios.get(`${API_URL}/search`, {
        params: { termo }
    });
    return response.data;
};

const KnowledgeBaseService = {
    getCategoriasComArtigos,
    searchArtigos,
};

export default KnowledgeBaseService;