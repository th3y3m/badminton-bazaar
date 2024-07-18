import React, { useContext } from 'react';
import logo from './logo.jpeg';
import { AuthContext } from '../../../AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const Header = () => {

    const { user, logout } = useContext(AuthContext);

    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
        toast.success('Logout successfully');
    }

    return (
        <header className="bg-white border-b-4 border-orange-500 relative">
            <div className="max-w-7xl mx-auto flex justify-between items-center py-2">
                <div className="flex items-center">
                    <input
                        type="text"
                        placeholder="Tìm kiếm ..."
                        className="ml-4 p-2 border border-gray-300 rounded-lg"
                    />
                </div>
                <div className="text-3xl font-bold text-red-600">
                    <img src={logo} alt="Logo" className="h-10 mr-2 inline" />
                </div>
                <div className="flex items-center">
                    <div className="relative">
                        <button className="text-orange-600 text-2xl">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 10.5V6a3.75 3.75 0 1 0-7.5 0v4.5m11.356-1.993 1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 0 1-1.12-1.243l1.264-12A1.125 1.125 0 0 1 5.513 7.5h12.974c.576 0 1.059.435 1.119 1.007ZM8.625 10.5a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm7.5 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z" />
                            </svg>
                        </button>
                        <span className="absolute top-0 right-0 bg-orange-600 text-white text-sm rounded-full h-5 w-5 flex items-center justify-center">0</span>
                        <div>
                            {user ? (
                                <div className="flex items-center">
                                    <span className="text-sm">Hello, {user.name}</span>
                                    <button onClick={handleLogout} className="text-red-500 ml-2">Logout</button>
                                </div>
                            ) : (
                                <div className="flex items-center">
                                    <button onClick={() => navigate('/login')} className="text-sm">Login</button>
                                    <button onClick={() => navigate('/register')} className="text-sm ml-2">Register</button>
                                </div>
                            )}
                        </div>
                    </div>

                </div>
            </div>
            <nav className="bg-orange-500">
                <ul className="flex justify-center space-x-4 text-white font-semibold py-2">
                    <li><a href="#" className="hover:underline">PRODUCTS</a></li>
                    <li><a href="#" className="hover:underline">RACKETS</a></li>
                    <li><a href="#" className="hover:underline">MEN</a></li>
                    <li><a href="#" className="hover:underline">WOMEN</a></li>
                    <li><a href="#" className="hover:underline">ABOUT US</a></li>
                    <li><a href="#" className="hover:underline">NEWS</a></li>
                </ul>
            </nav>
        </header>
    );
}

export default Header;