import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

// const addToCart = async (productId, userId) => {
//     return await axios.post(`Cart/Add?productId=${productId}&userId=${userId}`);
// };

const saveCartToCookie = async (productId, userId) => {
    try {
        return await axios.post(`Cart/AddToCookie?productId=${productId}&userId=${userId}`);
    } catch (error) {
        console.error("Error saving cart to cookie:", error);
        throw error;
    }
};

const deleteUnitItem = async (productId, userId) => {
    try {
        return await axios.post(`Cart/DeleteUnitItem?productId=${productId}&userId=${userId}`);
    } catch (error) {
        console.error("Error deleting unit item:", error);
        throw error;
    }
};

const getCart = async (id) => {
    try {
        if (id) {
            const response = await axios.get(`Cart/GetCart?userId=${id}`);
            return response;
        } else {
            const response = await axios.get(`Cart/GetCart`);
            return response;
        }
    } catch (error) {
        console.error("Error fetching cart:", error);
        throw error;
    }
};

// const saveCart = async (userId) => {
//     return await axios.post(`Cart/save?userId=${userId}`);
// };

// const clearCart = async (userId) => {
//     return await axios.post(`Cart/clear?userId=${userId}`);
// };

const removeFromCart = async (productId, userId) => {
    try {
        return await axios.post(`Cart/remove?productId=${productId}&userId=${userId}`);
    } catch (error) {
        console.error("Error removing from cart:", error);
        throw error;
    }
};

// const updateCart = async (productId, quantity, userId) => {
//     return await axios.post(`Cart/update?userId=${userId}&quantity=${quantity}`, { productId });
// };

const deleteCookie = async (userId) => {
    try {
        return await axios.post(`Cart/DeleteCookie?userId=${userId}`);
    } catch (error) {
        console.error("Error deleting cookie:", error);
        throw error;
    }
};

const numberOfItemsInCart = async (userId) => {
    try {
        return await axios.get(`Cart/ItemsInCart?userId=${userId}`);
    } catch (error) {
        console.error("Error getting number of items in cart:", error);
        throw error;
    }
};

export {
    // addToCart,
    deleteUnitItem,
    getCart,
    // saveCart,
    // clearCart,
    removeFromCart,
    // updateCart,
    deleteCookie,
    numberOfItemsInCart,
    saveCartToCookie
};