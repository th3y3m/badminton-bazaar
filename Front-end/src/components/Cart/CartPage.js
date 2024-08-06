import { useEffect, useState } from 'react';
import ProductRow from './ProductRow'; // Assuming the path
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { fetchCart } from '../../redux/slice/cartSlice';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { ROUTERS } from '../../utils/Routers';

const CartPage = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const user = useSelector((state) => state.auth.token);

    const cartItems = useSelector((state) => state.cart.cart);
    const cartItemsStatus = useSelector((state) => state.cart.status);
    const cartItemsError = useSelector((state) => state.cart.error);

    const [totalPrice, setTotalPrice] = useState(0);

    useEffect(() => {
        if (user?.id) {
            console.log("Fetching cart for user:", user.id);
            dispatch(fetchCart(user.id)).catch(error => {
                console.error("Error fetching cart:", error);
            });
        }
    }, [dispatch, user?.id]);

    useEffect(() => {
        const total = cartItems.reduce((acc, item) => acc + item.unitPrice * item.quantity, 0);
        setTotalPrice(total);
    }, [cartItems]);

    const handlePlaceOrders = () => {
        navigate(ROUTERS.USER.CHECKOUT);
    };

    if (cartItemsStatus === 'loading') {
        return <div className="text-blue-500">
            <FontAwesomeIcon icon={faSpinner} spin />
        </div>
    }

    if (cartItemsStatus === 'failed') {
        return <div>Error: {cartItemsError}</div>;
    }

    return (
        <div className='container mx-auto'>
            <h1 className='text-center my-16 text-4xl'>Cart Details</h1>
            {cartItems && cartItems.length === 0 ? (
                <p>Your Cart is Empty</p>
            ) : (
                <div>
                    <div>
                        <div className="grid grid-cols-7 flex-col justify-center w-full bg-gray-400 rounded-lg shadow-md p-4 gap-4">
                            <div className="col-span-1">
                                <h3 className='text-center font-semibold'>Image</h3>
                            </div>
                            <div className="col-span-2">
                                <h3 className='text-center font-semibold'>Product Name</h3>
                            </div>
                            <div className="col-span-1">
                                <h3 className='text-center font-semibold'>Unit Price</h3>
                            </div>
                            <div className="col-span-1">
                                <h3 className='text-center font-semibold'>Quantity</h3>
                            </div>
                            <div className="col-span-1">
                                <h3 className='text-center font-semibold'>Price</h3>
                            </div>
                            <div className="col-span-1">
                                <h3 className='text-center font-semibold'>Remove</h3>
                            </div>
                        </div>
                        {cartItems.map((item) => (
                            <ProductRow key={item.itemId} {...item} />
                        ))}
                    </div>
                    <div className="flex my-6 justify-end">
                        <p>Total price: <strong className="text-red-600">{totalPrice} $</strong></p>
                    </div>

                    <div className='flex my-6 justify-end'>
                        <button className="bg-blue-500 text-white px-4 py-2 rounded-lg" onClick={handlePlaceOrders}>Place Orders</button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default CartPage;