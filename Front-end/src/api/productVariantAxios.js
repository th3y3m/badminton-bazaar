import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedProductVariants = async (params) => {
    const {
        sortBy = "name_asc",
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

    return axios.get(`ProductVariant?${queryParams.toString()}`);
};

const fetchProductVariantById = async (id) => {
    return axios.get(`ProductVariant/${id}`);
};

const addProductVariant = async (productVariantModel) => {
    const formData = new FormData();
    formData.append('productVariantModel', JSON.stringify(productVariantModel));
    productVariantModel.productImageUrl.forEach(file => {
        formData.append('file', file);
    });

    return axios.post('ProductVariant', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
};

const updateProductVariant = async (productVariantModel, id) => {
    return axios.put(`ProductVariant/${id}`, productVariantModel);
};

const deleteProductVariantById = async (id) => {
    return axios.delete(`ProductVariant/${id}`);
};

export {
    fetchPaginatedProductVariants,
    fetchProductVariantById,
    addProductVariant,
    updateProductVariant,
    deleteProductVariantById
};
