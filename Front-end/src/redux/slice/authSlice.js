import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { loginApi } from '../../api/authAxios';
import { toast } from "react-toastify";

// Async thunk for login API
export const fetchLoginApi = createAsyncThunk(
    'Authentication/fetchLoginApi',
    async (params, thunkAPI) => {
        try {
            let res = await loginApi(params);

            console.log('res: ', res)
            if (res && res.token) {
                toast.success('Login successfully');
                return res;
            }
        } catch (error) {
            console.log(error);
            if (error.status === 400) {
                console.log('Invalid Email format');
                toast.error('Invalid Email format', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "colored",
                });
            } else if (error.status === 401) {
                toast.error('Email or password is incorrect');
            } else if (error.data && error.data.status === "Error" && error.data.message === "User is banned!") {
                toast.error('User is banned!', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "colored",
                });
            } else {
                toast.error('Login failed', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "colored",
                });
            }

            return thunkAPI.rejectWithValue(error);
        }
    }
);

export const fetchLogoutApi = createAsyncThunk(
    'Authentication/fetchLogoutApi',
    async (_, thunkAPI) => {
        try {
            toast.success('Logout successfully');
            return;
        } catch (error) {
            console.log(error);
            toast.error('Logout failed');
            return thunkAPI.rejectWithValue(error);
        }
    }
);

// Initial state for auth slice
const initialState = {
    token: JSON.parse(localStorage.getItem('authToken')) || {},
    status: "idle",
    error: null,
};

const authSlice = createSlice({
    name: 'Authentication',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchLoginApi.pending, (state) => {
                state.status = "loading";
                state.error = null;
            })
            .addCase(fetchLoginApi.fulfilled, (state, action) => {
                state.status = "succeeded";
                state.token = action.payload;
                // Save the token to localStorage
                localStorage.setItem('authToken', JSON.stringify(action.payload));
            })
            .addCase(fetchLoginApi.rejected, (state, action) => {
                state.status = "failed";
                state.error = action.error.message;
            })
            .addCase(fetchLogoutApi.pending, (state) => {
                state.status = "loading";
                state.error = null;
            })
            .addCase(fetchLogoutApi.fulfilled, (state) => {
                state.status = "succeeded";
                state.token = {};
                state.error = null;
                // Clear the token from localStorage
                localStorage.removeItem('authToken');
            })
            .addCase(fetchLogoutApi.rejected, (state, action) => {
                state.status = "failed";
                state.error = action.error.message;
            });
    },
});

export default authSlice.reducer;
