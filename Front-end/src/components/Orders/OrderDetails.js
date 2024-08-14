import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchSingleOrder } from "../../redux/slice/orderSlice";
import { fetchOrderDetail } from "../../redux/slice/orderDetailSlice";
import { fetchUserDetail } from "../../redux/slice/userDetailSlice";

const orderDetail = () => {
    const { id: orderId } = useParams();
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);

    const order = useSelector((state) => state.order.singleOrder);
    const orderStatus = useSelector((state) => state.order.status);

    const orderDetails = useSelector((state) => state.orderDetail.orderDetails);
    const orderDetailsStatus = useSelector((state) => state.orderDetail.status);

    const payment = useSelector((state) => state.payment.paymentDetail);
    const paymentStatus = useSelector((state) => state.payment.status);

    const userDetail = useSelector((state) => state.userDetails.userDetail);

    useEffect(() => {
        if (user && user.id) {
            dispatch(fetchUserDetail(user.id));
        }
        dispatch(fetchSingleOrder(orderId));

        if (order) {
            dispatch(fetchOrderDetail(orderId));
        }
        if (payment) {
            dispatch(fetchPaymentDetailsByOrder(orderId));
        }
    }, [dispatch, orderId]);

    return (
        <div className="container mx-auto">
            <div className="grid grid-cols-2">
                <div>
                    <h2>Order</h2>
                    {orderStatus === "loading" ? (
                        <div className="flex justify-center items-center py-8">
                            <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                        </div>
                    ) : orderStatus === "failed" ? (
                        <div>Error</div>
                    ) : (
                        <div>
                            <p>Order Id: {order.orderId}</p>
                            <p>User Id: {order.userId}</p>
                            <p>Order Date: {order.orderDate}</p>
                            <p>Ship Address: {order.shipAddress}</p>
                            <p>Status: {order.status}</p>
                        </div>
                    )}
                </div>
                <div>
                    <h2>Order Details</h2>
                    {orderDetailsStatus === "loading" ? (
                        <div>Loading...</div>
                    ) : orderDetailsStatus === "failed" ? (
                        <div>Error</div>
                    ) : (
                        <div>
                            {orderDetails.map((orderDetail) => (
                                <div key={orderDetail.id}>
                                    <p>Product Name: {orderDetail.productName}</p>
                                    <p>Quantity: {orderDetail.quantity}</p>
                                    <p>Price: {orderDetail.price}</p>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default orderDetail;