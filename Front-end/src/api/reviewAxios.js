import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedReviews = async (params) => {
    const {
        userId = "",
        productId = "",
        rating = null,
        searchQuery = "",
        sortBy = "date_desc",
        pageIndex = 1,
        pageSize = 10
    } = params;

    const queryParams = new URLSearchParams();
    if (userId) queryParams.append('userId', userId);
    if (productId) queryParams.append('productId', productId);
    if (rating !== null) queryParams.append('rating', rating);
    if (searchQuery) queryParams.append('searchQuery', searchQuery);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`Review?${queryParams.toString()}`);
};

const fetchReviewById = async (id) => {
    return axios.get(`Review/${id}`);
};

const addReview = async (reviewModel) => {
    return axios.post('Review', reviewModel);
};

const updateReview = async (reviewModel, id) => {
    return axios.put(`Review?id=${id}`, reviewModel);
};

const deleteReviewById = async (id) => {
    return axios.delete(`Review/${id}`);
};

export {
    fetchPaginatedReviews,
    fetchReviewById,
    addReview,
    updateReview,
    deleteReviewById
};
