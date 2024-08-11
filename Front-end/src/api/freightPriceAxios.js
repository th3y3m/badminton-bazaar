import axios from './customizeAxios'; // Import the configured axios instance

// Fetch all freight prices
const fetchAllFreightPrices = async () => {
    return axios.get('FreightPrice');
};

// Fetch a specific freight price by ID
const fetchFreightPriceById = async (id) => {
    return axios.get(`FreightPrice/${id}`);
};

// Add a new freight price
const addFreightPrice = async (freightPriceModel) => {
    return axios.post('FreightPrice', freightPriceModel);
};

// Update an existing freight price
const updateFreightPrice = async (freightPriceModel) => {
    return axios.put('FreightPrice', freightPriceModel);
};

// Delete a freight price by ID
const deleteFreightPrice = async (id) => {
    return axios.delete(`FreightPrice?id=${id}`);
};

// Get freight price based on distance
const fetchPriceByDistance = async (km) => {
    return axios.get(`FreightPrice/GetPriceByDistance?km=${km}`);
};

export {
    fetchAllFreightPrices,
    fetchFreightPriceById,
    addFreightPrice,
    updateFreightPrice,
    deleteFreightPrice,
    fetchPriceByDistance
};
