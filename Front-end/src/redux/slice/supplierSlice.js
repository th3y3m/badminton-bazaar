import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedSuppliers,
    fetchSupplierById,
    addSupplier,
    updateSupplier,
    deleteSupplierById
} from '../../api/supplierAxios';

// Define async thunks
export const fetchAllSuppliers = createAsyncThunk(
    'suppliers/fetchPaginatedSuppliers',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedSuppliers(params);
        return response;
    }
);

export const fetchSupplier = createAsyncThunk(
    'suppliers/fetchSupplierById',
    async (id, thunkAPI) => {
        const response = await fetchSupplierById(id);
        return response;
    }
);

export const createSupplier = createAsyncThunk(
    'suppliers/addSupplier',
    async (supplierModel, thunkAPI) => {
        const response = await addSupplier(supplierModel);
        return response;
    }
);

export const modifySupplier = createAsyncThunk(
    'suppliers/updateSupplier',
    async ({ supplierModel, id }, thunkAPI) => {
        const response = await updateSupplier(supplierModel, id);
        return response;
    }
);

export const removeSupplier = createAsyncThunk(
    'suppliers/deleteSupplierById',
    async (id, thunkAPI) => {
        const response = await deleteSupplierById(id);
        return response;
    }
);

// Initial state
const initialState = {
    suppliers: [],
    supplierDetail: {},
    status: 'idle',
    error: null
};

// Create the slice
const supplierSlice = createSlice({
    name: 'suppliers',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllSuppliers.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllSuppliers.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.suppliers = action.payload;
            })
            .addCase(fetchAllSuppliers.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSupplier.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSupplier.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.supplierDetail = action.payload;
            })
            .addCase(fetchSupplier.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createSupplier.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createSupplier.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.suppliers.push(action.payload);
            })
            .addCase(createSupplier.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifySupplier.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifySupplier.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.suppliers.findIndex(supplier => supplier.id === action.payload.id);
                if (index !== -1) {
                    state.suppliers[index] = action.payload;
                }
            })
            .addCase(modifySupplier.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeSupplier.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeSupplier.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.suppliers = state.suppliers.filter(supplier => supplier.id !== action.meta.arg);
            })
            .addCase(removeSupplier.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default supplierSlice.reducer;