import { useContext, useState } from "react";
import { loginApi } from "../../api/authAxios";
import { ROUTERS } from "../../utils/Routers";
import { jwtDecode } from "jwt-decode";
import { toast } from "react-toastify";
import { useNavigate, Link } from "react-router-dom";
import { AuthContext } from "../../AuthContext";

const Login = () => {

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isShowPassword, setIsShowPassword] = useState(false);
    const navigate = useNavigate();
    const { login } = useContext(AuthContext);

    const handleLogin = async () => {
        if (!email || !password) {
            alert('Email and password are required');
            return;
        }

        let res = await loginApi({ email, password });
        if (res && res.token) {
            localStorage.setItem("token", res.token);
            console.log('token: ', res.token)
            var decode = jwtDecode(res.token);
            localStorage.setItem("userRole", decode.role);
            const userData = {
              email: decode.email,
              role: decode.role
            };
            login(userData); // Lưu thông tin người dùng vào context
            navigate(ROUTERS.USER.HOME); 
          } else if (res && res.status === 401) {
            //toast.error(res.error);
          } else if(res && res.data.status === "Error" && res.data.message == "User is banned!"){
            toast.error('This account is banned!', {
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
    }


    return (
        <div>
            <div className="bg-gray-200">
                <div className="flex justify-center items-center h-screen">
                    <div className="w-1/3">
                        <div className="bg-white shadow-md rounded px-8 py-8 pt-8">
                            <div className="px-4 pb-4">
                                <label htmlFor="email" className="text-sm block font-bold  pb-2">EMAIL ADDRESS</label>
                                <input onChange={(e) => setEmail(e.target.value)} type="email" name="email" id="" placeholder="Enter email" className="block w-full p-3 rounded bg-gray-200 border border-transparent focus:outline-none" />
                                <label htmlFor="pass" className="text-sm block font-bold  pb-2">PASSWORD</label>
                                <input onChange={(e) => setPassword(e.target.value)} type="password" name="pass" id="" placeholder="Enter password" className="block w-full p-3 rounded bg-gray-200 border border-transparent focus:outline-none" />
                            </div>
                            <button onClick={handleLogin} className="w-full bg-orange-500 hover:bg-orange-700 text-white font-bold py-2 rounded shadow-lg hover:shadow-xl transition duration-200" type="submit">LOGIN</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

}

export default Login;