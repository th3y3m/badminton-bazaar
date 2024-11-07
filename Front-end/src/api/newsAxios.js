import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedNews = async (params) => {
    try {
        const {
            status,
            isHomePageBanner = false,
            isHomePageSlideShow = false,
            searchQuery = "",
            sortBy = "publicationdate_asc",
            pageIndex = 1,
            pageSize = 10
        } = params;

        const queryParams = new URLSearchParams();
        if (status !== undefined) queryParams.append('status', status);
        if (isHomePageBanner !== undefined) queryParams.append('isHomePageBanner', isHomePageBanner);
        if (isHomePageSlideShow !== undefined) queryParams.append('isHomePageSlideShow', isHomePageSlideShow);
        if (searchQuery) queryParams.append('searchQuery', searchQuery);
        if (sortBy) queryParams.append('sortBy', sortBy);
        if (pageIndex) queryParams.append('pageIndex', pageIndex);
        if (pageSize) queryParams.append('pageSize', pageSize);

        return await axios.get(`News?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated news:', error);
        throw error;
    }
};

const fetchNewsById = async (id) => {
    try {
        return await axios.get(`News/${id}`);
    } catch (error) {
        console.error(`Error fetching news by ID (${id}):`, error);
        throw error;
    }
};

const viewNews = async (id) => {
    try {
        return await axios.put(`News/AddAViewUnit/${id}`);
    } catch (error) {
        console.error(`Error viewing news by ID (${id}):`, error);
        throw error;
    }
};

const addNews = async (newsModel) => {
    try {
        return await axios.post('News', newsModel);
    } catch (error) {
        console.error('Error adding news:', error);
        throw error;
    }
};

const updateNews = async (newsModel, id) => {
    try {
        return await axios.put(`News?id=${id}`, newsModel);
    } catch (error) {
        console.error(`Error updating news by ID (${id}):`, error);
        throw error;
    }
};

const deleteNewsById = async (id) => {
    try {
        return await axios.delete(`News/${id}`);
    } catch (error) {
        console.error(`Error deleting news by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedNews,
    fetchNewsById,
    viewNews,
    addNews,
    updateNews,
    deleteNewsById
};