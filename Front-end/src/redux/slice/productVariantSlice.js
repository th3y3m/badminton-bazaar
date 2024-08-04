import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedProductVariants,
    fetchProductVariantById,
    addProductVariant,
    updateProductVariant,
    deleteProductVariantById
} from '../../api/productVariantAxios';

// Define async thunks
export const fetchAllProductVariants = createAsyncThunk(
    'productVariants/fetchPaginatedProductVariants',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedProductVariants(params);
        return response.items;
    }
);

export const fetchProductVariant = createAsyncThunk(
    'productVariants/fetchProductVariantById',
    async (id, thunkAPI) => {
        const response = await fetchProductVariantById(id);
        return response;
    }
);

export const createProductVariant = createAsyncThunk(
    'productVariants/addProductVariant',
    async (productVariantModel, thunkAPI) => {
        const response = await addProductVariant(productVariantModel);
        return response;
    }
);

export const modifyProductVariant = createAsyncThunk(
    'productVariants/updateProductVariant',
    async ({ productVariantModel, id }, thunkAPI) => {
        const response = await updateProductVariant(productVariantModel, id);
        return response;
    }
);

export const removeProductVariant = createAsyncThunk(
    'productVariants/deleteProductVariantById',
    async (id, thunkAPI) => {
        const response = await deleteProductVariantById(id);
        return response;
    }
);

// Initial state
const initialState = {
    productVariants: [],
    productVariantDetail: {},
    status: 'idle',
    error: null
};

// Create the slice
const productVariantSlice = createSlice({
    name: 'productVariants',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllProductVariants.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllProductVariants.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.productVariants = action.payload;
            })
            .addCase(fetchAllProductVariants.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchProductVariant.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchProductVariant.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.productVariantDetail = action.payload;
            })
            .addCase(fetchProductVariant.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createProductVariant.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createProductVariant.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.productVariants.push(action.payload);
            })
            .addCase(createProductVariant.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyProductVariant.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyProductVariant.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.productVariants.findIndex(variant => variant.id === action.meta.arg.id);
                if (index !== -1) {
                    state.productVariants[index] = action.payload;
                }
            })
            .addCase(modifyProductVariant.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeProductVariant.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeProductVariant.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.productVariants = state.productVariants.filter(variant => variant.id !== action.meta.arg);
            })
            .addCase(removeProductVariant.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default productVariantSlice.reducer;