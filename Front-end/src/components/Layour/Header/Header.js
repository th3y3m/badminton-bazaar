import React, { useState, useEffect } from 'react';
import logo from '../../../assets/logo1.png';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCartShopping } from '@fortawesome/free-solid-svg-icons';
import { useDispatch, useSelector } from 'react-redux';
import { fetchLogoutApi } from '../../../redux/slice/authSlice';
import { fetchNumberOfItems } from '../../../redux/slice/cartSlice';
import { fetchUserDetail } from '../../../redux/slice/userDetailSlice';
import { Avatar, Menu, MenuItem } from '@mui/material';
import { fetchUser } from '../../../redux/slice/userSlice';

const Header = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const user = useSelector((state) => state.auth.token);

  const numberOfItemsInCart = useSelector((state) => state.cart.itemsCount);
  const userDetails = useSelector((state) => state.userDetails.userDetail);

  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  const handleLogout = () => {
    dispatch(fetchLogoutApi()).then(() => {
      navigate('/');
    });
    handleClose();
  };

  const handleCart = () => {
    navigate('/cart');
  };

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  useEffect(() => {
    if (user && user.id) {
      dispatch(fetchNumberOfItems(user.id));
      dispatch(fetchUserDetail(user.id));
      dispatch(fetchUser(user.id));
    }
  }, [user, dispatch]);

  return (
    <header className="bg-white border-b-4 border-orange-500 relative">
      <div className="max-w-7xl mx-auto flex justify-between items-center py-2">
        <div className="text-3xl font-bold text-red-600">
          <img src={logo} alt="Logo" className="h-32 mr-2 inline cursor-pointer" onClick={() => navigate("/")} />
        </div>
        <div className="flex items-center">
          <div className="relative">
            <button className="text-orange-600 text-2xl" onClick={handleCart}>
              <FontAwesomeIcon icon={faCartShopping} />
              <div className='absolute top-2 right-8'>
                <span className="bg-orange-600 text-white text-sm rounded-full h-5 w-5 flex items-center justify-center">{numberOfItemsInCart}</span>
              </div>
            </button>
          </div>
          <div className="relative ml-8">
            {(user && user.token) ? (
              <div className="flex items-center">
                <Avatar src={userDetails.profilePicture} onClick={handleClick} style={{ cursor: 'pointer' }} />
                <Menu
                  anchorEl={anchorEl}
                  open={open}
                  onClose={handleClose}
                  anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'right',
                  }}
                  transformOrigin={{
                    vertical: 'top',
                    horizontal: 'right',
                  }}
                >
                  <MenuItem onClick={() => { handleClose(); navigate('/profile'); }}>View Profile</MenuItem>
                  <MenuItem onClick={() => { handleClose(); navigate('/orders'); }}>Order History</MenuItem>
                  <MenuItem onClick={handleLogout}>Log out</MenuItem>
                </Menu>
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
        <ul className="flex justify-center space-x-4 text-white font-semibold py-2">
          <li className='cursor-pointer' onClick={() => navigate("/products")}>PRODUCTS</li>
          <li className='cursor-pointer' onClick={() => navigate("/products")}>RACKETS</li>
          <li className='cursor-pointer' onClick={() => navigate("/products")}>MEN</li>
          <li className='cursor-pointer' onClick={() => navigate("/products")}>WOMEN</li>
          <li className='cursor-pointer' onClick={() => navigate("/products")}>ABOUT US</li>
          <li className='cursor-pointer' onClick={() => navigate("/news")}>NEWS</li>
        </ul>
      </nav>
    </header>
  );
};

export default Header;
