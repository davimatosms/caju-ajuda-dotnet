import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { Cliente } from '../../../services/AdminService';
import styles from './GerenciarClientesPage.module.css';

function GerenciarClientesPage() {
    const [clientes, setClientes] = useState<Cliente[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchClientes = async () => {
            try {
                const response = await AdminService.getClientes();
                setClientes(response?.$values || response || []);
            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar a lista de clientes.");
                }
            } finally {
                setIsLoading(false);
            }
        };

        fetchClientes();
    }, [navigate]);

    if (isLoading) {
        return <div className={styles.loading}>Carregando clientes...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }

    return (
        <div className={styles.pageContainer}>
            <div className={styles.header}>
                <h1>Gerenciar Clientes</h1>
            </div>
            <table className={styles.clientesTable}>
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>E-mail</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {clientes.map(cliente => (
                        <tr key={cliente.id}>
                            <td>{cliente.nome}</td>
                            <td>{cliente.email}</td>
                            <td>
                                <span className={`${styles.statusBadge} ${cliente.enabled ? styles.statusAtivo : styles.statusInativo}`}>
                                    {cliente.enabled ? 'Ativo' : 'Inativo'}
                                </span>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default GerenciarClientesPage;