import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedUserDetails = async (params) => {
    const {
        searchQuery = "",
        sortBy = "name_asc",
        pageIndex = 1,
        pageSize = 10
    } = params;

    const queryParams = new URLSearchParams();
    if (searchQuery) queryParams.append('searchQuery', searchQuery);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`UserDetail?${queryParams.toString()}`);
};

const fetchUserDetailById = async (id) => {
    return axios.get(`UserDetail/${id}`);
};

const updateUserDetail = async (UserDetailModel, id) => {
    return axios.put(`UserDetail?id=${id}`, UserDetailModel);
};

export {
    fetchPaginatedUserDetails,
    fetchUserDetailById,
    updateUserDetail,
};
