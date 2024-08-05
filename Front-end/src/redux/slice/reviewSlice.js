import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedReviews,
    fetchReviewById,
    addReview,
    updateReview,
    deleteReviewById,
    getAverageRating
} from '../../api/reviewAxios';

// Define async thunks
export const fetchAllReviews = createAsyncThunk(
    'reviews/fetchPaginatedReviews',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedReviews(params);
        return response;
    }
);

export const fetchReview = createAsyncThunk(
    'reviews/fetchReviewById',
    async (id, thunkAPI) => {
        const response = await fetchReviewById(id);
        return response;
    }
);

export const createReview = createAsyncThunk(
    'reviews/addReview',
    async (reviewModel, thunkAPI) => {
        const response = await addReview(reviewModel);
        return response;
    }
);

export const modifyReview = createAsyncThunk(
    'reviews/updateReview',
    async ({ reviewModel, id }, thunkAPI) => {
        const response = await updateReview(reviewModel, id);
        return response;
    }
);

export const removeReview = createAsyncThunk(
    'reviews/deleteReviewById',
    async (id, thunkAPI) => {
        const response = await deleteReviewById(id);
        return response;
    }
);
export const fetchAverageRating = createAsyncThunk(
    'reviews/fetchAverageRating',
    async (id, thunkAPI) => {
        const response = await getAverageRating(id);
        return response;
    }
);

// Initial state
const initialState = {
    reviews: [],
    reviewDetail: {},
    averageRating: 0,
    status: 'idle',
    error: null
};

// Create the slice
const reviewSlice = createSlice({
    name: 'reviews',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllReviews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllReviews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.reviews = action.payload;
            })
            .addCase(fetchAllReviews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchReview.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchReview.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.reviewDetail = action.payload;
            })
            .addCase(fetchReview.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createReview.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createReview.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.reviews.items.push(action.payload);
            })
            .addCase(createReview.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchAverageRating.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAverageRating.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.averageRating = action.payload;
            })
            .addCase(fetchAverageRating.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyReview.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyReview.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.reviews.findIndex(review => review.id === action.meta.arg.id);
                if (index !== -1) {
                    state.reviews[index] = action.payload;
                }
            })
            .addCase(modifyReview.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeReview.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeReview.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.reviews = state.reviews.filter(review => review.id !== action.meta.arg);
            })
            .addCase(removeReview.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default reviewSlice.reducer;