import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    deletePayment,
    generatePaymentToken,
    processPayment,
    // processBalancePayment,
    fetchPaymentById,
    fetchPaginatedPayments,
    fetchPaymentByOrderId
} from '../../api/paymentAxios';

// Define async thunks
export const fetchAllPayments = createAsyncThunk(
    'payments/fetchPayments',
    async ({ pageNumber, pageSize }, thunkAPI) => {
        const response = await fetchPaginatedPayments(pageNumber, pageSize);
        return response;
    }
);

export const removePayment = createAsyncThunk(
    'payments/deletePayment',
    async (paymentId, thunkAPI) => {
        const response = await deletePayment(paymentId);
        return response;
    }
);

export const createPaymentToken = createAsyncThunk(
    'payments/generatePaymentToken',
    async (bookingId, thunkAPI) => {
        const response = await generatePaymentToken(bookingId);
        return response;
    }
);

export const executePayment = createAsyncThunk(
    'payments/processPayment',
    async (token, thunkAPI) => {
        const response = await processPayment("Customer", token);
        return response;
    }
);

// export const executeBalancePayment = createAsyncThunk(
//     'payments/processBalancePayment',
//     async (token, thunkAPI) => {
//         const response = await processBalancePayment(token);
//         return response;
//     }
// );

export const fetchPaymentDetails = createAsyncThunk(
    'payments/fetchPaymentById',
    async (paymentId, thunkAPI) => {
        const response = await fetchPaymentById(paymentId);
        return response;
    }
);

export const fetchPaymentDetailsByOrder = createAsyncThunk(
    'payments/fetchPaymentDetailsByOrder',
    async (paymentId, thunkAPI) => {
        const response = await fetchPaymentByOrderId(paymentId);
        return response;
    }
);

// Initial state
const initialState = {
    payments: [],
    paymentDetail: {},
    paymentToken: null,
    paymentResult: null,
    status: 'idle',
    error: null
};

// Create the slice
const paymentSlice = createSlice({
    name: 'payments',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAllPayments.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchAllPayments.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.payments = action.payload.items;
                state.totalCount = action.payload.totalCount;
            })
            .addCase(fetchAllPayments.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removePayment.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removePayment.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.payments = state.payments.filter(payment => payment.id !== action.meta.arg);
            })
            .addCase(removePayment.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(createPaymentToken.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createPaymentToken.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.paymentToken = action.payload;
            })
            .addCase(createPaymentToken.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(executePayment.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(executePayment.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.paymentResult = action.payload;
            })
            .addCase(executePayment.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            // .addCase(executeBalancePayment.pending, (state) => {
            //     state.status = 'loading';
            // })
            // .addCase(executeBalancePayment.fulfilled, (state, action) => {
            //     state.status = 'succeeded';
            //     state.balancePaymentResult = action.payload;
            // })
            // .addCase(executeBalancePayment.rejected, (state, action) => {
            //     state.status = 'failed';
            //     state.error = action.error.message;
            // })
            .addCase(fetchPaymentDetails.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchPaymentDetails.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.paymentDetail = action.payload;
            })
            .addCase(fetchPaymentDetails.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchPaymentDetailsByOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchPaymentDetailsByOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.paymentDetail = action.payload;
            })
            .addCase(fetchPaymentDetailsByOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default paymentSlice.reducer;