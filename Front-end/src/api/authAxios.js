import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

const loginApi = async (login) => {
    return axios.post('Authentication/login', login);
};

const registerApi = async (register) => {
    return axios.post('Authentication/register', register);
};

export {
    loginApi,
    registerApi
};