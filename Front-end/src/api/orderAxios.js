import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedOrders = async (params) => {
    const {
        start,
        end,
        userId,
        sortBy = "orderdate_asc",
        status = null,
        pageIndex = 1,
        pageSize = 10
    } = params;

    const queryParams = new URLSearchParams();
    if (start) queryParams.append('start', start);
    if (end) queryParams.append('end', end);
    if (userId) queryParams.append('userId', userId);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (status) queryParams.append('status', status);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`Order/GetPaginatedOrders?${queryParams.toString()}`);
};

const fetchOrderById = async (orderId) => {
    return axios.get(`Order/${orderId}`);
};

const fetchTotalPrice = async (orderId) => {
    return axios.get(`Order/Price/${orderId}`);
};

// const createOrder = async (userId) => {
//     return axios.post('Order/CreateOrder', { userId });
// };
const createOrder = async (userId, freight, address) => {
    try {
        const requestBody = {
            userId: userId,
            freight: freight,
            address: address
        };

        return axios.post('Order/CreateOrder', requestBody); // Send the data in the request body
    } catch (error) {
        console.error(`Error creating order axios: ${error.message}`);
        throw error;
    }
};


const deleteOrderById = async (orderId) => {
    return axios.delete(`Order/DeleteOrder/${orderId}`);
};

export {
    fetchPaginatedOrders,
    fetchOrderById,
    fetchTotalPrice,
    createOrder,
    deleteOrderById
};
