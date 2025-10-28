import React from 'react';
import { Link } from 'react-router-dom';

const NotFoundPage: React.FC = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full text-center">
        <div className="mb-8">
          <h1 className="text-9xl font-bold text-primary-500">404</h1>
          <h2 className="text-3xl font-bold text-gray-800 mt-4 mb-2">
            Página não encontrada
          </h2>
          <p className="text-gray-600">
            A página que você está procurando não existe ou foi movida.
          </p>
        </div>

        <div className="space-y-4">
          <Link
            to="/"
            className="inline-block w-full px-6 py-3 bg-primary-500 hover:bg-primary-600 text-white font-semibold rounded-lg transition-colors duration-200"
          >
            Voltar para a Página Inicial
          </Link>
          
          <Link
            to="/dashboard"
            className="inline-block w-full px-6 py-3 bg-white hover:bg-gray-50 text-gray-700 font-semibold rounded-lg border border-gray-300 transition-colors duration-200"
          >
            Ir para o Dashboard
          </Link>
        </div>

        <div className="mt-8">
          <svg
            className="w-64 h-64 mx-auto text-gray-300"
            fill="currentColor"
            viewBox="0 0 24 24"
          >
            <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
          </svg>
        </div>
      </div>
    </div>
  );
};

export default NotFoundPage;
