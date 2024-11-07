import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedColors = async (params) => {
    try {
        const {
            searchQuery = "",
            sortBy = "color_asc",
            pageIndex = 1,
            pageSize = 10
        } = params;

        const queryParams = new URLSearchParams();
        if (searchQuery) queryParams.append('searchQuery', searchQuery);
        if (sortBy) queryParams.append('sortBy', sortBy);
        if (pageIndex) queryParams.append('pageIndex', pageIndex);
        if (pageSize) queryParams.append('pageSize', pageSize);

        return await axios.get(`Color?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated colors:', error);
        throw error;
    }
};

const fetchColorById = async (id) => {
    try {
        return await axios.get(`Color/${id}`);
    } catch (error) {
        console.error(`Error fetching color by ID (${id}):`, error);
        throw error;
    }
};

const addColor = async (colorModel) => {
    try {
        return await axios.post('Color', { colorModel });
    } catch (error) {
        console.error('Error adding color:', error);
        throw error;
    }
};

const fetchColorsOfProduct = async (productId) => {
    try {
        return await axios.get(`Color/GetColorsOfProduct/${productId}`);
    } catch (error) {
        console.error(`Error fetching colors of product (${productId}):`, error);
        throw error;
    }
}

export {
    fetchPaginatedColors,
    fetchColorById,
    addColor,
    fetchColorsOfProduct
};