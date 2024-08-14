import { useDispatch, useSelector } from "react-redux";
import { fetchPaginatedOrders } from "../../api/orderAxios";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { useEffect, useState } from "react";
import ReactPaginate from "react-paginate";

const OrdersPage = () => {
    const navigate = useNavigate();

    const user = useSelector((state) => state.auth.token);

    const [orders, setOrders] = useState([]);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setIsLoading(true);
        const getOrders = async () => {
            try {
                const data = await fetchPaginatedOrders({
                    userId: user.id,
                    sortBy: "orderdate_asc",
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

    return (
        <div className="container mx-auto px-4 py-8">
            <h1 className="text-2xl font-bold mb-8 text-center">Orders Page</h1>
            <div className="overflow-x-auto">
                <div className="grid grid-cols-7 gap-4 text-center bg-gray-200 p-4 rounded-t-lg">
                    <h3 className="font-semibold">Order Id</h3>
                    <h3 className="font-semibold">Order Date</h3>
                    <h3 className="font-semibold col-span-2">Ship Address</h3>
                    <h3 className="font-semibold">Total Price</h3>
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
                            <p>{order.orderDate}</p>
                            <p className="col-span-2">{order.shipAddress}</p>
                            <p>{order.totalPrice}</p>
                            <p>{order.status}</p>
                            <button
                                onClick={() => navigate(`/order-details/${order.orderId}`)}
                                className="text-blue-500 hover:underline"
                            >
                                View
                            </button>
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
        </div>
    );
};

export default OrdersPage;
