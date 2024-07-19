import { useContext, useEffect, useState } from 'react';
import { getCart, saveCart } from "../../api/cartAxios";
import { AuthContext } from '../../AuthContext'
import ProductRow from './ProductRow'; // Assuming the path
import { useNavigate } from 'react-router-dom';
import { createOrder } from '../../api/orderAxios';


const CartPage = () => {
    const [cartItems, setCartItems] = useState([]);
    const [totalPrice, setTotalPrice] = useState(0);
    const { user } = useContext(AuthContext);
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
        getCartDetails(user.id);
        saveCart(user.id);
    };

    const handlePlaceOrders = () => {
        createOrder(user.id);
        navigate('/orders');
    };


    useEffect(() => {
        if (user) {
            getCartDetails(user.id);
        }
    }, [user]); // Removed cartItems from dependency array to avoid infinite loop

    return (
        <div>
            <h1>Cart Details</h1>
            {cartItems.length === 0 ? <p>Your Cart is Empty</p> : (
                <div>
                    <div className="flex justify-center items-center">
                        {cartItems.map((item) => (
                            <ProductRow key={item.itemId} {...item} updateCart={handleUpdateCart} />
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