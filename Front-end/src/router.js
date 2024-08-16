import { Navigate, Route, Routes } from "react-router-dom";
import MasterLayout from "./components/Layour/Layout";
import { ROUTERS } from './utils/Routers';
import HomePage from "./components/Home/Home";
import ProductPage from "./components/Product/ProductsPage";
import Login from "./components/Login/Login";
import CartPage from "./components/Cart/CartPage";
import Register from "./components/Register/Register";
import ProductDetailsPage from "./components/Product/ProductDetailsPage";
import NewsPage from "./components/News/NewsPage";
import NewsList from "./components/News/NewsList";
import Profile from "./components/Profile/Profile";
import CheckOutPage from "./components/Checkout/CheckoutPage";
import PaymentFailed from "./components/Payment/PaymentFailed";
import PaymentSuccessful from "./components/Payment/PaymentSuccessful";
import ErrorPage from "./components/Error/ErrorPage";
import OrdersPage from "./components/Orders/OrdersPage";
import OrderDetail from "./components/Orders/OrderDetails";

const ProtectedRoute = ({ component: Component, allowedRoles, ...rest }) => {
    const userRole = localStorage.getItem('userRole');

    if (userRole && !allowedRoles.includes(userRole)) {
        return <Navigate to={ROUTERS.USER.LOGIN} />;
    }

    return <Component {...rest} />;
};

const renderUserRouter = () => {
    const userRouters = [
        { path: ROUTERS.USER.HOME, component: HomePage },
        { path: ROUTERS.USER.PROFILE, component: Profile },
        { path: ROUTERS.USER.NEWS, component: NewsList },
        { path: ROUTERS.USER.NEWSDETAILS + "/:id", component: NewsPage },
        { path: ROUTERS.USER.PAYMENTFAILED, component: PaymentFailed },
        { path: ROUTERS.USER.CART, component: CartPage },
        { path: ROUTERS.USER.REGISTER, component: Register },
        { path: ROUTERS.USER.PRODUCTDETAILS + "/:id", component: ProductDetailsPage },
        { path: ROUTERS.USER.CHECKOUT, component: CheckOutPage },
        { path: ROUTERS.USER.PRODUCTS, component: ProductPage },
        { path: ROUTERS.USER.PAYMENTSUCCESSFUL, component: PaymentSuccessful },
        { path: ROUTERS.USER.ORDERS, component: OrdersPage },
        { path: ROUTERS.USER.ORDERDETAILS + "/:id", component: OrderDetail },
    ];

    return (
        <MasterLayout>
            <Routes>
                {userRouters.map((item, key) => (
                    <Route
                        key={key}
                        path={item.path}
                        element={
                            <ProtectedRoute component={item.component} allowedRoles={['Customer', '']} />
                        }
                    />
                ))}
                {/* 404 Error Page Route */}
                <Route path="*" element={<ErrorPage />} />
            </Routes>
        </MasterLayout>
    );
};

const RouterCustom = () => {
    return (
        <Routes>
            <Route path={ROUTERS.USER.LOGIN} element={<Login />} />
            <Route path="/*" element={renderUserRouter()} />
        </Routes>
    );
};

export default RouterCustom;
