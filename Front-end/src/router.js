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
import AdminHomePage from "./components/admin/Home/AdminHomePage";
import AdminProductPage from "./components/admin/Product/AdminProductPage";
import AdminProductDetailsPage from "./components/admin/Product/AdminProductDetailsPage";
import AdminNewsList from "./components/admin/News/AdminNewsList";
import AdminNewsPage from "./components/admin/News/AdminNewsPage";
import AdminOrdersPage from "./components/admin/Order/AdminOrdersPage";
import AdminOrderDetail from "./components/admin/Order/AdminOrderDetail";
import AdminUsersPage from "./components/admin/User/AdminUsersPage";
import AdminUserDetailPage from "./components/admin/User/AdminUserDetailPage";

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

const renderAdminRouter = () => {
    const adminRouters = [
        { path: ROUTERS.ADMIN.HOME, component: AdminHomePage },
        { path: ROUTERS.ADMIN.PRODUCTS, component: AdminProductPage },
        { path: ROUTERS.ADMIN.PRODUCTDETAILS + "/:id", component: AdminProductDetailsPage },
        { path: ROUTERS.ADMIN.NEWS, component: AdminNewsList },
        { path: ROUTERS.ADMIN.NEWSDETAILS + "/:id", component: AdminNewsPage },
        { path: ROUTERS.ADMIN.ORDERS, component: AdminOrdersPage },
        { path: ROUTERS.ADMIN.ORDERDETAILS + "/:id", component: AdminOrderDetail },
        { path: ROUTERS.ADMIN.USERS, component: AdminUsersPage },
        { path: ROUTERS.ADMIN.USERDETAILS + "/:id", component: AdminUserDetailPage },
    ];

    return (
        <Routes>
            {adminRouters.map((item, key) => (
                <Route
                    key={key}
                    path={item.path}
                    element={
                        <ProtectedRoute component={item.component} allowedRoles={['Admin']} />
                    }
                />
            ))}
            {/* 404 Error Page Route */}
            <Route path="*" element={<ErrorPage />} />
        </Routes>
    );
};

const RouterCustom = () => {
    return (
        <Routes>
            {/* User Routes */}
            <Route path={ROUTERS.USER.LOGIN} element={<Login />} />
            <Route path="/*" element={renderUserRouter()} />

            {/* Admin Routes */}
            <Route path={ROUTERS.ADMIN.LOGIN} element={<AdminLogin />} />
            <Route path="/admin/*" element={renderAdminRouter()} />
        </Routes>
    );
};

export default RouterCustom;