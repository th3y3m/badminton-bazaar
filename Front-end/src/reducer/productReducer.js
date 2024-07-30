import { FETCH_PRODUCTS_ERROR, FETCH_PRODUCTS_REQUEST, FETCH_PRODUCTS_SUCCESS } from "../redux/types";

const INITIAL_STATE = {
    products: [],
    isLoading: false,
    isError: false,
};

const productReducer = (state = INITIAL_STATE, action) => {
    switch (action.type) {
        case FETCH_PRODUCTS_REQUEST:
            return {
                ...state,
                isLoading: true,
                isError: false,
            };
        case FETCH_PRODUCTS_SUCCESS:
            return {
                ...state,
                products: action.payload, products: action.payload.items || [],
                isLoading: false,
                isError: false,
            };
        case FETCH_PRODUCTS_ERROR:
            return {
                ...state,
                isLoading: false,
                isError: true,
            };
        default:
            return state;
    }
}

export default productReducer;