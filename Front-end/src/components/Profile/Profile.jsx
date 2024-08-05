import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchUserDetail, modifyUserDetail } from "../../redux/slice/userDetailSlice";
import { toast } from "react-toastify";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const Profile = () => {
    const dispatch = useDispatch();
    const user = useSelector((state) => state.auth.token);

    const userDetails = useSelector((state) => state.userDetails.userDetail);
    const userDetailsStatus = useSelector((state) => state.userDetails.status);
    const userDetailsError = useSelector((state) => state.userDetails.error);

    const account = useSelector((state) => state.user.userDetail);
    const accountStatus = useSelector((state) => state.user.status);
    const accountError = useSelector((state) => state.user.error);

    const [fullName, setFullName] = useState(userDetails.fullName || "");
    const [address, setAddress] = useState(userDetails.address || "");
    const [profilePicture, setProfilePicture] = useState(null);

    useEffect(() => {
        if (userDetails.fullName) setFullName(userDetails.fullName);
        if (userDetails.address) setAddress(userDetails.address);
    }, [userDetails]);

    const handleUpdate = async () => {
        const formData = {
            FullName: fullName || userDetails.fullName,
            Address: address || "",
            ImageUrl: profilePicture || userDetails.profilePicture
        };
        try {
            await dispatch(modifyUserDetail({ UserDetailModel: formData, id: account.id }));
            dispatch(fetchUserDetail(user.id));
        } catch (error) {
            console.error("Failed to update user details", error);
        }
    };

    const handleUpdateImage = async (e) => {
        const file = e.target.files[0];
        setProfilePicture(file);
        const formData = {
            FullName: fullName || userDetails.fullName,
            Address: address || "",
            ImageUrl: file || userDetails.profilePicture
        };
        console.log(formData);
        try {
            await dispatch(modifyUserDetail({ UserDetailModel: formData, id: account.id }));
            dispatch(fetchUserDetail(user.id));
            toast.success('Profile picture updated successfully');
        } catch (error) {
            console.error("Failed to update user details", error);
            toast.error('Failed to update profile picture');
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-center text-3xl font-bold mb-6">Profile</h1>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="col-span-1 flex flex-col items-center">
                    <label htmlFor="profilePicture" className="block text-lg font-semibold mb-2">
                        <img src={userDetails.profilePicture || "default_profile_picture_url"} alt="Avatar" className="w-32 h-32 rounded-full mb-4" />
                    </label>
                    <input
                        id="profilePicture"
                        type="file"
                        className="hidden"
                        onChange={handleUpdateImage}
                    />
                    <p className="text-lg font-semibold">Email: <span className="font-normal">{account.email}</span></p>
                    <p className="text-lg font-semibold">Balance: <span className="font-normal">$ {userDetails.balance}</span></p>
                    <p className="text-lg font-semibold">Bonus Point: <span className="font-normal">{userDetails.point}</span></p>
                </div>
                <div className="col-span-2">
                    <h2 className="text-2xl font-semibold mb-4">User Information</h2>
                    <div className="mb-4">
                        <label htmlFor="fullName" className="block text-lg font-semibold mb-2">Full Name</label>
                        <input
                            id="fullName"
                            type="text"
                            value={fullName}
                            onChange={(e) => setFullName(e.target.value)}
                            className="w-full p-2 border border-gray-300 rounded"
                        />
                    </div>
                    <div className="mb-4">
                        <label htmlFor="address" className="block text-lg font-semibold mb-2">Address</label>
                        <input
                            id="address"
                            type="text"
                            value={address}
                            onChange={(e) => setAddress(e.target.value)}
                            className="w-full p-2 border border-gray-300 rounded"
                        />
                    </div>
                </div>
            </div>
            <div className="text-center">
                {userDetailsStatus === 'failed' && (
                    <div className="text-red-500">Error: {userDetailsError}</div>
                )}
                {userDetailsStatus === 'loading' && (
                    <div className="text-blue-500">
                        <FontAwesomeIcon icon={faSpinner} spin />
                    </div>
                )}
                <button
                    className="bg-blue-500 text-white text-lg font-semibold py-2 px-4 rounded mt-4"
                    onClick={handleUpdate}
                >
                    Update
                </button>
            </div>
        </div>
    );
};

export default Profile;
