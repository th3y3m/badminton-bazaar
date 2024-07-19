import { useState, useEffect } from 'react';
import { removeFromCart, deleteUnitItem, addToCart } from "../../api/cartAxios";
import { fetchProductById } from "../../api/productAxios";

const ProductRow = (cartItem) => {
    const [product, setProduct] = useState({});
    const [quantity, setQuantity] = useState(cartItem.quantity);

    
    const handleRemoveOne = () => {
        if (quantity > 1) {
            deleteUnitItem(cartItem.itemId, cartItem.userId).then(() => {
                setQuantity(prevQuantity => prevQuantity - 1);
                cartItem.updateCart();
            });
        }
    };
    
    const handleRemove = () => {
        removeFromCart(cartItem.itemId, cartItem.userId);
        cartItem.updateCart();
    };
    const handleAddOne = () => {
        addToCart(cartItem.itemId, cartItem.userId).then(() => {
            setQuantity(prevQuantity => prevQuantity + 1);
            cartItem.updateCart();
        });
    };

    useEffect(() => {
        const getProduct = async () => {
            try {
                const data = await fetchProductById(cartItem.itemId);
                setProduct(data);
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        };
        getProduct();
    }, [cartItem.itemId]);

    return (
        <div className="flex flex-col items-center justify-center w-64 h-96 bg-white rounded-lg shadow-md">
            <img src={product.imageUrl} alt={product.productName} className="w-full" />
            <h3 className="text-lg font-semibold mt-2">{product.productName}</h3>
            <div className="flex items-center">
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg" onClick={handleRemoveOne}>-</button>
                <p className="text-red-600 font-bold text-2xl mx-4">{quantity}</p>
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg" onClick={handleAddOne}>+</button>
            </div>
            <p className="text-lg font-semibold mt-2">{cartItem.unitPrice} $</p>
            <p className="text-lg font-semibold mt-2">{quantity * cartItem.unitPrice} $</p>
            <button className="bg-red-600 text-white px-4 py-2 rounded-lg mt-2" onClick={handleRemove}>Remove</button>
        </div>
    );
}

export default ProductRow;