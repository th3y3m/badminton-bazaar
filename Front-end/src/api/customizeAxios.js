import axios from "axios";
import axiosRetry from "axios-retry";
import { refreshToken } from "./authAxios";

// Create axios instance
const instance = axios.create({
    baseURL: 'https://localhost:8080/api/',
    withCredentials: true
});

// Add request interceptor to include the token in the Authorization header
instance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('authToken'); // Retrieve token from localStorage
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Set up axios retry for network/server errors
axiosRetry(instance, {
    retries: 3,
    retryDelay: axiosRetry.exponentialDelay,
    shouldRetry: (error) => {
        // Retry on network errors or 5xx server errors, excluding 401 Unauthorized
        return error.response && error.response.status >= 500 && error.response.status !== 401;
    },
});

// Interceptor to handle 401 errors and refresh token
instance.interceptors.response.use(
    (response) => response.data,
    async (error) => {
        const originalRequest = error.config;

        // If error is 401 Unauthorized, attempt to refresh token
        if (error.response && error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true; // Prevent infinite loop of retries

            try {
                const response = await refreshToken(); // Call the refresh token endpoint

                // Assuming response contains the new access token
                const newToken = response.authToken;
                localStorage.setItem('authToken', newToken);

                // Update the Authorization header and retry the original request
                originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
                return instance(originalRequest); // Retry the request with the new token
            } catch (refreshError) {
                // If refreshing fails, handle logout or show error
                console.error("Token refresh failed:", refreshError);
                return Promise.reject(refreshError);
            }
        }

        // If not 401 or refresh failed, reject the promise
        return Promise.reject(error);
    }
);

export default instance;
