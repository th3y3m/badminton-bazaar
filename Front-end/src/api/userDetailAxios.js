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
    const formData = new FormData();
    formData.append('FullName', UserDetailModel.FullName);
    formData.append('Address', UserDetailModel.Address);
    formData.append('ProfilePicture', UserDetailModel.ProfilePicture);
    formData.append('ImageUrl', UserDetailModel.ImageUrl);
    return axios.put(`UserDetail/${id}`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
};

export {
    fetchPaginatedUserDetails,
    fetchUserDetailById,
    updateUserDetail,
};
