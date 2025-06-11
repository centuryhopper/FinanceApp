import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import Budgets from './components/Budgets';
import Dashboard from './components/Dashboard';
import Home from './components/Home';
import Navbar from './components/Navbar';
import Settings from './components/Settings';
import Transactions from './components/Transactions';
import Footer from './components/Footer';

function App() {
  return (
    <div className="app-wrapper bg-dark text-white">
      <BrowserRouter>
        <Navbar />
        <div className="app-content container-fluid p-5">
          <Routes>
            <Route path="/budgets" element={<Budgets />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/transactions" element={<Transactions />} />
            <Route path="/settings" element={<Settings />} />
            <Route path="/" element={<Home />} />
          </Routes>
        </div>
        <Footer />
      </BrowserRouter>
    </div>
  );
}

export default App;
