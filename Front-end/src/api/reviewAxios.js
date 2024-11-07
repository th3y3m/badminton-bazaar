import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedReviews = async (params) => {
    try {
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

        return await axios.get(`Review?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated reviews:', error);
        throw error;
    }
};

const fetchReviewById = async (id) => {
    try {
        return await axios.get(`Review/${id}`);
    } catch (error) {
        console.error(`Error fetching review by ID (${id}):`, error);
        throw error;
    }
};

const addReview = async (reviewModel) => {
    try {
        return await axios.post('Review', reviewModel);
    } catch (error) {
        console.error('Error adding review:', error);
        throw error;
    }
};

const updateReview = async (reviewModel, id) => {
    try {
        return await axios.put(`Review?id=${id}`, reviewModel);
    } catch (error) {
        console.error(`Error updating review by ID (${id}):`, error);
        throw error;
    }
};

const deleteReviewById = async (id) => {
    try {
        return await axios.delete(`Review/${id}`);
    } catch (error) {
        console.error(`Error deleting review by ID (${id}):`, error);
        throw error;
    }
};

const getAverageRating = async (id) => {
    try {
        return await axios.get(`Review/GetAverageRating/${id}`);
    } catch (error) {
        console.error(`Error fetching average rating for product ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedReviews,
    fetchReviewById,
    getAverageRating,
    addReview,
    updateReview,
    deleteReviewById
};