import React from 'react';
import logo from './logo.jpeg';

const Header = () => {
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
                    <span className="text-blue-600 mr-6">Hotline: </span>
                    <div className="relative">
                        <button className="text-orange-600 text-2xl">
                            <i className="fas fa-shopping-cart"></i>
                        </button>
                        <span className="absolute top-0 right-0 bg-orange-600 text-white text-sm rounded-full h-5 w-5 flex items-center justify-center">0</span>
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