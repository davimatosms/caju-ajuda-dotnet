import React, { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import KnowledgeBaseService, { Categoria, Artigo } from '../../services/KnowledgeBaseService';
import styles from './KnowledgeBasePage.module.css';

function KnowledgeBasePage() {
    const [categorias, setCategorias] = useState<Categoria[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // --- NOVOS ESTADOS PARA A BUSCA ---
    const [searchTerm, setSearchTerm] = useState('');
    const [searchResults, setSearchResults] = useState<Artigo[]>([]);
    const [isSearching, setIsSearching] = useState(false);

    // Busca as categorias iniciais
    useEffect(() => {
        const fetchCategorias = async () => {
            try {
                const response = await KnowledgeBaseService.getCategoriasComArtigos();
                setCategorias(response?.$values || response || []);
            } catch (err) {
                setError("Não foi possível carregar a base de conhecimento.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchCategorias();
    }, []);

    // Efeito para realizar a busca quando o usuário digita
    useEffect(() => {
        // Se o campo de busca está vazio, não faz nada
        if (searchTerm.trim() === '') {
            setSearchResults([]);
            return;
        }

        setIsSearching(true);
        // "Debounce": espera 500ms após o usuário parar de digitar para fazer a busca
        const delayDebounceFn = setTimeout(() => {
            KnowledgeBaseService.searchArtigos(searchTerm)
                .then(results => {
                    setSearchResults(results?.$values || results || []);
                })
                .catch(() => {
                    // Tratar erro de busca se necessário
                })
                .finally(() => {
                    setIsSearching(false);
                });
        }, 500);

        // Limpa o timer se o usuário digitar novamente
        return () => clearTimeout(delayDebounceFn);
    }, [searchTerm]);


    if (isLoading) {
        return <div className={styles.loading}>Carregando...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }

    return (
        <div>
            <section className={styles.searchSection}>
                <h1>Base de Conhecimento</h1>
                <p>Encontre respostas rápidas para suas dúvidas mais comuns.</p>
                <input 
                    type="search" 
                    placeholder="Pesquisar artigos por palavra-chave..." 
                    className={styles.searchBox}
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
            </section>

            <div className={styles.pageContainer}>
                {/* Renderização condicional: mostra os resultados da busca ou as categorias */}
                {searchTerm.trim() !== '' ? (
                    <div className={styles.searchResultsContainer}>
                        <h2>Resultados da Busca por "{searchTerm}"</h2>
                        {isSearching ? (
                            <p>Buscando...</p>
                        ) : searchResults.length > 0 ? (
                            <ul className={styles.articleList}>
                                {searchResults.map(artigo => (
                                    <li key={artigo.id}>
                                        <Link to={`/ajuda/artigo/${artigo.id}`}>{artigo.titulo}</Link>
                                    </li>
                                ))}
                            </ul>
                        ) : (
                            <p>Nenhum artigo encontrado.</p>
                        )}
                    </div>
                ) : (
                    <div className={styles.contentGrid}>
                        {categorias.map(categoria => (
                            <div key={categoria.id} className={styles.categoryCard}>
                                <h2>{categoria.nome}</h2>
                                <ul className={styles.articleList}>
                                    {categoria.artigos.$values.map(artigo => (
                                        <li key={artigo.id}>
                                            <Link to={`/ajuda/artigo/${artigo.id}`}>{artigo.titulo}</Link>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

export default KnowledgeBasePage;