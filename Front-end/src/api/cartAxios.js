import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

// const addToCart = async (productId, userId) => {
//     return await axios.post(`Cart/Add?productId=${productId}&userId=${userId}`);
// };
const saveCartToCookie = async (productId, userId) => {
    return await axios.post(`Cart/AddToCookie?productId=${productId}&userId=${userId}`);
};

const deleteUnitItem = async (productId, userId) => {
    return await axios.post(`Cart/DeleteUnitItem?productId=${productId}&userId=${userId}`);
};

const getCart = async (userId) => {
    return await axios.get(`Cart/GetCart?userId=${userId}`);
};

// const saveCart = async (userId) => {
//     return await axios.post(`Cart/save?userId=${userId}`);
// };

// const clearCart = async (userId) => {
//     return await axios.post(`Cart/clear?userId=${userId}`);
// };

const removeFromCart = async (productId, userId) => {
    return await axios.post(`Cart/remove?productId=${productId}&userId=${userId}`);
};

// const updateCart = async (productId, quantity, userId) => {
//     return await axios.post(`Cart/update?userId=${userId}&quantity=${quantity}`, { productId });
// };

const deleteCookie = async (userId) => {
    return await axios.post(`Cart/DeleteCookie?userId=${userId}`);
};

const numberOfItemsInCart = async (userId) => {
    return await axios.get(`Cart/ItemsInCart?userId=${userId}`);
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