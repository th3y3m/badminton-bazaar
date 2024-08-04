import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import {
    deleteUnitItem,
    getCart,
    removeFromCart,
    deleteCookie,
    numberOfItemsInCart,
    saveCartToCookie
} from '../../api/cartAxios'; // Adjust the import path as needed

// Define async thunks
export const fetchCart = createAsyncThunk(
    'cart/fetchCart',
    async (userId, thunkAPI) => {
        try {
            const data = await getCart(userId);
            return data;
        } catch (error) {
            return thunkAPI.rejectWithValue(error);
        }
    }
);

export const addToCookie = createAsyncThunk(
    'cart/addToCookie',
    async ({ productId, userId }, thunkAPI) => {
        const response = await saveCartToCookie(productId, userId);
        return response.data;
    }
);

export const removeItem = createAsyncThunk(
    'cart/removeItem',
    async ({ productId, userId }, thunkAPI) => {
        const response = await removeFromCart(productId, userId);
        return response.data;
    }
);

export const deleteAUnitItem = createAsyncThunk(
    'cart/deleteAUnitItem',
    async ({ productId, userId }, thunkAPI) => {
        const response = await deleteUnitItem(productId, userId);
        return response.data;
    }
);

export const clearCartCookie = createAsyncThunk(
    'cart/clearCartCookie',
    async (userId, thunkAPI) => {
        const response = await deleteCookie(userId);
        return response.data;
    }
);

export const fetchNumberOfItems = createAsyncThunk(
    'cart/fetchNumberOfItems',
    async (userId, thunkAPI) => {
        const response = await numberOfItemsInCart(userId);

        return response;
    }
);

// Initial state
const initialState = {
    cart: [],
    status: 'idle',
    error: null,
    itemsCount: 0
};

// Create the slice
const cartSlice = createSlice({
    name: 'cart',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchCart.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchCart.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.cart = action.payload;
            })
            .addCase(fetchCart.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(addToCookie.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(addToCookie.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.cart.push(action.payload);
            })
            .addCase(addToCookie.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(removeItem.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(removeItem.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.cart = state.cart.filter(item => item.productId !== action.payload.productId);
            })
            .addCase(removeItem.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(deleteAUnitItem.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(deleteAUnitItem.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.cart = state.cart.filter(item => item.productId !== action.payload.productId);
            })
            .addCase(deleteAUnitItem.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(clearCartCookie.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(clearCartCookie.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.cart = [];
            })
            .addCase(clearCartCookie.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            })
            .addCase(fetchNumberOfItems.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchNumberOfItems.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.itemsCount = action.payload ? action.payload : 0;
            })
            .addCase(fetchNumberOfItems.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.error.message;
            });
    }
});

// Export the reducer
export default cartSlice.reducer;
