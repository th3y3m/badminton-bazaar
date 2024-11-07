import axios from './customizeAxios'; // Import the configured axios instance

const fetchPaginatedSuppliers = async (params) => {
    try {
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

        return await axios.get(`Supplier?${queryParams.toString()}`);
    } catch (error) {
        console.error('Error fetching paginated suppliers:', error);
        throw error;
    }
};

const fetchSupplierById = async (id) => {
    try {
        return await axios.get(`Supplier/${id}`);
    } catch (error) {
        console.error(`Error fetching supplier by ID (${id}):`, error);
        throw error;
    }
};

const addSupplier = async (supplierModel) => {
    try {
        return await axios.post('Supplier', supplierModel);
    } catch (error) {
        console.error('Error adding supplier:', error);
        throw error;
    }
};

const updateSupplier = async (supplierModel, id) => {
    try {
        return await axios.put(`Supplier?id=${id}`, supplierModel);
    } catch (error) {
        console.error(`Error updating supplier by ID (${id}):`, error);
        throw error;
    }
};

const deleteSupplierById = async (id) => {
    try {
        return await axios.delete(`Supplier/${id}`);
    } catch (error) {
        console.error(`Error deleting supplier by ID (${id}):`, error);
        throw error;
    }
};

export {
    fetchPaginatedSuppliers,
    fetchSupplierById,
    addSupplier,
    updateSupplier,
    deleteSupplierById
};