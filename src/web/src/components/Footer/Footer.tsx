import React from 'react';

function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300 py-12 mt-auto">
      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8 mb-8">
          {/* Brand */}
          <div className="col-span-1 md:col-span-2">
            <div className="flex items-center space-x-2 mb-4">
              <div className="w-10 h-10 bg-gradient-to-br from-primary-500 to-primary-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">C</span>
              </div>
              <span className="text-xl font-bold text-white">Caju Ajuda</span>
            </div>
            <p className="text-gray-400 max-w-md">
              Sistema inteligente de gestão de chamados com priorização por IA. 
              Simplifique seu suporte técnico e aumente a produtividade da sua equipe.
            </p>
          </div>

          {/* Links Rápidos */}
          <div>
            <h3 className="text-white font-semibold mb-4">Links Rápidos</h3>
            <ul className="space-y-2">
              <li>
                <a href="/login" className="text-gray-400 hover:text-primary-400 transition-colors">Entrar</a>
              </li>
              <li>
                <a href="/register" className="text-gray-400 hover:text-primary-400 transition-colors">Criar Conta</a>
              </li>
            </ul>
          </div>

          {/* Contato */}
          <div>
            <h3 className="text-white font-semibold mb-4">Suporte</h3>
            <ul className="space-y-2">
              <li>
                <a href="mailto:suporte@cajuajuda.com" className="text-gray-400 hover:text-primary-400 transition-colors">
                  suporte@cajuajuda.com
                </a>
              </li>
              <li className="text-gray-400">
                Segunda a Sexta, 9h-18h
              </li>
            </ul>
          </div>
        </div>

        {/* Copyright */}
        <div className="border-t border-gray-800 pt-8 text-center">
          <p className="text-gray-400 text-sm">
            &copy; {new Date().getFullYear()} Caju Ajuda. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </footer>
  );
}

export default Footer;