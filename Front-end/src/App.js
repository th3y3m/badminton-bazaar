import { useContext } from 'react';
import Footer from './components/Layour/Footer/Footer';
import Header from './components/Layour/Header/Header';
import ProductPage from './components/Product/ProductsPage';
import './index.scss';
import { AuthContext } from './AuthContext';
import Login from './components/Login/Login';
import { Route, Router } from 'react-router-dom';
import { Switch } from '@mui/material';
import Register from './components/Register/Register';
import { useSelector } from 'react-redux';


function App() {

  const user = useSelector((state) => state.auth.account);

  console.log('user: ', user);

  return (
    <div className="App">
      <Header />
      {/* <ProductPage /> */}
      <Login />
      <Footer />
    </div>
  );
}

export default App;
