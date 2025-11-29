import React from 'react';
import { Link } from 'react-router-dom';
import HeroCaju from '../../components/HeroCaju/HeroCaju';

function LandingPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      {/* Hero Section */}
      <section className="relative overflow-hidden bg-gradient-to-br from-primary-500 via-primary-600 to-primary-700 text-white">
        <div className="absolute inset-0 bg-black opacity-5"></div>
        <div className="container mx-auto px-4 py-20 md:py-32 relative z-10">
          <div className="max-w-4xl mx-auto text-center">
            <h1 className="text-4xl md:text-6xl font-bold mb-6 leading-tight animate-fade-in">
              O suporte técnico que <span className="text-yellow-300 underline decoration-wavy">resolve</span>.
            </h1>
            <p className="text-lg md:text-xl mb-8 text-primary-50 max-w-2xl mx-auto">
              Simplifique a abertura de chamados, acelere a resolução de problemas e otimize a produtividade da sua equipe com a ajuda da nossa IA.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center mb-12">
              <Link to="/register" className="btn-primary text-lg px-8 py-3 bg-white text-primary-600 hover:bg-gray-100 shadow-lg hover:shadow-xl transform hover:scale-105 transition-all">
                Comece Agora de Graça
              </Link>
              <Link to="/login" className="btn-secondary text-lg px-8 py-3 bg-transparent border-2 border-white text-white hover:bg-white hover:text-primary-600">
                Já tenho conta
              </Link>
            </div>
            <div className="mt-12">
              <HeroCaju />
            </div>
          </div>
        </div>
        {/* Wave divider */}
        <div className="absolute bottom-0 left-0 right-0">
          <svg viewBox="0 0 1440 120" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M0 120L60 110C120 100 240 80 360 70C480 60 600 60 720 65C840 70 960 80 1080 85C1200 90 1320 90 1380 90L1440 90V120H1380C1320 120 1200 120 1080 120C960 120 840 120 720 120C600 120 480 120 360 120C240 120 120 120 60 120H0Z" fill="rgb(249 250 251)" />
          </svg>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20 px-4">
        <div className="container mx-auto max-w-7xl">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4">
              Funcionalidades Principais
            </h2>
            <p className="text-lg text-gray-600 max-w-2xl mx-auto">
              Tudo que você precisa para gerenciar seu suporte de forma inteligente
            </p>
          </div>
          
          <div className="grid md:grid-cols-3 gap-8">
            {/* Feature 1 */}
            <div className="card group hover:shadow-caju-lg transition-all duration-300 hover:-translate-y-2">
              <div className="flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-xl mb-4 group-hover:bg-primary-500 group-hover:text-white transition-colors">
                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Priorização com IA</h3>
              <p className="text-gray-600">
                Nossa inteligência artificial analisa o conteúdo de cada chamado para atribuir a prioridade correta, garantindo que os problemas mais críticos sejam vistos primeiro.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="card group hover:shadow-caju-lg transition-all duration-300 hover:-translate-y-2">
              <div className="flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-xl mb-4 group-hover:bg-primary-500 group-hover:text-white transition-colors">
                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Interface Centralizada</h3>
              <p className="text-gray-600">
                Acompanhe o andamento de todos os seus chamados em um dashboard simples e intuitivo. Responda, anexe arquivos e veja o histórico completo em um só lugar.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="card group hover:shadow-caju-lg transition-all duration-300 hover:-translate-y-2">
              <div className="flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-xl mb-4 group-hover:bg-primary-500 group-hover:text-white transition-colors">
                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 8v8m-4-5v5m-4-2v2m-2 4h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Métricas para Gestão</h3>
              <p className="text-gray-600">
                O painel do administrador oferece gráficos e métricas em tempo real sobre a performance da equipe de suporte, ajudando na tomada de decisões estratégicas.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* How it Works Section */}
      <section className="py-20 px-4 bg-white">
        <div className="container mx-auto max-w-7xl">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4">
              Como Funciona?
            </h2>
            <p className="text-lg text-gray-600 max-w-2xl mx-auto">
              Três passos simples para resolver seus problemas
            </p>
          </div>

          <div className="grid md:grid-cols-3 gap-12">
            {/* Step 1 */}
            <div className="text-center relative">
              <div className="flex items-center justify-center w-16 h-16 bg-primary-500 text-white text-2xl font-bold rounded-full mx-auto mb-6 shadow-caju">
                1
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Crie sua Conta</h3>
              <p className="text-gray-600">
                Faça um registro rápido e gratuito para ter acesso imediato à nossa plataforma de suporte.
              </p>
              {/* Connector Line */}
              <div className="hidden md:block absolute top-8 left-1/2 w-full h-0.5 bg-gradient-to-r from-primary-300 to-primary-200 -z-10"></div>
            </div>

            {/* Step 2 */}
            <div className="text-center relative">
              <div className="flex items-center justify-center w-16 h-16 bg-primary-500 text-white text-2xl font-bold rounded-full mx-auto mb-6 shadow-caju">
                2
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Abra um Chamado</h3>
              <p className="text-gray-600">
                Descreva seu problema em nosso formulário simples. Nossa IA analisará e priorizará seu ticket na hora.
              </p>
              {/* Connector Line */}
              <div className="hidden md:block absolute top-8 left-1/2 w-full h-0.5 bg-gradient-to-r from-primary-300 to-primary-200 -z-10"></div>
            </div>

            {/* Step 3 */}
            <div className="text-center">
              <div className="flex items-center justify-center w-16 h-16 bg-primary-500 text-white text-2xl font-bold rounded-full mx-auto mb-6 shadow-caju">
                3
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">Acompanhe e Resolva</h3>
              <p className="text-gray-600">
                Interaja com a equipe de suporte e acompanhe o progresso em tempo real até que seu problema seja resolvido.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 px-4 bg-gradient-to-br from-primary-600 to-primary-700 text-white">
        <div className="container mx-auto max-w-4xl text-center">
          <h2 className="text-3xl md:text-4xl font-bold mb-6">
            Pronto para transformar seu suporte técnico?
          </h2>
          <p className="text-xl mb-8 text-primary-50">
            Junte-se a centenas de empresas que já melhoraram sua gestão de suporte com Caju Ajuda
          </p>
          <Link to="/register" className="inline-block btn-primary text-lg px-8 py-3 bg-white text-primary-600 hover:bg-gray-100 shadow-lg hover:shadow-xl transform hover:scale-105 transition-all">
            Começar Gratuitamente →
          </Link>
        </div>
      </section>
    </div>
  );
}

export default LandingPage;