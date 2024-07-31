import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedUsers,
    fetchUserById,
    deleteUser,
    banUser,
    unbanUser
} from '../../api/userAxios';

// Define async thunks
export const fetchAllUsers = createAsyncThunk(
    'users/fetchPaginatedUsers',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedUsers(params);
        return response.data;
    }
);

export const fetchUser = createAsyncThunk(
    'users/fetchUserById',
    async (id, thunkAPI) => {
        const response = await fetchUserById(id);
        return response.data;
    }
);

export const removeUser = createAsyncThunk(
    'users/deleteUser',
    async (id, thunkAPI) => {
        const response = await deleteUser(id);
        return response.data;
    }
);

export const banUserById = createAsyncThunk(
    'users/banUser',
    async (id, thunkAPI) => {
        const response = await banUser(id);
        return response.data;
    }
);

export const unbanUserById = createAsyncThunk(
    'users/unbanUser',
    async (id, thunkAPI) => {
        const response = await unbanUser(id);
        return response.data;
    }
);

// Initial state
const initialState = {
    users: [],
    userDetail: null,
    status: 'idle',
    error: null
};

// Create the slice
const userSlice = createSlice({
    name: 'users',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllUsers.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllUsers.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.users = action.payload;
            })
            .addCase(fetchAllUsers.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchUser.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchUser.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.userDetail = action.payload;
            })
            .addCase(fetchUser.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeUser.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeUser.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.users = state.users.filter(user => user.id !== action.meta.arg);
            })
            .addCase(removeUser.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(banUserById.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(banUserById.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.users.findIndex(user => user.id === action.meta.arg);
                if (index !== -1) {
                    state.users[index].status = 'banned';
                }
            })
            .addCase(banUserById.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(unbanUserById.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(unbanUserById.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.users.findIndex(user => user.id === action.meta.arg);
                if (index !== -1) {
                    state.users[index].status = 'active';
                }
            })
            .addCase(unbanUserById.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default userSlice.reducer;