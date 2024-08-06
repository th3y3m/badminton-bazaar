import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    language: "vn",
    status: 'idle',
    error: null,
}

export const fetchLanguage = createAsyncThunk(
    'language/fetchLanguage',
    async (lang, thunkAPI) => {
        try {
            return lang;
        } catch (error) {
            toast.error('Change language failed');
            return thunkAPI.rejectWithValue(error);
        }
    }
);

export const langSlice = createSlice({
    name: 'language',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchLanguage.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchLanguage.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.language = action.payload;
            })
            .addCase(fetchLanguage.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
    },
})

export default langSlice.reducer