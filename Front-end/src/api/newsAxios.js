import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedNews = async (params) => {
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

    return axios.get(`News?${queryParams.toString()}`);
};

const fetchNewsById = async (id) => {
    return axios.get(`News/${id}`);
};
const viewNews = async (id) => {
    return axios.put(`News/AddAViewUnit/${id}`);
};

const addNews = async (newsModel) => {
    return axios.post('News', newsModel);
};

const updateNews = async (newsModel, id) => {
    return axios.put(`News?id=${id}`, newsModel);
};

const deleteNewsById = async (id) => {
    return axios.delete(`News/${id}`);
};

export {
    fetchPaginatedNews,
    fetchNewsById,
    viewNews,
    addNews,
    updateNews,
    deleteNewsById
};
