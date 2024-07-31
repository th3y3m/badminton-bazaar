import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedNews,
    fetchNewsById,
    addNews,
    updateNews,
    deleteNewsById
} from '../../api/newsAxios';

// Define async thunks
export const fetchNews = createAsyncThunk(
    'news/fetchPaginatedNews',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedNews(params);
        return response.items;
    }
);
export const fetchSlideNews = createAsyncThunk(
    'news/fetchSlideNews',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedNews(params);
        return response.items;
    }
);
export const fetchBannerNews = createAsyncThunk(
    'news/fetchBannerNews',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedNews(params);
        return response.items;
    }
);

export const fetchSingleNews = createAsyncThunk(
    'news/fetchNewsById',
    async (id, thunkAPI) => {
        const response = await fetchNewsById(id);
        return response.data;
    }
);

export const createNews = createAsyncThunk(
    'news/addNews',
    async (newsModel, thunkAPI) => {
        const response = await addNews(newsModel);
        return response.data;
    }
);

export const modifyNews = createAsyncThunk(
    'news/updateNews',
    async ({ newsModel, id }, thunkAPI) => {
        const response = await updateNews(newsModel, id);
        return response.data;
    }
);

export const removeNews = createAsyncThunk(
    'news/deleteNewsById',
    async (id, thunkAPI) => {
        const response = await deleteNewsById(id);
        return response.data;
    }
);

// Initial state
const initialState = {
    news: [],
    newsSlide: [],
    banners: [],
    singleNews: null,
    status: 'idle',
    error: null
};

// Create the slice
const newsSlice = createSlice({
    name: 'news',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.news = action.payload;
            })
            .addCase(fetchNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSlideNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSlideNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.newsSlide = action.payload;
            })
            .addCase(fetchSlideNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchBannerNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchBannerNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.banners = action.payload;
            })
            .addCase(fetchBannerNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSingleNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSingleNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.singleNews = action.payload;
            })
            .addCase(fetchSingleNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.news.push(action.payload);
            })
            .addCase(createNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.news.findIndex(news => news.id === action.payload.id);
                if (index !== -1) {
                    state.news[index] = action.payload;
                }
            })
            .addCase(modifyNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeNews.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeNews.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.news = state.news.filter(news => news.id !== action.payload.id);
            })
            .addCase(removeNews.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default newsSlice.reducer;