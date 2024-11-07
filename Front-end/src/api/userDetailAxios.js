import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedUserDetails = async (params) => {
    try {
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

        return await axios.get(`UserDetail?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated user details:', error);
        throw error;
    }
};

const fetchUserDetailById = async (id) => {
    try {
        return await axios.get(`UserDetail/${id}`);
    } catch (error) {
        console.error(`Error fetching user detail by ID (${id}):`, error);
        throw error;
    }
};

const updateUserDetail = async (UserDetailModel, id) => {
    try {
        const formData = new FormData();
        formData.append('FullName', UserDetailModel.FullName);
        formData.append('Address', UserDetailModel.Address);
        formData.append('ProfilePicture', UserDetailModel.ProfilePicture);
        formData.append('ImageUrl', UserDetailModel.ImageUrl);
        return await axios.put(`UserDetail/${id}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    } catch (error) {
        console.error(`Error updating user detail by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedUserDetails,
    fetchUserDetailById,
    updateUserDetail,
};