import { useState } from "react";
import { registerApi } from "../../api/authAxios";

const Register = () => {
    const [email, setEmail] = useState('');
    const [name, setName] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleRegister = async () => {
        if (!email || !name || !password || !confirmPassword) {
            alert('All fields are required');
            return;
        }

        if (password !== confirmPassword) {
            alert('Passwords do not match');
            return;
        }

        let res = await registerApi({ email, name, password, confirmPassword });
        
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
                                <label htmlFor="name" className="text-sm block font-bold  pb-2">FULL NAME</label>
                                <input onChange={(e) => setName(e.target.value)} type="text" name="name" id="" placeholder="Enter full name" className="block w-full p-3 rounded bg-gray-200 border border-transparent focus:outline-none" />
                                <label htmlFor="pass" className="text-sm block font-bold  pb-2">PASSWORD</label>
                                <input onChange={(e) => setPassword(e.target.value)} type="password" name="pass" id="" placeholder="Enter password" className="block w-full p-3 rounded bg-gray-200 border border-transparent focus:outline-none" />
                                <label htmlFor="passConfirm" className="text-sm block font-bold  pb-2">CONFIRM PASSWORD</label>
                                <input onChange={(e) => setConfirmPassword(e.target.value)} type="password" name="passConfirm" id="" placeholder="Enter confirm password" className="block w-full p-3 rounded bg-gray-200 border border-transparent focus:outline-none" />
                            </div>
                            <button className="w-full bg-orange-500 hover:bg-orange-700 text-white font-bold py-2 rounded shadow-lg hover:shadow-xl transition duration-200" type="submit">Register</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

}

export default Register;