import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.scss';
import App from './App';
import reportWebVitals from './reportWebVitals';
import Home from './components/Home/Home';
import Header from './components/Layour/Header/Header';
import Footer from './components/Layour/Footer/Footer';
import ProductPage from './components/Product/ProductsPage';
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { AuthProvider } from "./AuthContext";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { BrowserRouter } from "react-router-dom";
import RouterCustom from './router';
import store from './redux/store';
import { Provider } from 'react-redux'
import { useMemo } from 'react';




const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <Provider store={store}>
    <React.StrictMode>
      <ToastContainer
        position="top-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />

      <BrowserRouter>
        <GoogleOAuthProvider clienId="333628503460-h7f0nupbv8c3u8e548tj6f9ruioj8jso.apps.googleusercontent.com">
          <AuthProvider>
            <RouterCustom />
          </AuthProvider>
        </GoogleOAuthProvider>

      </BrowserRouter>


    </React.StrictMode>
  </Provider>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
