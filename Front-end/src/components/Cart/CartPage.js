import { useContext, useEffect, useState } from 'react';
import { getCart } from "../../api/cartAxios";
import { AuthContext } from '../../AuthContext'
import ProductRow from './ProductRow'; // Assuming the path
import { useNavigate } from 'react-router-dom';
import { createOrder } from '../../api/orderAxios';


const CartPage = () => {
    const [cartItems, setCartItems] = useState([]);
    const [totalPrice, setTotalPrice] = useState(0);
    const { user, cartCount } = useContext(AuthContext);
    const navigate = useNavigate();

    const getCartDetails = async (userId) => {
        try {
            const data = await getCart(userId);
            setCartItems(data);
            // Calculate total price here after fetching cart items
            const total = data.reduce((acc, item) => acc + item.unitPrice * item.quantity, 0);
            setTotalPrice(total);
        } catch (error) {
            console.error("Error fetching products:", error);
        }
    }
    const handleUpdateCart = () => {
        getCartDetails(user.userId);
    };

    const handlePlaceOrders = () => {
        createOrder(user.userId);
        navigate('/orders');
    };


    useEffect(() => {
        if (user) {
            getCartDetails(user.userId);
        }
    }, [cartCount]);

    return (
        <div className='container mx-auto'>
            <h1 className='text-center my-16 text-4xl'>Cart Details</h1>
            {cartItems.length === 0 ? <p>Your Cart is Empty</p> : (
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
                            <ProductRow key={item.itemId} {...item} updateCart={handleUpdateCart} onQuantityChange={handleUpdateCart} />                        ))}
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