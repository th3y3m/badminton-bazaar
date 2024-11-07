import axios from './customizeAxios'; // Import the configured axios instance

// Fetch all freight prices
const fetchAllFreightPrices = async () => {
    try {
        return await axios.get('FreightPrice');
    } catch (error) {
        console.error('Error fetching all freight prices:', error);
        throw error;
    }
};

// Fetch a specific freight price by ID
const fetchFreightPriceById = async (id) => {
    try {
        return await axios.get(`FreightPrice/${id}`);
    } catch (error) {
        console.error(`Error fetching freight price by ID (${id}):`, error);
        throw error;
    }
};

// Add a new freight price
const addFreightPrice = async (freightPriceModel) => {
    try {
        return await axios.post('FreightPrice', freightPriceModel);
    } catch (error) {
        console.error('Error adding freight price:', error);
        throw error;
    }
};

// Update an existing freight price
const updateFreightPrice = async (freightPriceModel) => {
    try {
        return await axios.put('FreightPrice', freightPriceModel);
    } catch (error) {
        console.error('Error updating freight price:', error);
        throw error;
    }
};

// Delete a freight price by ID
const deleteFreightPrice = async (id) => {
    try {
        return await axios.delete(`FreightPrice?id=${id}`);
    } catch (error) {
        console.error(`Error deleting freight price by ID (${id}):`, error);
        throw error;
    }
};

// Get freight price based on distance
const fetchPriceByDistance = async (km) => {
    try {
        return await axios.get(`FreightPrice/GetPriceByDistance?km=${km}`);
    } catch (error) {
        console.error(`Error fetching price by distance (${km} km):`, error);
        throw error;
    }
};

export {
    fetchAllFreightPrices,
    fetchFreightPriceById,
    addFreightPrice,
    updateFreightPrice,
    deleteFreightPrice,
    fetchPriceByDistance
};