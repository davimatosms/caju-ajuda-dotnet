import React from 'react';
import styles from './LandingPage.module.css';
import { Link } from 'react-router-dom';

function LandingPage() {
  return (
    <div>
      <section className={styles.heroSection}>
        <h1>O suporte técnico que <span className={styles.highlight}>resolve</span>.</h1>
        <p>Simplifique a abertura de chamados, acelere a resolução de problemas e otimize a produtividade da sua equipe com a ajuda da nossa IA.</p>
        <Link to="/register" className={styles.ctaButton}>Comece Agora de Graça</Link>
      </section>

      {/* SEÇÃO DE FUNCIONALIDADES (NOVA) */}
      <section className={styles.section}>
        <h2 className={styles.sectionTitle}>Funcionalidades Principais</h2>
        <div className={styles.featuresGrid}>
          <div className={styles.featureCard}>
            <h3>Priorização com IA</h3>
            <p>Nossa inteligência artificial analisa o conteúdo de cada chamado para atribuir a prioridade correta, garantindo que os problemas mais críticos sejam vistos primeiro.</p>
          </div>
          <div className={styles.featureCard}>
            <h3>Interface Centralizada</h3>
            <p>Acompanhe o andamento de todos os seus chamados em um dashboard simples e intuitivo. Responda, anexe arquivos e veja o histórico completo em um só lugar.</p>
          </div>
          <div className={styles.featureCard}>
            <h3>Métricas para Gestão</h3>
            <p>O painel do administrador oferece gráficos e métricas em tempo real sobre a performance da equipe de suporte, ajudando na tomada de decisões estratégicas.</p>
          </div>
        </div>
      </section>

      {/* SEÇÃO COMO FUNCIONA (NOVA) */}
      <section className={`${styles.section} ${styles.howItWorksSection}`}>
        <h2 className={styles.sectionTitle}>Como Funciona?</h2>
        <div className={styles.howItWorksGrid}>
          <div className={styles.stepCard}>
            <h3><span className={styles.stepNumber}>1</span> Crie sua Conta</h3>
            <p>Faça um registro rápido e gratuito para ter acesso imediato à nossa plataforma de suporte.</p>
          </div>
          <div className={styles.stepCard}>
            <h3><span className={styles.stepNumber}>2</span> Abra um Chamado</h3>
            <p>Descreva seu problema em nosso formulário simples. Nossa IA analisará e priorizará seu ticket na hora.</p>
          </div>
          <div className={styles.stepCard}>
            <h3><span className={styles.stepNumber}>3</span> Acompanhe e Resolva</h3>
            <p>Interaja com a equipe de suporte e acompanhe o progresso em tempo real até que seu problema seja resolvido.</p>
          </div>
        </div>
      </section>
    </div>
  );
}

export default LandingPage;