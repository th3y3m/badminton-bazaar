import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { useEffect, useState } from "react";
import { fetchProductVariantById } from "../../api/productVariantAxios";

const ProductInformationCard = ({ cartItem }) => {
    const [productVariant, setProductVariant] = useState({});
    const [isLoading, setIsLoading] = useState(false);

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
        return (
            <div className="text-blue-500 flex justify-center items-center h-full">
                <FontAwesomeIcon icon={faSpinner} spin />
            </div>
        );
    }

    return (
        <div className="grid grid-cols-5 gap-1 items-center h-32 p-2 border-b border-gray-200">
            <div className="col-span-1 relative"> {/* Add relative positioning here */}
                <img src={productVariant.variantImageURL} alt={cartItem.itemName} className="w-32 h-32 object-cover" />
                <span className="absolute top-0 right-0 bg-gray-800 text-white text-xs font-bold rounded-full px-2 py-1">
                    {cartItem.quantity}
                </span>
            </div>
            <div className="col-span-3 flex items-center ">
                <h2 className="text-lg font-medium">{cartItem.itemName}</h2>
            </div>
            <div className="col-span-1 flex items-center justify-end">
                <p className="text-lg font-semibold">${cartItem.quantity * cartItem.unitPrice}</p>
            </div>
        </div>
    );
};

export default ProductInformationCard;
