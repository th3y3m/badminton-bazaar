import React from 'react';
import { Link } from "react-router-dom";

function ErrorPage() {
    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100">
            <div className="bg-white p-8 shadow-lg rounded-md text-center">
                <h2 className="text-3xl font-bold text-red-600 mb-4">404 - Page Not Found</h2>
                <p className="text-lg mb-6">Oops! The page you are looking for does not exist.</p>
                <Link to="/">
                    <button className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 transition duration-300">
                        Back to Home
                    </button>
                </Link>
            </div>
        </div>
    );
}

export default ErrorPage;
