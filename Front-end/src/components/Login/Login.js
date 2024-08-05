import { useEffect, useState } from "react";
import { ROUTERS } from "../../utils/Routers";
import { toast } from "react-toastify";
import { useDispatch, useSelector } from 'react-redux';
import { fetchLoginApi } from "../../redux/slice/authSlice";
import { useNavigate } from "react-router-dom";

const Login = () => {

    const dispatch = useDispatch();
    const navigate = useNavigate();

    const userLogin = useSelector((state) => state.auth.token);

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async () => {
        if (!email || !password) {
            console.log('Email and password are required');
            toast.error('Email and password are required', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "colored",
            });
            return;
        }
        dispatch(fetchLoginApi({ email, password }));
    }

    useEffect(() => {
        if (userLogin && userLogin.token) {
            navigate(ROUTERS.USER.HOME);
        }
    }, [userLogin]);
    return (
        <div className="min-h-screen bg-orange-50 flex items-center justify-center">
            <div className="max-w-md w-full bg-white shadow-md rounded-lg p-8">
                <h2 className="text-2xl font-bold text-center mb-6 text-orange-600">Login</h2>
                <div className="mb-4">
                    <label htmlFor="email" className="block text-sm font-bold text-gray-700 mb-2">Email Address</label>
                    <input onChange={(e) => setEmail(e.target.value)} type="email" name="email" id="email" placeholder="Enter email" className="block w-full p-3 rounded border border-gray-300 focus:ring focus:ring-orange-200 focus:outline-none" />
                </div>
                <div className="mb-6">
                    <label htmlFor="pass" className="block text-sm font-bold text-gray-700 mb-2">Password</label>
                    <input onChange={(e) => setPassword(e.target.value)} type="password" name="pass" id="pass" placeholder="Enter password" className="block w-full p-3 rounded border border-gray-300 focus:ring focus:ring-orange-200 focus:outline-none" />
                </div>
                <button onClick={handleLogin} className="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 rounded-lg shadow-md hover:shadow-lg transition duration-200" type="submit">Login</button>
            </div>
        </div>
    );

}

export default Login;