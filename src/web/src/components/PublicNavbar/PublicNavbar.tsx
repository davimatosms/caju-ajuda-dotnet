import React, { useState, useRef } from 'react';
import { Link } from 'react-router-dom';

function PublicNavbar() {
  const [open, setOpen] = useState(false);
  const hamburgerRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);
  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const useFocusTrap = require('../../hooks/useFocusTrap').default;
  useFocusTrap(menuRef, open, () => setOpen(false));

  return (
    <nav className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center space-x-2" aria-label="Caju Ajuda">
            <div className="w-10 h-10 bg-gradient-to-br from-primary-500 to-primary-600 rounded-lg flex items-center justify-center">
              <span className="text-white font-bold text-xl">C</span>
            </div>
            <span className="text-xl font-bold text-gray-900">Caju Ajuda</span>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center space-x-4">
            <Link 
              to="/login" 
              className="text-gray-700 hover:text-primary-600 font-medium transition-colors"
            >
              Entrar
            </Link>
            <Link 
              to="/register" 
              className="btn-primary px-6 py-2"
            >
              Criar Conta
            </Link>
          </div>

          {/* Mobile Hamburger */}
          <button
            ref={hamburgerRef}
            className="md:hidden p-2 rounded-lg hover:bg-gray-100 transition-colors"
            aria-label="Abrir menu"
            aria-expanded={open}
            aria-controls="public-mobile-menu"
            onClick={() => setOpen(true)}
          >
            <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
        </div>
      </div>

      {/* Mobile Menu */}
      {open && (
        <>
          {/* Backdrop */}
          <div 
            className="fixed inset-0 bg-black bg-opacity-50 z-40 md:hidden"
            onClick={() => setOpen(false)}
          />
          
          {/* Menu Panel */}
          <div
            ref={menuRef}
            id="public-mobile-menu"
            className="fixed top-0 right-0 bottom-0 w-64 bg-white shadow-xl z-50 md:hidden animate-slide-in"
            role="dialog"
            aria-modal="true"
            aria-label="Menu Público"
          >
            <div className="p-4">
              <div className="flex justify-between items-center mb-8">
                <span className="text-lg font-bold text-gray-900">Menu</span>
                <button
                  className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
                  aria-label="Fechar menu"
                  onClick={() => setOpen(false)}
                >
                  <svg className="w-6 h-6 text-gray-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
              
              <div className="flex flex-col space-y-3">
                <Link
                  to="/login"
                  onClick={() => setOpen(false)}
                  className="px-4 py-3 text-gray-700 hover:bg-gray-100 rounded-lg font-medium transition-colors"
                >
                  Entrar
                </Link>
                <Link
                  to="/register"
                  onClick={() => setOpen(false)}
                  className="btn-primary px-4 py-3 text-center"
                >
                  Criar Conta
                </Link>
              </div>
            </div>
          </div>
        </>
      )}
    </nav>
  );
}

export default PublicNavbar;