import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { fetchOrderDetailByOrderId } from '../../api/orderDetailAxios';

// Define async thunk
export const fetchOrderDetail = createAsyncThunk(
    'orderDetails/fetchOrderDetailByOrderId',
    async (orderId, thunkAPI) => {
        const response = await fetchOrderDetailByOrderId(orderId);
        return response;
    }
);

// Initial state
const initialState = {
    orderDetail: null,
    status: 'idle',
    error: null
};

// Create the slice
const orderDetailSlice = createSlice({
    name: 'orderDetails',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchOrderDetail.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchOrderDetail.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orderDetail = action.payload;
            })
            .addCase(fetchOrderDetail.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default orderDetailSlice.reducer;