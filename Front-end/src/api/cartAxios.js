import axios from './customizeAxios'; // Assuming your axios instance is saved in axios.js

const addtoCart = async (productId, userId) => {
    return await axios.post(`Cart/Add?userId=${userId}`, { productId });
};

const deleteUnitItem = async (productId, userId) => {
    return await axios.post(`Cart/DeleteUnitItem?userId=${userId}`, { productId });
};

const getCart = async (userId) => {
    return await axios.get(`Cart/GetCart?userId=${userId}`);
};

const saveCart = async (userId) => {
    return await axios.post(`Cart/save?userId=${userId}`);
};

const clearCart = async (userId) => {
    return await axios.post(`Cart/clear?userId=${userId}`);
};

const removeFromCart = async (productId, userId) => {
    return await axios.post(`Cart/remove?userId=${userId}`, { productId });
};

const updateCart = async (productId, quantity, userId) => {
    return await axios.post(`Cart/update?userId=${userId}&quantity=${quantity}`, { productId });
};

const deleteCookie = async (userId) => {
    return await axios.post(`Cart/DeleteCookie?userId=${userId}`);
};

const numberOfItemsInCart = async (userId) => {
    return await axios.get(`Cart/ItemsInCart?userId=${userId}`);
};

export {
    addtoCart,
    deleteUnitItem,
    getCart,
    saveCart,
    clearCart,
    removeFromCart,
    updateCart,
    deleteCookie,
    numberOfItemsInCart
};