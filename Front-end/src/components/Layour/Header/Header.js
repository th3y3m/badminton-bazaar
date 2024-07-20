import React, { useContext, useEffect } from 'react';
import logo from './logo1.png';
import { AuthContext } from '../../../AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { numberOfItemsInCart } from '../../../api/cartAxios';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCartShopping } from '@fortawesome/free-solid-svg-icons';

const Header = () => {
  const { user, logout, cartCount, setCartCount } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
    toast.success('Logout successfully');
  };

  const handleCart = () => {
    navigate('/cart');
  };

  const fetchCart = async (userId) => {
    try {
      const data = await numberOfItemsInCart(userId);
      setCartCount(data);
    } catch (error) {
      console.error("Error fetching products:", error);
    }
  };

  useEffect(() => {
    if (user) {
      fetchCart(user.userId);
    }
  }, [user]);

  return (
    <header className="bg-white border-b-4 border-orange-500 relative">
      <div className="max-w-7xl mx-auto flex justify-between items-center py-2">
        
        <div className="text-3xl font-bold text-red-600">
          <img src={logo} alt="Logo" className="h-32 mr-2 inline cursor-pointer" onClick={() => navigate("/")}/> {/* Increase logo size here */}
        </div>
        <div className="flex items-center">
          <div className="relative">
            <button className="text-orange-600 text-2xl" onClick={handleCart}>
              <FontAwesomeIcon icon={faCartShopping} />
              <div className='absolute top-2 right-8'>
                <span className="bg-orange-600 text-white text-sm rounded-full h-5 w-5 flex items-center justify-center">{user ? cartCount : 0}</span>
              </div>
            </button>
          </div>
          <div className="relative ml-8">
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
      <nav className="bg-orange-500">
        <ul className="flex justify-center space-x-4 text-white font-semibold py-2 cursor-pointer" onClick={() => navigate("/products")}>
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
};

export default Header;
