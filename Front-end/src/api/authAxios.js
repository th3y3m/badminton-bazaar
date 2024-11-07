import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

const loginApi = async (login) => {
    try {
        const response = await axios.post('auth/login', login);
        return response;
    } catch (error) {
        console.error('Login failed', error);
        throw error;
    }
};

const registerApi = async (register) => {
    try {
        const response = await axios.post('auth/register', register);
        return response;
    } catch (error) {
        console.error('Registration failed', error);
        throw error;
    }
};

const refreshToken = async () => {
    try {
        const response = await axios.post('auth/refresh-token');
        return response;
    } catch (error) {
        console.error('Token refresh failed', error);
        throw error;
    }
};

const signinWithGoogle = async () => {
    try {
        const response = await axios.post('auth/signin-google');
        return response;
    } catch (error) {
        console.error('Google sign-in failed', error);
        throw error;
    }
};

const signinWithFacebook = async () => {
    try {
        const response = await axios.post('auth/signin-facebook');
        return response;
    } catch (error) {
        console.error('Facebook sign-in failed', error);
        throw error;
    }
};

const changePassword = async (changePasswordRequest) => {
    try {
        const response = await axios.post('auth/change-password', changePasswordRequest);
        return response;
    } catch (error) {
        console.error('Change password failed', error);
        throw error;
    }
};

const forgetPassword = async (forgetPasswordModel) => {
    try {
        const response = await axios.post('auth/forget-password', forgetPasswordModel);
        return response;
    } catch (error) {
        console.error('Forget password failed', error);
        throw error;
    }
};

const resetPassword = async (resetPasswordModel) => {
    try {
        const response = await axios.post('auth/reset-password', resetPasswordModel);
        return response;
    } catch (error) {
        console.error('Reset password failed', error);
        throw error;
    }
};

const linkExternalLogin = async (linkExternalLogin) => {
    try {
        const response = await axios.post('auth/link-external-login', linkExternalLogin);
        return response;
    } catch (error) {
        console.error('Link external login failed', error);
        throw error;
    }
};

const unlinkExternalLogin = async (unlinkExternalLogin) => {
    try {
        const response = await axios.post('auth/unlink-external-login', unlinkExternalLogin);
        return response;
    } catch (error) {
        console.error('Unlink external login failed', error);
        throw error;
    }
};

const createPasswordForExternalLogin = async (createPasswordForExternalLogin) => {
    try {
        const response = await axios.post('auth/create-password-for-external-login', createPasswordForExternalLogin);
        return response;
    } catch (error) {
        console.error('Create password for external login failed', error);
        throw error;
    }
};

export {
    loginApi,
    registerApi,
    refreshToken,
    signinWithGoogle,
    signinWithFacebook,
    changePassword,
    forgetPassword,
    resetPassword,
    linkExternalLogin,
    unlinkExternalLogin,
    createPasswordForExternalLogin
};