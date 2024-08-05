import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    fetchPaginatedOrders,
    fetchOrderById,
    fetchTotalPrice,
    createOrder,
    deleteOrderById
} from '../../api/orderAxios';

// Define async thunks
export const fetchOrders = createAsyncThunk(
    'orders/fetchPaginatedOrders',
    async (params, thunkAPI) => {
        const response = await fetchPaginatedOrders(params);
        return response;
    }
);

export const fetchSingleOrder = createAsyncThunk(
    'orders/fetchOrderById',
    async (orderId, thunkAPI) => {
        const response = await fetchOrderById(orderId);
        return response;
    }
);

export const fetchOrderPrice = createAsyncThunk(
    'orders/fetchTotalPrice',
    async (orderId, thunkAPI) => {
        const response = await fetchTotalPrice(orderId);
        return response;
    }
);

export const createNewOrder = createAsyncThunk(
    'orders/createOrder',
    async (userId, thunkAPI) => {
        const response = await createOrder(userId);
        return response;
    }
);

export const deleteOrder = createAsyncThunk(
    'orders/deleteOrderById',
    async (orderId, thunkAPI) => {
        const response = await deleteOrderById(orderId);
        return response;
    }
);

// Initial state
const initialState = {
    orders: [],
    singleOrder: null,
    orderPrice: null,
    status: 'idle',
    error: null
};

// Create the slice
const orderSlice = createSlice({
    name: 'orders',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchOrders.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchOrders.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orders = action.payload;
            })
            .addCase(fetchOrders.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchSingleOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchSingleOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.singleOrder = action.payload;
            })
            .addCase(fetchSingleOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchOrderPrice.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchOrderPrice.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orderPrice = action.payload;
            })
            .addCase(fetchOrderPrice.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createNewOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createNewOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orders.push(action.payload);
            })
            .addCase(createNewOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(deleteOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(deleteOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orders = state.orders.filter(order => order.id !== action.meta.arg);
            })
            .addCase(deleteOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default orderSlice.reducer;