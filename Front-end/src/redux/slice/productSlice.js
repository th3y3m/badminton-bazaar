import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedProducts,
    fetchProductById,
    addProduct,
    updateProduct,
    deleteProductById,
    getTopSeller,
    numOfProductRemaining
} from '../../api/productAxios';

// Define async thunks
export const fetchProducts = createAsyncThunk(
    'products/fetchPaginatedProducts',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedProducts(params);
        return response.data;
    }
);

export const fetchProduct = createAsyncThunk(
    'products/fetchProductById',
    async (id, thunkAPI) => {
        const response = await fetchProductById(id);
        return response.data;
    }
);

export const createProduct = createAsyncThunk(
    'products/addProduct',
    async (productModel, thunkAPI) => {
        const response = await addProduct(productModel);
        return response.data;
    }
);

export const modifyProduct = createAsyncThunk(
    'products/updateProduct',
    async ({ productModel, productId }, thunkAPI) => {
        const response = await updateProduct(productModel, productId);
        return response.data;
    }
);

export const removeProduct = createAsyncThunk(
    'products/deleteProductById',
    async (id, thunkAPI) => {
        const response = await deleteProductById(id);
        return response.data;
    }
);

export const fetchTopSeller = createAsyncThunk(
    'products/getTopSeller',
    async (num, thunkAPI) => {
        const response = await getTopSeller(num);
        return response.data;
    }
);

export const fetchProductRemaining = createAsyncThunk(
    'products/numOfProductRemaining',
    async (id, thunkAPI) => {
        const response = await numOfProductRemaining(id);
        return response.data;
    }
);

// Initial state
const initialState = {
    products: [],
    product: null,
    topSellers: [],
    productRemaining: null,
    status: 'idle',
    error: null
};

// Create the slice
const productSlice = createSlice({
    name: 'products',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchProducts.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchProducts.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.products = action.payload;
            })
            .addCase(fetchProducts.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.product = action.payload;
            })
            .addCase(fetchProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.products.push(action.payload);
            })
            .addCase(createProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.products.findIndex(product => product.id === action.payload.id);
                if (index !== -1) {
                    state.products[index] = action.payload;
                }
            })
            .addCase(modifyProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.products = state.products.filter(product => product.id !== action.payload.id);
            })
            .addCase(removeProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchTopSeller.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchTopSeller.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.topSellers = action.payload;
            })
            .addCase(fetchTopSeller.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchProductRemaining.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchProductRemaining.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.productRemaining = action.payload;
            })
            .addCase(fetchProductRemaining.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default productSlice.reducer;