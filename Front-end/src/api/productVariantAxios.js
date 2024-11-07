import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedProductVariants = async (params) => {
    try {
        const {
            sortBy = "price_asc",
            status = true,
            colorId = "",
            sizeId = "",
            productId = "",
            pageIndex = 1,
            pageSize = 10
        } = params;

        const queryParams = new URLSearchParams();
        if (sortBy) queryParams.append('sortBy', sortBy);
        if (status !== undefined) queryParams.append('status', status);
        if (colorId) queryParams.append('colorId', colorId);
        if (sizeId) queryParams.append('sizeId', sizeId);
        if (productId) queryParams.append('productId', productId);
        if (pageIndex) queryParams.append('pageIndex', pageIndex);
        if (pageSize) queryParams.append('pageSize', pageSize);

        return await axios.get(`ProductVariant?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated product variants:', error);
        throw error;
    }
};

const fetchProductVariantById = async (id) => {
    try {
        return await axios.get(`ProductVariant/${id}`);
    } catch (error) {
        console.error(`Error fetching product variant by ID (${id}):`, error);
        throw error;
    }
};

const addProductVariant = async (productVariantModel) => {
    try {
        const formData = new FormData();
        formData.append('productVariantModel', JSON.stringify(productVariantModel));
        productVariantModel.productImageUrl.forEach(file => {
            formData.append('file', file);
        });

        return await axios.post('ProductVariant', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    } catch (error) {
        console.error('Error adding product variant:', error);
        throw error;
    }
};

const updateProductVariant = async (productVariantModel, id) => {
    try {
        return await axios.put(`ProductVariant/${id}`, productVariantModel);
    } catch (error) {
        console.error(`Error updating product variant by ID (${id}):`, error);
        throw error;
    }
};

const deleteProductVariantById = async (id) => {
    try {
        return await axios.delete(`ProductVariant/${id}`);
    } catch (error) {
        console.error(`Error deleting product variant by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedProductVariants,
    fetchProductVariantById,
    addProductVariant,
    updateProductVariant,
    deleteProductVariantById
};