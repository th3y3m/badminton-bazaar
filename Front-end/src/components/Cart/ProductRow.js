import { useState, useEffect, useContext } from 'react';
import { removeFromCart, deleteUnitItem, saveCartToCookie } from "../../api/cartAxios";
import { fetchProductVariantById } from '../../api/productVariantAxios';
import { fetchProductById } from '../../api/productAxios';
import { AuthContext } from '../../AuthContext';

const ProductRow = (cartItem) => {
    const { user } = useContext(AuthContext);
    const { cartCount, setCartCount } = useContext(AuthContext);
    const [productVariant, setProductVariant] = useState({});
    const [quantity, setQuantity] = useState(cartItem.quantity);
    const [product, setProduct] = useState({});
    
    
    
    const handleRemove = () => {
        removeFromCart(cartItem.itemId, user.userId);
        cartItem.updateCart();
    };
    const handleRemoveOne = () => {
        if (quantity > 1) {
            deleteUnitItem(cartItem.itemId, cartItem.userId).then(() => {
                setQuantity(prevQuantity => prevQuantity - 1);
                setCartCount(prevQuantity => prevQuantity - 1);
                cartItem.updateCart();
                cartItem.onQuantityChange(); // Trigger recalculation of total price
            });
        }
    };
    
    const handleAddOne = () => {
        saveCartToCookie(cartItem.itemId, cartItem.userId).then(() => {
            setQuantity(prevQuantity => prevQuantity + 1);
            setCartCount(prevQuantity => prevQuantity + 1);
            cartItem.updateCart();
            cartItem.onQuantityChange(); // Trigger recalculation of total price
        });
    };

    const getProduct = async () => {
        try {
            const data = await fetchProductById(productVariant.productId);
            setProduct(data);
        } catch (error) {
            console.error("Error fetching products:", error);
        }
    }

    useEffect(() => {
        getProduct();
    }, [productVariant.productId, cartCount]);

    useEffect(() => {
        const getProductVariant = async () => {
            try {
                const data = await fetchProductVariantById(cartItem.itemId);
                console.log(data);

                setProductVariant(data);
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        };
        getProductVariant();
        console.log(productVariant);
    }, [cartItem.itemId, ]);

    return (
        <div className="grid grid-cols-7 flex-col items-center justify-center w-full h-96 bg-white rounded-lg shadow-md">
            <img src={productVariant.variantImageURL} alt={product.productName} className="w-full col-span-1" />
            <h3 className="text-lg font-semibold mt-2 col-span-2">{product.productName}</h3>
            <p className="text-lg font-semibold mt-2 col-span-1 text-center">{cartItem.unitPrice} $</p>
            <div className="flex items-center col-span-1 justify-center">
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg text-center" onClick={handleRemoveOne}>-</button>
                <p className="text-red-600 font-bold text-2xl mx-4 text-center">{quantity}</p>
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg text-center" onClick={handleAddOne}>+</button>
            </div>
            <p className="text-lg font-semibold mt-2 col-span-1 text-center">{quantity * cartItem.unitPrice} $</p>
            <button className="bg-red-600 text-white px-4 py-2 rounded-lg mt-2 col-span-1" onClick={handleRemove}>Remove</button>
        </div>
    );
}

export default ProductRow;