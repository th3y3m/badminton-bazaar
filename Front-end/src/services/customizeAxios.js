import axios from "axios";

const instance = axios.create({
    baseURL: 'https://localhost:7173/api/',
});

instance.interceptors.response.use(function (response) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response.data ? response.data : {statuscode: response.status};
}, function (error) {
    console.log(">>>>>>>>> Check error: " + error.name);
    console.log(">>>>>>>>> Check error: " + error.response);
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error
    return Promise.reject(error);
});