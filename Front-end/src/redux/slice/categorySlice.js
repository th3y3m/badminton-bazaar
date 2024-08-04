import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedCategories,
    fetchCategoryById,
    addCategory,
    updateCategory,
    deleteCategoryById
} from '../../api/categoryAxios';

// Define async thunks
export const fetchCategories = createAsyncThunk(
    'categories/fetchPaginatedCategories',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedCategories(params);
        return response;
    }
);

export const fetchSingleCategory = createAsyncThunk(
    'categories/fetchCategoryById',
    async (id, thunkAPI) => {
        const response = await fetchCategoryById(id);
        return response;
    }
);

export const createCategory = createAsyncThunk(
    'categories/addCategory',
    async (categoryModel, thunkAPI) => {
        const response = await addCategory(categoryModel);
        return response;
    }
);

export const modifyCategory = createAsyncThunk(
    'categories/updateCategory',
    async ({ categoryModel, categoryId }, thunkAPI) => {
        const response = await updateCategory(categoryModel, categoryId);
        return response;
    }
);

export const removeCategory = createAsyncThunk(
    'categories/deleteCategoryById',
    async (id, thunkAPI) => {
        const response = await deleteCategoryById(id);
        return response;
    }
);

// Initial state
const initialState = {
    categories: [],
    singleCategory: {},
    status: 'idle',
    error: null
};

// Create the slice
const categorySlice = createSlice({
    name: 'categories',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchCategories.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchCategories.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.categories = action.payload;
            })
            .addCase(fetchCategories.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSingleCategory.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSingleCategory.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.singleCategory = action.payload;
            })
            .addCase(fetchSingleCategory.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createCategory.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createCategory.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.categories.push(action.payload);
            })
            .addCase(createCategory.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyCategory.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyCategory.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.categories.findIndex(category => category.id === action.payload.id);
                if (index !== -1) {
                    state.categories[index] = action.payload;
                }
            })
            .addCase(modifyCategory.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeCategory.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeCategory.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.categories = state.categories.filter(category => category.id !== action.payload.id);
            })
            .addCase(removeCategory.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default categorySlice.reducer;