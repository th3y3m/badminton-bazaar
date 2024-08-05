import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedUserDetails,
    fetchUserDetailById,
    updateUserDetail
} from '../../api/userDetailAxios';

// Define async thunks
export const fetchAllUserDetails = createAsyncThunk(
    'userDetails/fetchPaginatedUserDetails',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedUserDetails(params);
        return response;
    }
);

export const fetchUserDetail = createAsyncThunk(
    'userDetails/fetchUserDetailById',
    async (id, thunkAPI) => {
        const response = await fetchUserDetailById(id);
        return response;
    }
);

export const modifyUserDetail = createAsyncThunk(
    'userDetails/updateUserDetail',
    async ({ UserDetailModel, id }, thunkAPI) => {
        const response = await updateUserDetail(UserDetailModel, id);
        return response;
    }
);

// Initial state
const initialState = {
    userDetails: [],
    userDetail: {},
    status: 'idle',
    error: null
};

// Create the slice
const userDetailSlice = createSlice({
    name: 'userDetails',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllUserDetails.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllUserDetails.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.userDetails = action.payload;
            })
            .addCase(fetchAllUserDetails.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchUserDetail.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchUserDetail.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.userDetail = action.payload;
            })
            .addCase(fetchUserDetail.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(modifyUserDetail.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(modifyUserDetail.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.userDetails.findIndex(userDetail => userDetail.id === action.meta.arg.id);
                if (index !== -1) {
                    state.userDetails[index] = action.payload;
                }
            })
            .addCase(modifyUserDetail.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default userDetailSlice.reducer;