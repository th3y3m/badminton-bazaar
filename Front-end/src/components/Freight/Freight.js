import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchFreightPrices } from "../../redux/slice/freightPriceSlice";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const FreightPrice = () => {
    const dispatch = useDispatch();

    const prices = useSelector((state) => state.freightPrice.freightPrices);
    const pricesStatus = useSelector((state) => state.freightPrice.status);

    useEffect(() => {
        dispatch(fetchFreightPrices()).catch(error => {
            console.error("Error fetching freight prices:", error);
        });
    }, []);

    if (pricesStatus === 'loading') {
        return (
            <div className="text-blue-500 flex justify-center items-center">
                <FontAwesomeIcon icon={faSpinner} spin />
            </div>
        );
    }

    if (pricesStatus === 'failed') {
        return <div className="text-red-500">Error loading freight prices</div>;
    }

    return (
        <div className="p-4">
            <h1 className="text-lg font-bold mb-2">Freight Price</h1>
            <div className="space-y-2">
                {prices && prices.map((price) => (
                    <div key={price.id} className="flex justify-between">
                        <p>{price.minDistance} - {price.maxDistance} km: </p>
                        <p className="font-semibold">${price.pricePerKm}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default FreightPrice;
