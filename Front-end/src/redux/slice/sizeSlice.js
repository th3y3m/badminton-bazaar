import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedSizes,
    fetchSizeById,
    addSize,
    fetchSizesOfProduct
} from '../../api/sizeAxios';

// Define async thunks
export const fetchAllSizes = createAsyncThunk(
    'sizes/fetchPaginatedSizes',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedSizes(params);
        return response;
    }
);

export const fetchSizesForProduct = createAsyncThunk(
    'sizes/fetchSizesOfProduct',
    async (productId, thunkAPI) => {
        const response = await fetchSizesOfProduct(productId);
        return response;
    }
);

export const fetchSize = createAsyncThunk(
    'sizes/fetchSizeById',
    async (id, thunkAPI) => {
        const response = await fetchSizeById(id);
        return response;
    }
);

export const createSize = createAsyncThunk(
    'sizes/addSize',
    async (SizeModel, thunkAPI) => {
        const response = await addSize(SizeModel);
        return response.data;
    }
);

// Initial state
const initialState = {
    sizes: [],
    sizeDetail: {},
    status: 'idle',
    error: null
};

// Create the slice
const sizeSlice = createSlice({
    name: 'sizes',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllSizes.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllSizes.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.sizes = action.payload;
            })
            .addCase(fetchAllSizes.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSizesForProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSizesForProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.sizes = action.payload;
            })
            .addCase(fetchSizesForProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSize.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSize.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.sizeDetail = action.payload;
            })
            .addCase(fetchSize.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createSize.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createSize.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.sizes.push(action.payload);
            })
            .addCase(createSize.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default sizeSlice.reducer;