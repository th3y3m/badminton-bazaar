import axios from './customizeAxios'; // Import the configured axios instance

const fetchOrderDetailByOrderId = async (orderId) => {
    try {
        return await axios.get(`OrderDetail/GetOrderDetailByOrderId/${orderId}`);
    } catch (error) {
        console.error(`Error fetching order detail by order ID (${orderId}):`, error);
        throw error;
    }
};

export { fetchOrderDetailByOrderId };