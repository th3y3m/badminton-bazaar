import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedUsers = async (params) => {
    const {
        searchQuery = "",
        sortBy = "name_asc",
        status = true,
        pageIndex = 1,
        pageSize = 10
    } = params;

    const queryParams = new URLSearchParams();
    if (searchQuery) queryParams.append('searchQuery', searchQuery);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (status) queryParams.append('status', status);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`User?${queryParams.toString()}`);
};

const fetchUserById = async (id) => {
    return axios.get(`User/${id}`);
};
const deleteUser = async (id) => {
    return axios.delete(`User/${id}`);
};
const banUser = async (id) => {
    return axios.put(`User/ban/${id}`);
};
const unbanUser = async (id) => {
    return axios.get(`User/unban/${id}`);
};

export {
    fetchPaginatedUsers,
    fetchUserById,
    deleteUser,
    banUser,
    unbanUser
};
