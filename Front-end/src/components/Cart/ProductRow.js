import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { addToCookie, deleteAUnitItem, fetchCart, fetchNumberOfItems, removeItem } from '../../redux/slice/cartSlice';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { fetchProductVariantById } from '../../api/productVariantAxios';

const ProductRow = ({ cartItem }) => {
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);

    const [productVariant, setProductVariant] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    const handleRemove = () => {
        dispatch(removeItem({ productId: cartItem.itemId, userId: user.id }))
            .then(() => {
                dispatch(fetchNumberOfItems(user.id)); // Dispatch here after adding to cart
            })
            .then(() => {
                dispatch(fetchCart(user.id)); // Dispatch here after adding to cart
            })
            .catch((error) => {
                console.error("Error adding to cart:", error);
            });
    };
    const handleRemoveOne = () => {
        if (cartItem.quantity > 1) {
            dispatch(deleteAUnitItem({ productId: cartItem.itemId, userId: user.id }))
                .then(() => {
                    dispatch(fetchNumberOfItems(user.id)); // Dispatch here after adding to cart
                })
                .then(() => {
                    dispatch(fetchCart(user.id)); // Dispatch here after adding to cart
                })
                .catch((error) => {
                    console.error("Error adding to cart:", error);
                });
        }
    };

    const handleAddOne = () => {
        dispatch(addToCookie({ productId: cartItem.itemId, userId: user.id }))
            .then(() => {
                dispatch(fetchNumberOfItems(user.id));
            })
            .then(() => {
                dispatch(fetchCart(user.id)); // Dispatch here after adding to cart
            })
            .catch((error) => {
                console.error("Error adding to cart:", error);
            });
    };

    useEffect(() => {
        const fetchProduct = async (id) => {
            setIsLoading(true);
            const response = await fetchProductVariantById(id);
            setProductVariant(response);
            setIsLoading(false);
        };
        fetchProduct(cartItem.itemId);
    }, [cartItem.itemId]);


    if (isLoading) {
        return <div className="text-blue-500">
            <FontAwesomeIcon icon={faSpinner} spin />
        </div>
    }

    return (
        <div className="grid grid-cols-7 flex-col items-center justify-center w-full h-96 bg-white rounded-lg shadow-md">
            <img src={productVariant.variantImageURL} alt={cartItem.itemName} className="w-full col-span-1" />
            <h3 className="text-lg font-semibold mt-2 col-span-2">{cartItem.itemName}</h3>
            <p className="text-lg font-semibold mt-2 col-span-1 text-center">{cartItem.unitPrice} $</p>
            <div className="flex items-center col-span-1 justify-center">
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg text-center" onClick={handleRemoveOne}>-</button>
                <p className="text-red-600 font-bold text-2xl mx-4 text-center">{cartItem.quantity}</p>
                <button className="bg-blue-500 text-white px-2 py-1 rounded-lg text-center" onClick={handleAddOne}>+</button>
            </div>
            <p className="text-lg font-semibold mt-2 col-span-1 text-center">{cartItem.quantity * cartItem.unitPrice} $</p>
            <button className="bg-red-600 text-white px-4 py-2 rounded-lg mt-2 col-span-1" onClick={handleRemove}>Remove</button>
        </div>
    );
}

export default ProductRow;