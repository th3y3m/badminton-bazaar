import React, { useState } from "react";
import { useSelector } from "react-redux";

const CheckOutPage = () => {
    const cart = useSelector((state) => state.cart);
    const user = useSelector((state) => state.auth.user);
    const userDetail = useSelector((state) => state.userDetails.userDetail);
    const account = useSelector((state) => state.user.userDetail);
    console.log("checkout");
    const [address, setAddress] = useState(userDetail.address || "");

    return (
        <div className="container mx-auto">
            <div className="grid grid-cols-2">
                <div className="col-span-1">
                    <h1>ORDER DETAILS</h1>
                    <p className="mt-7">Delivery Details</p>
                    <div>
                        <p>Email: {account.email}</p>
                        <p>Full Name: {userDetail.fullName}</p>
                    </div>
                    <div>
                        <label htmlFor="address">Address</label>
                        <input type="text" id="address" value={address} onChange={e => setAddress(e.target.value)} />
                    </div>
                </div>
                <div className="col-span-1 bg-[#fafafa]"></div>
            </div>
        </div>
    );
}

export default CheckOutPage;