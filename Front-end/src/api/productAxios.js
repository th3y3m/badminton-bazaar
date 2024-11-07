import axios from "./customizeAxios";

const fetchPaginatedProducts = async (params) => {
    try {
        const {
            start,
            end,
            searchQuery = "",
            sortBy = "name_asc",
            status = true,
            supplierId = "",
            categoryId = "",
            pageIndex = 1,
            pageSize = 10
        } = params;

        const queryParams = new URLSearchParams();

        if (start !== undefined && start !== null) queryParams.append('start', start);
        if (end !== undefined && end !== null) queryParams.append('end', end);
        if (searchQuery) queryParams.append('searchQuery', searchQuery);
        if (sortBy) queryParams.append('sortBy', sortBy);
        if (status !== undefined) queryParams.append('status', status);
        if (supplierId) queryParams.append('supplierId', supplierId);
        if (categoryId) queryParams.append('categoryId', categoryId);
        if (pageIndex) queryParams.append('pageIndex', pageIndex);
        if (pageSize) queryParams.append('pageSize', pageSize);

        return await axios.get(`Product?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated products:', error);
        throw error;
    }
};

const fetchProductById = async (id) => {
    try {
        return await axios.get(`Product/${id}`);
    } catch (error) {
        console.error(`Error fetching product by ID (${id}):`, error);
        throw error;
    }
};

const fetchRelatedProduct = async (id) => {
    try {
        return await axios.get(`Product/GetRelatedProduct/${id}`);
    } catch (error) {
        console.error(`Error fetching related product by ID (${id}):`, error);
        throw error;
    }
};

const addProduct = async (productModel) => {
    try {
        const formData = new FormData();
        formData.append('productModel', JSON.stringify(productModel));
        if (productModel.productImageUrl) {
            formData.append('file', productModel.productImageUrl);
        }

        return await axios.post('Product', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    } catch (error) {
        console.error('Error adding product:', error);
        throw error;
    }
};

const updateProduct = async (productModel, productId) => {
    try {
        return await axios.put(`Product`, productModel, {
            params: { productId }
        });
    } catch (error) {
        console.error(`Error updating product by ID (${productId}):`, error);
        throw error;
    }
};

const deleteProductById = async (id) => {
    try {
        return await axios.delete(`Product/${id}`);
    } catch (error) {
        console.error(`Error deleting product by ID (${id}):`, error);
        throw error;
    }
};

const getTopSeller = async (num) => {
    try {
        return await axios.get(`Product/TopSeller/${num}`);
    } catch (error) {
        console.error(`Error fetching top seller products (num: ${num}):`, error);
        throw error;
    }
};

const numOfProductRemaining = async (id) => {
    try {
        return await axios.get(`Product/ProductRemaining/${id}`);
    } catch (error) {
        console.error(`Error fetching number of products remaining by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedProducts,
    fetchProductById,
    fetchRelatedProduct,
    addProduct,
    updateProduct,
    deleteProductById,
    getTopSeller,
    numOfProductRemaining
};