import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedColors,
    fetchColorById,
    addColor,
    fetchColorsOfProduct
} from '../../api/colorAxios';

// Define async thunks
export const fetchColors = createAsyncThunk(
    'colors/fetchPaginatedColors',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedColors(params);
        return response;
    }
);

export const fetchSingleColor = createAsyncThunk(
    'colors/fetchColorById',
    async (id, thunkAPI) => {
        const response = await fetchColorById(id);
        return response;
    }
);

export const createColor = createAsyncThunk(
    'colors/addColor',
    async (colorModel, thunkAPI) => {
        const response = await addColor(colorModel);
        return response;
    }
);

export const fetchColorsByProduct = createAsyncThunk(
    'colors/fetchColorsOfProduct',
    async (productId, thunkAPI) => {
        const response = await fetchColorsOfProduct(productId);
        return response;
    }
);

// Initial state
const initialState = {
    colors: [],
    singleColor: {},
    status: 'idle',
    error: null
};

// Create the slice
const colorSlice = createSlice({
    name: 'colors',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchColors.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchColors.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.colors = action.payload;
            })
            .addCase(fetchColors.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSingleColor.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSingleColor.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.singleColor = action.payload;
            })
            .addCase(fetchSingleColor.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createColor.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createColor.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.colors.push(action.payload);
            })
            .addCase(createColor.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchColorsByProduct.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchColorsByProduct.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.colors = action.payload;
            })
            .addCase(fetchColorsByProduct.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default colorSlice.reducer;