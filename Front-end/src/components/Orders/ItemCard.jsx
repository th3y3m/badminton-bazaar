import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { useEffect, useState } from "react";
import { fetchProductVariantById } from "../../api/productVariantAxios";
import { fetchProductById } from "../../api/productAxios";

const ItemCard = ({ cartItem }) => {
    const [productVariant, setProductVariant] = useState({});
    const [product, setProduct] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    // Fetch product variant when cartItem changes
    useEffect(() => {
        const fetchProductVariant = async (id) => {
            setIsLoading(true);
            const response = await fetchProductVariantById(id);
            setProductVariant(response);
            setIsLoading(false);
        };

        fetchProductVariant(cartItem.productVariantId);
    }, [cartItem.productVariantId]);

    // Fetch product details when productVariant changes
    useEffect(() => {
        const fetchProduct = async (productId) => {
            setIsLoading(true);
            const response = await fetchProductById(productId);
            setProduct(response);
            setIsLoading(false);
        };

        if (productVariant.productId) {
            fetchProduct(productVariant.productId);
        }
    }, [productVariant.productId]);

    if (isLoading) {
        return (
            <div className="text-blue-500 flex justify-center items-center h-full">
                <FontAwesomeIcon icon={faSpinner} spin />
            </div>
        );
    }

    return (
        <div className="grid grid-cols-5 gap-4 items-center h-32 p-2">
            <div className="col-span-1 relative">
                <img src={productVariant.variantImageURL} alt={product.productName} className="w-24 h-24 object-cover rounded" />
                <span className="absolute top-0 right-0 bg-gray-800 text-white text-xs font-bold rounded-full px-2 py-1">
                    {cartItem.quantity}
                </span>
            </div>
            <div className="col-span-3">
                <h2 className="text-lg font-medium">{product.productName}</h2>
            </div>
            <div className="col-span-1 flex justify-end">
                <p className="text-lg font-semibold">${cartItem.quantity * cartItem.unitPrice}</p>
            </div>
        </div>
    );
};

export default ItemCard;
