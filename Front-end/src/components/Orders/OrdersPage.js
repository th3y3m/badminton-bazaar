import { useDispatch, useSelector } from "react-redux";
import { fetchPaginatedOrders } from "../../api/orderAxios";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { useEffect, useState } from "react";
import ReactPaginate from "react-paginate";
import { Modal } from "@mui/material";

const OrdersPage = () => {
    const navigate = useNavigate();

    const user = useSelector((state) => state.auth.token);

    const [orders, setOrders] = useState([]);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const [confirmModal, setConfirmModal] = useState(false);

    useEffect(() => {
        setIsLoading(true);
        const getOrders = async () => {
            try {
                const data = await fetchPaginatedOrders({
                    userId: user.id,
                    sortBy: "orderdate_desc",
                    status: null,
                    pageIndex: currentPage,
                    pageSize: 10
                });
                setOrders(data);
                if (data && data.totalPages) {
                    setTotalPage(data.totalPages);
                }
            } catch (error) {
                console.error("Error fetching orders:", error);
            }
            setIsLoading(false);
        };
        getOrders();
    }, [currentPage, user.id]);

    const handlePageClick = (data) => {
        let selectedPage = data.selected + 1;
        setCurrentPage(selectedPage);
    };
    const handleCancelOrder = (id) => {
        setConfirmModal(true);
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-8 text-center">Orders Page</h1>
            <div className="overflow-x-auto">
                <div className="grid grid-cols-7 gap-4 text-center bg-gray-200 p-4 rounded-t-lg">
                    <h3 className="font-semibold">Order Id</h3>
                    <h3 className="font-semibold">Order Date</h3>
                    <h3 className="font-semibold col-span-2">Ship Address</h3>
                    <h3 className="font-semibold">Shipped Date</h3>
                    <h3 className="font-semibold">Status</h3>
                    <span></span>
                </div>
                {isLoading ? (
                    <div className="flex justify-center items-center py-8">
                        <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                    </div>
                ) : orders && orders.items && orders.items.length > 0 ? (
                    orders.items.map((order) => (
                        <div key={order.orderId} className="grid grid-cols-7 gap-4 text-center bg-white p-4 border-b border-gray-200">
                            <p>{order.orderId}</p>
                            <p>{new Date(order.orderDate).toLocaleString('en-GB', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })}</p>
                            <p className="col-span-2">{order.shipAddress}</p>
                            <p>{new Date(order.shippedDate).toLocaleString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}</p>
                            <p>{order.status}</p>
                            <div>

                                <button
                                    onClick={() => navigate(`/order-details/${order.orderId}`)}
                                    className="text-blue-500 hover:underline"
                                >
                                    View
                                </button>
                                <button
                                    onClick={() => handleCancelOrder(order.orderId)}
                                    className="text-red-700 hover:underline"
                                >
                                    Cancel
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <div className="text-center py-4">
                        <p>No orders found.</p>
                    </div>
                )}
            </div>
            {totalPage > 1 && (
                <div className="flex justify-center mt-8">
                    <ReactPaginate
                        breakLabel="..."
                        nextLabel="Next >"
                        onPageChange={handlePageClick}
                        pageRangeDisplayed={3}
                        pageCount={totalPage}
                        previousLabel="< Previous"
                        renderOnZeroPageCount={null}
                        containerClassName="flex items-center space-x-1"
                        pageClassName="flex items-center"
                        pageLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                        previousLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                        nextLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                        breakLinkClassName="px-3 py-2 border border-gray-300 text-gray-700"
                        activeLinkClassName="bg-blue-500 text-white hover:bg-blue-600"
                    />
                </div>
            )}
            <Modal open={confirmModal}>
                <div className="bg-white p-8 rounded-lg w-96">
                    <h2 className="text-xl font-bold mb-4">Cancel Order</h2>
                    <p>Are you sure you want to cancel this order?</p>
                    <div className="flex justify-end mt-4">
                        <button
                            onClick={() => setConfirmModal(false)}
                            className="bg-gray-200 text-gray-700 px-4 py-2 rounded-lg mr-4"
                        >
                            Cancel
                        </button>
                        <button
                            onClick={() => setConfirmModal(false)}
                            className="bg-red-500 text-white px-4 py-2 rounded-lg"
                        >
                            Confirm
                        </button>
                    </div>
                </div>
            </Modal>
        </div>
    );
};

export default OrdersPage;
