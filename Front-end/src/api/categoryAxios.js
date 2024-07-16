import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

const fetchPaginatedCategories = async (params) => {
    const {
        searchQuery = "",
        sortBy = "categoryname_asc",
        status = true,
        pageIndex = 1,
        pageSize = 10
    } = params;

    const queryParams = new URLSearchParams();
    if (searchQuery) queryParams.append('searchQuery', searchQuery);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (status !== undefined) queryParams.append('status', status);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`Category?${queryParams.toString()}`);
};

const fetchCategoryById = async (id) => {
    return axios.get(`Category/${id}`);
};

const addCategory = async (categoryModel) => {
    return axios.post('Category', categoryModel);
};

const updateCategory = async (categoryModel, categoryId) => {
    return axios.put(`Category`, categoryModel, {
        params: { categoryId }
    });
};

const deleteCategoryById = async (id) => {
    return axios.delete(`Category/${id}`);
};

export {
    fetchPaginatedCategories,
    fetchCategoryById,
    addCategory,
    updateCategory,
    deleteCategoryById
};
