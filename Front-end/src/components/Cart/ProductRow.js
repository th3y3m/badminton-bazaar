import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchProduct } from '../../redux/slice/productSlice';
import { fetchProductVariant } from '../../redux/slice/productVariantSlice';
import { addToCookie, deleteAUnitItem, fetchCart, fetchNumberOfItems, removeItem } from '../../redux/slice/cartSlice';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const ProductRow = (cartItem) => {
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);

    const product = useSelector((state) => state.product.product);
    const productStatus = useSelector((state) => state.product.status);
    const productError = useSelector((state) => state.product.error);

    const productVariant = useSelector((state) => state.productVariant.productVariantDetail);
    const productVariantStatus = useSelector((state) => state.productVariant.status);
    const productVariantError = useSelector((state) => state.productVariant.error);

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
        dispatch(fetchProductVariant(cartItem.itemId));
    }, [cartItem.itemId, dispatch]);

    useEffect(() => {
        if (productVariant) {

            dispatch(fetchProduct(productVariant.productId));
        }
    }, [cartItem.itemId, dispatch]);

    if (productError === 'failed' || productVariantError === 'failed') {
        return <div>Error: {productError || productVariantError}</div>;
    }
    if (productStatus === 'loading' || productVariantStatus === 'loading') {
        return <div className="text-blue-500">
            <FontAwesomeIcon icon={faSpinner} spin />
        </div>;
    }

    return (
        <div className="grid grid-cols-7 flex-col items-center justify-center w-full h-96 bg-white rounded-lg shadow-md">
            <img src={productVariant.variantImageURL} alt={product.productName} className="w-full col-span-1" />
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