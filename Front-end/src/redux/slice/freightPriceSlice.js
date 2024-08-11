import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchAllFreightPrices,
    fetchFreightPriceById,
    addFreightPrice,
    updateFreightPrice,
    deleteFreightPrice,
    fetchPriceByDistance
} from '../../api/freightPriceAxios';

// Define async thunks
export const fetchFreightPrices = createAsyncThunk(
    'freightPrices/fetchAllFreightPrices',
    async (_, thunkAPI) => {
        const response = await fetchAllFreightPrices();
        return response;
    }
);

export const fetchSingleFreightPrice = createAsyncThunk(
    'freightPrices/fetchFreightPriceById',
    async (id, thunkAPI) => {
        const response = await fetchFreightPriceById(id);
        return response;
    }
);

export const createFreightPrice = createAsyncThunk(
    'freightPrices/addFreightPrice',
    async (freightPriceModel, thunkAPI) => {
        const response = await addFreightPrice(freightPriceModel);
        return response;
    }
);

export const updateFreightPriceDetails = createAsyncThunk(
    'freightPrices/updateFreightPrice',
    async (freightPriceModel, thunkAPI) => {
        const response = await updateFreightPrice(freightPriceModel);
        return response;
    }
);

export const deleteFreightPriceById = createAsyncThunk(
    'freightPrices/deleteFreightPrice',
    async (id, thunkAPI) => {
        const response = await deleteFreightPrice(id);
        return response;
    }
);

export const fetchFreightPriceByDistance = createAsyncThunk(
    'freightPrices/fetchPriceByDistance',
    async (km, thunkAPI) => {
        const response = await fetchPriceByDistance(km);
        return response;
    }
);

// Initial state
const initialState = {
    freightPrices: [],
    priceByDistance: 0,
    status: 'idle',
    error: null,
};

// Create the slice
const freightPriceSlice = createSlice({
    name: 'freightPrices',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            // Fetch all freight prices
            .addCase(fetchFreightPrices.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchFreightPrices.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.freightPrices = action.payload;
            })
            .addCase(fetchFreightPrices.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })

            // Fetch single freight price by ID
            // .addCase(fetchSingleFreightPrice.pending, (state) => {
            //     state.status = 'loading';
            // })
            // .addCase(fetchSingleFreightPrice.fulfilled, (state, action) => {
            //     state.status = 'succeeded';
            //     state.singleFreightPrice = action.payload;
            // })
            // .addCase(fetchSingleFreightPrice.rejected, (state, action) => {
            //     state.status = 'failed';
            //     state.error = action.error.message;
            // })

            // Create a new freight price
            .addCase(createFreightPrice.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createFreightPrice.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.freightPrices.push(action.payload);
            })
            .addCase(createFreightPrice.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })

            // Update freight price
            .addCase(updateFreightPriceDetails.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(updateFreightPriceDetails.fulfilled, (state, action) => {
                state.status = 'succeeded';
                // Update the specific freight price in the state
                const index = state.freightPrices.findIndex(f => f.id === action.payload.id);
                if (index !== -1) {
                    state.freightPrices[index] = action.payload;
                }
            })
            .addCase(updateFreightPriceDetails.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })

            // Delete freight price by ID
            .addCase(deleteFreightPriceById.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(deleteFreightPriceById.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.freightPrices = state.freightPrices.filter(f => f.id !== action.meta.arg);
            })
            .addCase(deleteFreightPriceById.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })

            // Fetch freight price by distance
            .addCase(fetchFreightPriceByDistance.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchFreightPriceByDistance.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.priceByDistance = action.payload;
            })
            .addCase(fetchFreightPriceByDistance.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default freightPriceSlice.reducer;
