import axios from './customizeAxios'; // Import the configured axios instance

const fetchOrderDetailByOrderId = async (orderId) => {
    return axios.get(`OrderDetai/GetOrderDetailByOrderId/${orderId}`);
};