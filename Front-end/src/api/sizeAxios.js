import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedSizes = async (params) => {
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

    return axios.get(`Size?${queryParams.toString()}`);
};

const fetchSizesOfProduct = async (productId) => {
    return axios.get(`Size/GetSizesOfProduct/${productId}`);
}

const fetchSizeById = async (id) => {
    return axios.get(`Size/${id}`);
};

const addSize = async (SizeModel) => {
    return axios.post('Size', { SizeModel });
};

export {
    fetchPaginatedSizes,
    fetchSizeById,
    addSize,
    fetchSizesOfProduct
};
