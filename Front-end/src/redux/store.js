import { configureStore } from '@reduxjs/toolkit'
import productReducer from './slice/productSlice'
import categoryReducer from './slice/categorySlice'
import orderReducer from './slice/orderSlice'
import userReducer from './slice/userSlice'
import orderDetailReducer from './slice/orderDetailSlice'
import colorReducer from './slice/colorSlice'
import sizeReducer from './slice/sizeSlice'
import newsReducer from './slice/newsSlice'
import paymentReducer from './slice/paymentSlice'
import productVariantReducer from './slice/productVariantSlice'
import reviewReducer from './slice/reviewSlice'
import supplierReducer from './slice/supplierSlice'
import userDetailReducer from './slice/userDetailSlice'

const store = configureStore({
  reducer: {
    product: productReducer,
    category: categoryReducer,
    order: orderReducer,
    user: userReducer,
    userDetails: userDetailReducer,
    orderDetail: orderDetailReducer,
    color: colorReducer,
    size: sizeReducer,
    news: newsReducer,
    payment: paymentReducer,
    productVariant: productVariantReducer,
    review: reviewReducer,
    supplier: supplierReducer,
  },
});

export default store;