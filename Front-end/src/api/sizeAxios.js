import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedSizes = async (params) => {
    try {
        const {
            searchQuery = "",
            sortBy = "size_asc",
            pageIndex = 1,
            pageSize = 10
        } = params;

        const queryParams = new URLSearchParams();
        if (searchQuery) queryParams.append('searchQuery', searchQuery);
        if (sortBy) queryParams.append('sortBy', sortBy);
        if (pageIndex) queryParams.append('pageIndex', pageIndex);
        if (pageSize) queryParams.append('pageSize', pageSize);

        return await axios.get(`Size?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated sizes:', error);
        throw error;
    }
};

const fetchSizesOfProduct = async (productId) => {
    try {
        return await axios.get(`Size/GetSizesOfProduct/${productId}`);
    } catch (error) {
        console.error(`Error fetching sizes of product (${productId}):`, error);
        throw error;
    }
}

const fetchSizeById = async (id) => {
    try {
        return await axios.get(`Size/${id}`);
    } catch (error) {
        console.error(`Error fetching size by ID (${id}):`, error);
        throw error;
    }
};

const addSize = async (SizeModel) => {
    try {
        return await axios.post('Size', { SizeModel });
    } catch (error) {
        console.error('Error adding size:', error);
        throw error;
    }
};

export {
    fetchPaginatedSizes,
    fetchSizeById,
    addSize,
    fetchSizesOfProduct
};