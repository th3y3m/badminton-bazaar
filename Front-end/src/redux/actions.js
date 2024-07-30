import { fetchPaginatedProducts } from "../api/productAxios";
import { FETCH_PRODUCTS_ERROR, FETCH_PRODUCTS_REQUEST, FETCH_PRODUCTS_SUCCESS } from "./types";

export const fetchAllProducts = () => {
    return async (dispatch) => {
        dispatch({ type: FETCH_PRODUCTS_REQUEST });
        try {
            const data = await fetchPaginatedProducts({
                start: value[0] || null,
                end: value[1] || null,
                searchQuery: "",
                sortBy: sort || "name_asc",
                status: true,
                supplierId: "",
                categoryId: category,
                pageIndex: currentPage,
                pageSize: 12
            });
            dispatch({ type: FETCH_PRODUCTS_SUCCESS, payload: data });
        } catch (error) {
            dispatch({ type: FETCH_PRODUCTS_ERROR, payload: error });
        }
    };
}

export const fetchProductsRequest = () => {
    return {
        type: FETCH_PRODUCTS_REQUEST
    };
}

export const fetchProductsSuccess = (data) => {
    return {
        type: FETCH_PRODUCTS_SUCCESS,
        payload: data
    };
}

export const fetchProductsError = () => {
    return {
        type: FETCH_PRODUCTS_ERROR,
    };
}