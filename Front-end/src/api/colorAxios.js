import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedColors = async (params) => {
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

    return axios.get(`Color?${queryParams.toString()}`);
};

const fetchColorById = async (id) => {
    return axios.get(`Color/${id}`);
};

const addColor = async (colorModel) => {
    return axios.post('Color', { colorModel });
};

export {
    fetchPaginatedColors,
    fetchColorById,
    addColor
};
