import React, { useState } from "react";
import { Menu, X } from "lucide-react";
import { Link } from "react-router-dom";

const Navbar: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);

  const navLinks = [
    { name: "Dashboard", href: "/dashboard" },
    { name: "Transactions", href: "/transactions" },
    { name: "Budgets", href: "/budgets" },
    { name: "Settings", href: "/settings" },
  ];

  return (
    <nav className="fixed top-0 left-0 w-full z-50 backdrop-blur-md bg-white/30 border-b border-white/20">
      <div className="max-w-7xl mx-auto px-6 py-4 flex justify-between items-center">
        {/* Hamburger - Mobile only */}
        <button
          onClick={() => setIsOpen(true)}
          className="md:hidden text-gray-800 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400 rounded"
          aria-label="Open menu"
        >
          <Menu size={28} />
        </button>

        {/* Logo - Centered on desktop */}
        <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 font-bold text-xl tracking-widest uppercase text-blue-700 select-none pointer-events-none md:pointer-events-auto md:relative md:left-auto md:top-auto md:translate-x-0 md:translate-y-0">
          <Link to="/">Personal Spending</Link>
        </div>

        {/* Desktop nav */}
        <div className="hidden md:flex space-x-10 font-semibold text-gray-700">
          {navLinks.map((link) => (
            <Link
              key={link.name}
              to={link.href}
              className="relative px-3 py-2 rounded-lg transition
                hover:text-white hover:bg-gradient-to-r hover:from-blue-500 hover:via-purple-600 hover:to-pink-500"
            >
              {link.name}
            </Link>
          ))}
        </div>
      </div>

      {/* Mobile Menu Overlay */}
      {isOpen && (
        <>
          <div
            className="fixed inset-0 bg-black/50 backdrop-blur-sm"
            onClick={() => setIsOpen(false)}
          />
          <aside className="fixed top-0 left-0 w-64 h-full bg-white shadow-lg p-6 flex flex-col space-y-6 z-60 animate-slide-in">
            <button
              onClick={() => setIsOpen(false)}
              className="self-end text-gray-700 hover:text-blue-600 focus:outline-none"
              aria-label="Close menu"
            >
              <X size={28} />
            </button>
            {navLinks.map((link) => (
              <Link
                key={link.name}
                to={link.href}
                onClick={() => setIsOpen(false)}
                className="text-lg font-semibold text-gray-800 hover:text-blue-600"
              >
                {link.name}
              </Link>
            ))}
          </aside>
        </>
      )}

      <style>{`
        @keyframes slide-in {
          from { transform: translateX(-100%); }
          to { transform: translateX(0); }
        }
        .animate-slide-in {
          animation: slide-in 0.3s ease forwards;
        }
      `}</style>
    </nav>
  );
};

export default Navbar;
