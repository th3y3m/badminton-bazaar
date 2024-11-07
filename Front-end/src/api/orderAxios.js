import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedOrders = async (params) => {
    try {
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

        return await axios.get(`Order/GetPaginatedOrders?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated orders:', error);
        throw error;
    }
};

const fetchOrderById = async (orderId) => {
    try {
        return await axios.get(`Order/${orderId}`);
    } catch (error) {
        console.error(`Error fetching order by ID (${orderId}):`, error);
        throw error;
    }
};

const fetchTotalPrice = async (orderId) => {
    try {
        return await axios.get(`Order/Price/${orderId}`);
    } catch (error) {
        console.error(`Error fetching total price for order ID (${orderId}):`, error);
        throw error;
    }
};

const createOrder = async (userId, freight, address) => {
    try {
        const requestBody = {
            userId: userId,
            freight: freight,
            address: address
        };

        return await axios.post('Order/CreateOrder', requestBody);
    } catch (error) {
        console.error(`Error creating order: ${error.message}`);
        throw error;
    }
};

const deleteOrderById = async (orderId) => {
    try {
        return await axios.delete(`Order/DeleteOrder/${orderId}`);
    } catch (error) {
        console.error(`Error deleting order by ID (${orderId}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedOrders,
    fetchOrderById,
    fetchTotalPrice,
    createOrder,
    deleteOrderById
};