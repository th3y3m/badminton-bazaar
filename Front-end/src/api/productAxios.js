import axios from "./customizeAxios";

const fetchPaginatedProducts = async (params) => {
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
    if (start !== undefined) queryParams.append('start', start);
    if (end !== undefined) queryParams.append('end', end);
    if (searchQuery) queryParams.append('searchQuery', searchQuery);
    if (sortBy) queryParams.append('sortBy', sortBy);
    if (status !== undefined) queryParams.append('status', status);
    if (supplierId) queryParams.append('supplierId', supplierId);
    if (categoryId) queryParams.append('categoryId', categoryId);
    if (pageIndex) queryParams.append('pageIndex', pageIndex);
    if (pageSize) queryParams.append('pageSize', pageSize);

    return axios.get(`Product?${queryParams.toString()}`);
};

const fetchProductById = async (id) => {
    return axios.get(`Product/${id}`);
};

const addProduct = async (productModel) => {
    const formData = new FormData();
    formData.append('productModel', JSON.stringify(productModel));
    if (productModel.productImageUrl) {
        formData.append('file', productModel.productImageUrl);
    }

    return axios.post('Product', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
};

const updateProduct = async (productModel, productId) => {
    return axios.put(`Product`, productModel, {
        params: { productId }
    });
};

const deleteProductById = async (id) => {
    return axios.delete(`Product/${id}`);
};
const getTopSeller = async (num) => {
    return axios.get(`Product/TopSeller/${num}`);
};

export {
    fetchPaginatedProducts,
    fetchProductById,
    addProduct,
    updateProduct,
    deleteProductById,
    getTopSeller
};