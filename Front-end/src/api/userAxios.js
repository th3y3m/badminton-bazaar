import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedUsers = async (params) => {
    try {
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

        return await axios.get(`User?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated users:', error);
        throw error;
    }
};

const fetchUserById = async (id) => {
    try {
        return await axios.get(`User/${id}`);
    } catch (error) {
        console.error(`Error fetching user by ID (${id}):`, error);
        throw error;
    }
};

const deleteUser = async (id) => {
    try {
        return await axios.delete(`User/${id}`);
    } catch (error) {
        console.error(`Error deleting user by ID (${id}):`, error);
        throw error;
    }
};

const banUser = async (id) => {
    try {
        return await axios.put(`User/ban/${id}`);
    } catch (error) {
        console.error(`Error banning user by ID (${id}):`, error);
        throw error;
    }
};

const unbanUser = async (id) => {
    try {
        return await axios.get(`User/unban/${id}`);
    } catch (error) {
        console.error(`Error unbanning user by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedUsers,
    fetchUserById,
    deleteUser,
    banUser,
    unbanUser
};