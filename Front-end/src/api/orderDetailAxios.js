import axios from './customizeAxios'; // Import the configured axios instance

const fetchOrderDetailByOrderId = async (orderId) => {
    return axios.get(`OrderDetail/GetOrderDetailByOrderId/${orderId}`);
};

export { fetchOrderDetailByOrderId };