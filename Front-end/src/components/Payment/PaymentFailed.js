import React from 'react';
import { Link } from "react-router-dom";
import noImg from "../../assets/red_cross.png";

function PaymentFailed() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 shadow-lg rounded-md text-center">
        <img src={noImg} alt="Payment Failed" className="mx-auto mb-4 w-24 h-24" />
        <h2 className="text-2xl font-bold text-red-600 mb-2">OOPS !!!</h2>
        <p className="text-lg leading-relaxed mb-6">
          Something went wrong! Please try again on your orders or contact with Us:
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

export default PaymentFailed;
