import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import ProductInformationCard from "./ProductInformationCard";
import { fetchCart } from "../../redux/slice/cartSlice";
import DisplayMap from "../map/DisplayMap";
import "./style.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMapLocation } from "@fortawesome/free-solid-svg-icons";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const CheckOutPage = () => {
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);
    const cart = useSelector((state) => state.cart.cart);
    const cartItemsStatus = useSelector((state) => state.cart.status);
    const cartItemsError = useSelector((state) => state.cart.error);

    const userDetail = useSelector((state) => state.userDetails.userDetail);
    const account = useSelector((state) => state.user.userDetail);

    const [address, setAddress] = useState(userDetail.address || "");
    const [inputAddress, setInputAddress] = useState(userDetail.address || "");
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        if (user?.id) {
            console.log("Fetching cart for user:", user.id);
            dispatch(fetchCart(user.id)).catch(error => {
                console.error("Error fetching cart:", error);
            });
        }
    }, [dispatch, user?.id]);

    const handleInputAddressChange = (e) => {
        setInputAddress(e.target.value);
    };

    const handleUpdateAddress = () => {
        setAddress(inputAddress);
        console.log("Address updated to:", inputAddress);
    };

    const handleGeolocation = () => {
        setIsLoading(true);
        navigator.geolocation.getCurrentPosition(
            async (position) => {
                const lat = position.coords.latitude;
                const lon = position.coords.longitude;

                try {
                    const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}`);
                    const data = await response.json();

                    if (data && data.display_name) {
                        setInputAddress(data.display_name);
                        setAddress(data.display_name);
                        setIsLoading(false);
                    } else {
                        console.error("No address found for these coordinates");
                    }
                } catch (error) {
                    console.error("Error during reverse geocoding:", error);
                }
            },
            () => {
                console.error("User location permission denied");
            }
        );
    };

    return (
        <div className="container mx-auto p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="p-4 bg-white shadow-md rounded">
                    <h1 className="text-2xl font-bold mb-4">ORDER DETAILS</h1>
                    <p className="mt-7 font-semibold">Delivery Details</p>
                    <div className="mt-4">
                        <p>Email: {account.email}</p>
                        <p>Full Name: {userDetail.fullName}</p>
                    </div>
                    <div className="mt-4">
                        <label htmlFor="address" className="block font-medium">Address</label>
                        {isLoading && (
                            <span className="text-blue-500 flex justify-center items-center h-full">
                                <FontAwesomeIcon icon={faSpinner} spin />
                            </span>
                        )
                        }
                        <div className="flex items-center mt-2">
                            <input
                                type="text"
                                id="address"
                                value={inputAddress}
                                onChange={handleInputAddressChange}
                                className="p-2 border border-gray-300 rounded w-full"
                            />
                            <button
                                onClick={handleGeolocation}
                                className="ml-2 p-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                            >
                                <FontAwesomeIcon icon={faMapLocation} />
                            </button>
                        </div>
                        <button
                            className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                            onClick={handleUpdateAddress}
                        >
                            Update
                        </button>
                    </div>
                    <div className="map-section">
                        <div className="map-form">
                            <div className="map-header">
                                <h2 className="map-title">Store Location</h2>
                                <p className="map-address">819 Hương Lộ 2, Phường Bình Trị Đông A, Quận Bình Tân, TP. HCM Ho Chi Minh</p>
                            </div>
                            <div className="map-container">
                                <div className="map-wrapper">
                                    <div className="map-overlay">
                                        <div className="map-legend">
                                            <div className="map-legend-icon"></div>
                                            <span>Store Location</span>
                                        </div>
                                    </div>
                                    <DisplayMap address={"819 Hương Lộ 2, Phường Bình Trị Đông A, Quận Bình Tân, TP. HCM Ho Chi Minh"} address2={address} />
                                </div>
                            </div>
                        </div>
                    </div>
                    <button className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
                        Place Order
                    </button>
                </div>
                <div className="p-4 bg-gray-50 shadow-md rounded">
                    <h1 className="text-2xl font-bold mb-4">ORDER SUMMARY</h1>
                    {cartItemsStatus === "failed" && (
                        <div className="text-red-500">Error {cartItemsError}</div>
                    )}
                    {cartItemsStatus === "loading" && (

                        <div className="text-blue-500 flex justify-center items-center h-full">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}

                    <div className="space-y-4">
                        {cart.map((cartItem) => (
                            <ProductInformationCard key={cartItem.itemId} cartItem={cartItem} />
                        ))}
                    </div>
                    <div className="mt-4 text-lg font-semibold flex justify-end">
                        <p className="mr-1 font-bold text-3xl">Total: ${cart.reduce((acc, item) => acc + item.quantity * item.unitPrice, 0)}</p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CheckOutPage;
