import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

const fetchPaginatedCategories = async (params) => {
    try {
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

        return await axios.get(`Category?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated categories:', error);
        throw error;
    }
};

const fetchCategoryById = async (id) => {
    try {
        return await axios.get(`Category/${id}`);
    } catch (error) {
        console.error('Error fetching category by ID:', error);
        throw error;
    }
};

const addCategory = async (categoryModel) => {
    try {
        return await axios.post('Category', categoryModel);
    } catch (error) {
        console.error('Error adding category:', error);
        throw error;
    }
};

const updateCategory = async (categoryModel, categoryId) => {
    try {
        return await axios.put(`Category`, categoryModel, {
            params: { categoryId }
        });
    } catch (error) {
        console.error('Error updating category:', error);
        throw error;
    }
};

const deleteCategoryById = async (id) => {
    try {
        return await axios.delete(`Category/${id}`);
    } catch (error) {
        console.error('Error deleting category by ID:', error);
        throw error;
    }
};

export {
    fetchPaginatedCategories,
    fetchCategoryById,
    addCategory,
    updateCategory,
    deleteCategoryById
};
