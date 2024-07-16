import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedSuppliers = async (params) => {
    const {
        searchQuery = "",
        sortBy = "companyname_asc",
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

    return axios.get(`Supplier?${queryParams.toString()}`);
};

const fetchSupplierById = async (id) => {
    return axios.get(`Supplier/${id}`);
};

const addSupplier = async (supplierModel) => {
    return axios.post('Supplier', supplierModel);
};

const updateSupplier = async (supplierModel, id) => {
    return axios.put(`Supplier?id=${id}`, supplierModel);
};

const deleteSupplierById = async (id) => {
    return axios.delete(`Supplier/${id}`);
};

export {
    fetchPaginatedSuppliers,
    fetchSupplierById,
    addSupplier,
    updateSupplier,
    deleteSupplierById
};
