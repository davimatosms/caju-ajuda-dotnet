import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminService, { DashboardMetrics } from '../../../services/AdminService';
import styles from './DashboardAdminPage.module.css';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, LineChart, Line } from 'recharts';

function DashboardAdminPage() {
    const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchMetrics = async () => {
            try {
                const data = await AdminService.getDashboardMetrics();
                setMetrics(data);
            } catch (err: any) {
                if (err.response && (err.response.status === 401 || err.response.status === 403)) {
                    navigate('/login');
                } else {
                    setError("Não foi possível carregar as métricas do dashboard.");
                }
            } finally {
                setIsLoading(false);
            }
        };
        fetchMetrics();
    }, [navigate]);

    if (isLoading) {
        return <div className={styles.loading}>Carregando métricas...</div>;
    }
    if (error) {
        return <div className={styles.error}>{error}</div>;
    }
    if (!metrics) {
        return <div className={styles.error}>Nenhuma métrica encontrada.</div>;
    }

    return (
        <div className={styles.dashboardContainer}>
            <div className={styles.header}>
                <h1>Dashboard do Administrador</h1>
            </div>
            
            <div className={styles.statsGrid}>
                <div className={styles.statCard}>
                    <h3>Total de Chamados</h3>
                    <p className={styles.statValue}>{metrics.totalChamados}</p>
                </div>
                <div className={styles.statCard}>
                    <h3>Chamados Abertos</h3>
                    <p className={styles.statValue}>{metrics.chamadosAbertos}</p>
                </div>
                <div className={styles.statCard}>
                    <h3>Tempo Médio de Resposta</h3>
                    <p className={styles.statValue}>{metrics.tempoMedioPrimeiraRespostaHoras.toFixed(1)}h</p>
                </div>
                <div className={styles.statCard}>
                    <h3>Tempo Médio de Resolução</h3>
                    <p className={styles.statValue}>{metrics.tempoMedioResolucaoHoras.toFixed(1)}h</p>
                </div>
            </div>

            <div className={styles.chartsGrid}>
                <div className={styles.chartContainer}>
                    <h3>Atividade nos Últimos 7 Dias</h3>
                    <ResponsiveContainer width="100%" height={300}>
                        <LineChart data={metrics.statsUltimos7Dias}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="dia" />
                            <YAxis allowDecimals={false} />
                            <Tooltip />
                            <Legend />
                            <Line type="monotone" dataKey="criados" name="Criados" stroke="#8884d8" />
                            <Line type="monotone" dataKey="fechados" name="Fechados" stroke="#82ca9d" />
                        </LineChart>
                    </ResponsiveContainer>
                </div>

                <div className={styles.chartContainer}>
                    <h3>Chamados por Prioridade</h3>
                     <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={metrics.chamadosPorPrioridade}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="name" />
                            <YAxis allowDecimals={false} />
                            <Tooltip />
                            <Legend />
                            <Bar dataKey="total" name="Total" fill="#8884d8" />
                        </BarChart>
                    </ResponsiveContainer>
                </div>
            </div>
        </div>
    );
}

export default DashboardAdminPage;