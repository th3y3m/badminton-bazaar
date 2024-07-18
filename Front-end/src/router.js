import { Navigate, Route, Routes } from "react-router-dom";
import MasterLayout from "./components/Layour/Layout";
import { ROUTERS } from './utils/Routers';
import HomePage from "./components/Home/Home";
import ProductPage from "./components/Product/ProductsPage";
import Login from "./components/Login/Login";

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
        // { path: ROUTERS.USER.PROFILE, component: ProfilePage },
        // { path: ROUTERS.USER.NEWS, component: NewsPage },
        // { path: ROUTERS.USER.PAYMENTDETAIL, component: PaymentDetail },
        // { path: ROUTERS.USER.PAYMENTFAILED, component: PaymentFailed },
        // { path: ROUTERS.USER.PAYMENTSUCCESSFUL, component: PaymentSuccessful },
        // { path: ROUTERS.USER.CART, component: Cart },
        // { path: ROUTERS.USER.ORDERS, component: Orders },
        { path: ROUTERS.USER.PRODUCTS, component: ProductPage },
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