import axios from "axios";

const instance = axios.create({
    baseURL: 'https://localhost:7173/api/',
});

instance.interceptors.response.use(
    function (response) {
        // Ensure that response.data is always returned, and if not, return an empty object or null
        return response.data;
    },
    function (error) {
        let res = {};
        if (error.response) {
            res.data = error.response.data;
            res.status = error.response.status;
            res.headers = error.response.headers;
        } else if (error.request) {
            console.log(error.request);
        } else {
            console.log('Error', error.message);
        }
        return Promise.reject(res); // Reject the promise with the error response
    }
);

export default instance;
