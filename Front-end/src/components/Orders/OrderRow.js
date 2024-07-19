const OrderRow = (order) => {

    const [totalPrice, setTotalPrice] = useState(0);

    const calculateTotalPrice = () => {
        const total = order.items.reduce((acc, item) => acc + item.unitPrice * item.quantity, 0);
        setTotalPrice(total);
    }

    return (
        <div>


        </div>
    );
}

export default OrderRow;