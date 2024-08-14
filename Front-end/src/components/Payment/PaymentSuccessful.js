import React from 'react';
import { Link, useLocation } from "react-router-dom";
import { FaHeart } from "react-icons/fa";
import yesImg from "../../assets/green_tick.png";
import noImg from "../../assets/red_cross.png"; // Add this import

function PaymentSuccessful() {
  const query = new URLSearchParams(useLocation().search); // Parse the query string
  const vnp_TxnRef = query.get('vnp_TxnRef'); // Get the value of vnp_TxnRef

  if (vnp_TxnRef == null) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 shadow-lg rounded-md text-center">
          <img src={noImg} alt="Payment Failed" className="mx-auto mb-4 w-24 h-24" />
          <h2 className="text-2xl font-bold text-red-600 mb-2">OOPS !!!</h2>
          <p className="text-lg leading-relaxed mb-6">
            Something went wrong! Please try again on your orders or contact us:
            <a href="mailto:courtcallers@gmail.com" className="text-blue-500 underline ml-1">bazaarb43@gmail.com</a>
          </p>
          <Link to="/">
            <button className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 transition duration-300">
              Back to Home
            </button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 shadow-lg rounded-md text-center">
        <img src={yesImg} alt="Payment Successful" className="mx-auto mb-4 w-24 h-24" />
        <h2 className="text-2xl font-bold text-green-600 mb-2">Thank You!</h2>
        <p className="text-lg leading-relaxed mb-6">
          Your booking <strong>{vnp_TxnRef}</strong> has been successfully submitted. Thanks for choosing our service
          <FaHeart className="inline-block text-red-500 text-lg ml-1" />
        </p>
        <div className="flex justify-around">
          <Link to="/">
            <button className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 transition duration-300 mr-4">
              Back to Home
            </button>
          </Link>
          <Link to="/booked">
            <button className="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 transition duration-300">
              View Booking
            </button>
          </Link>
        </div>
      </div>
    </div>
  );
}

export default PaymentSuccessful;
