import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchSingleOrder } from "../../redux/slice/orderSlice";
import { fetchOrderDetail } from "../../redux/slice/orderDetailSlice";
import { fetchUserDetail } from "../../redux/slice/userDetailSlice";
import { useParams } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { fetchPaymentDetailsByOrder } from "../../redux/slice/paymentSlice";
import ItemCard from "./ItemCard";

const OrderDetail = () => {
    const { id: orderId } = useParams();
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);

    const order = useSelector((state) => state.order.singleOrder);
    const orderStatus = useSelector((state) => state.order.status);

    const orderDetails = useSelector((state) => state.orderDetail.orderDetail);
    const orderDetailsStatus = useSelector((state) => state.orderDetail.status);

    const payment = useSelector((state) => state.payment.paymentDetail);
    const paymentStatus = useSelector((state) => state.payment.status);

    const userDetail = useSelector((state) => state.userDetails.userDetail);

    useEffect(() => {

        dispatch(fetchSingleOrder(orderId));
        dispatch(fetchOrderDetail(orderId));
        dispatch(fetchPaymentDetailsByOrder(orderId));
    }, [dispatch, orderId]);
    useEffect(() => {
        if (user && user.id) {
            dispatch(fetchUserDetail(user.id));
        }
    }, [dispatch, user.id]);

    return (
        <div className="container mx-auto p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white shadow-lg rounded-lg p-6">
                    <h2 className="text-xl font-bold mb-4">Order</h2>
                    {orderStatus === "loading" ? (
                        <div className="flex justify-center items-center py-8">
                            <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                        </div>
                    ) : orderStatus === "failed" ? (
                        <div className="text-red-500 font-semibold">Failed to load order details.</div>
                    ) : (
                        <div className="space-y-2">
                            <p><span className="font-semibold">Order Id:</span> {order.orderId}</p>
                            <p><span className="font-semibold">Name:</span> {userDetail.fullName}</p>
                            <p><span className="font-semibold">Order Date:</span> {new Date(order.orderDate).toLocaleString('en-GB', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })}</p>
                            <p><span className="font-semibold">Ship Address:</span> {order.shipAddress}</p>
                            <p><span className="font-semibold">Status:</span> {order.status}</p>
                        </div>
                    )}
                    <h2 className="text-xl font-bold mt-8 mb-4">Payment</h2>
                    {paymentStatus === "loading" ? (
                        <div className="flex justify-center items-center py-8">
                            <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                        </div>
                    ) : paymentStatus === "failed" ? (
                        <div className="text-red-500 font-semibold">Failed to load payment details.</div>
                    ) : (
                        <div className="space-y-2">
                            <p><span className="font-semibold">Payment Id:</span> {payment.paymentId}</p>
                            <p><span className="font-semibold">Payment Date:</span> {new Date(payment.paymentDate).toLocaleString('en-GB', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })}</p>
                            <p><span className="font-semibold">Amount:</span> ${payment.paymentAmount}</p>
                            <p><span className="font-semibold">Payment Message:</span> {payment.paymentMessage}</p>
                            <p><span className="font-semibold">Payment Method:</span> {payment.paymentMethod}</p>
                            <p><span className="font-semibold">Payment Status:</span> {payment.paymentStatus}</p>
                        </div>
                    )}
                </div>
                <div className="bg-white shadow-lg rounded-lg p-6">
                    <h2 className="text-xl font-bold mb-4">Order Details</h2>
                    {orderDetailsStatus === "loading" ? (
                        <div className="flex justify-center items-center py-8">
                            <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                        </div>
                    ) : orderDetailsStatus === "failed" ? (
                        <div className="text-red-500 font-semibold">Failed to load order details.</div>
                    ) : (
                        <div className="space-y-4">
                            {orderDetails && orderDetails.map((orderDetail) => (
                                <div key={orderDetail.orderDetailId} className="border-b py-4">
                                    <ItemCard cartItem={orderDetail} />
                                </div>
                            ))}
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
}

export default OrderDetail;
