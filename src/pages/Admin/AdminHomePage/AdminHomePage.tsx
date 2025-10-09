import React from 'react';
import { Link } from 'react-router-dom';
import styles from './AdminHomePage.module.css';

function AdminHomePage() {
    // Futuramente, podemos pegar o nome do admin logado para personalizar a saudação
    const adminName = "Admin";

    return (
        <div className={styles.homeContainer}>
            <div className={styles.welcomeHeader}>
                <h1>Bem-vindo, {adminName}!</h1>
                <p>Selecione uma das opções abaixo para gerenciar o sistema.</p>
            </div>
            <div className={styles.navGrid}>
                <Link to="/admin/dashboard" className={styles.navCard}>
                    <h2>Ver Dashboard</h2>
                    <p>Acesse as métricas e os gráficos para uma visão geral do suporte.</p>
                </Link>
                <Link to="/admin/tecnicos" className={styles.navCard}>
                    <h2>Gerenciar Técnicos</h2>
                    <p>Adicione, edite e gerencie as contas da sua equipe de suporte técnico.</p>
                </Link>
                <Link to="/admin/clientes" className={styles.navCard}>
                    <h2>Gerenciar Clientes</h2>
                    <p>Visualize e administre as contas de todos os clientes do sistema.</p>
                </Link>
            </div>
        </div>
    );
}

export default AdminHomePage;