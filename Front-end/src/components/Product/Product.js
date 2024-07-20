import { useNavigate } from "react-router-dom";


const Product = ({ product }) => {
const navigate = useNavigate();

    return (
        <div className="flex flex-col items-center justify-center w-64 h-96 bg-white rounded-lg shadow-md">
            {/* <img src={product.imageUrl} alt={product.productName} className="w-32 h-32" />
            <div className="flex flex-col items-center justify-center w-48 h-24">
                <h1 className="text-lg font-bold">{product.productName}</h1>
                <p className="text-sm font-light">{product.basePrice}</p>
            </div> */}
            <button onClick={() => navigate(`/product-details/${product.productId}`)}>
            <img src={product.imageUrl} alt={product.productName} className="w-full" />
            <h3 className="text-lg font-semibold mt-2">{product.productName}</h3>
            <p className="text-red-600 font-bold text-2xl">{product.basePrice} $</p>

            </button>
        </div>
    );
}

export default Product;