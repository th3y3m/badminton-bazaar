import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import ProductInformationCard from "./ProductInformationCard";
import { fetchCart } from "../../redux/slice/cartSlice";
import DisplayMap from "../map/DisplayMap";

const CheckOutPage = () => {
    const dispatch = useDispatch();

    const user = useSelector((state) => state.auth.token);
    const cart = useSelector((state) => state.cart.cart);
    const cartItemsStatus = useSelector((state) => state.cart.status);
    const cartItemsError = useSelector((state) => state.cart.error);

    const userDetail = useSelector((state) => state.userDetails.userDetail);
    const account = useSelector((state) => state.user.userDetail);

    const [address, setAddress] = useState(userDetail.address || "");

    useEffect(() => {
        if (user?.id) {
            console.log("Fetching cart for user:", user.id);
            dispatch(fetchCart(user.id)).catch(error => {
                console.error("Error fetching cart:", error);
            });
        }
    }, [dispatch, user?.id]);

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
                        <input
                            type="text"
                            id="address"
                            value={address}
                            onChange={e => setAddress(e.target.value)}
                            className="mt-2 p-2 border border-gray-300 rounded w-full"
                        />
                    </div>
                    <div>
                        {/* <DisplayMap address={"819 Hương Lộ 2, Phường Bình Trị Đông A, Quận Bình Tân, TP. HCM Ho Chi Minh"} /> */}
                    </div>
                    <button className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
                        Place Order
                    </button>
                </div>
                <div className="p-4 bg-gray-50 shadow-md rounded">
                    <h1 className="text-2xl font-bold mb-4">ORDER SUMMARY</h1>
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
}

export default CheckOutPage;
