import { fetchPaginatedOrders } from "../../api/orderAxios";

const OrdersPage = () => {

    const [orders, setOrders] = useState([]);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        const getOrders = async () => {
            try {
                const data = await fetchPaginatedOrders({
                    start,
                    end,
                    sortBy : "orderdate_asc",
                    status : null,
                    pageIndex : currentPage,
                    pageSize : 10
                })
                setOrders(data);
            } catch (error) {
                console.error("Error fetching orders:", error);
            }
        };
        getOrders();
    }, []);

    return (
        <div>
            <h1>Orders Page</h1>
        </div>
    );

}

export default OrdersPage;