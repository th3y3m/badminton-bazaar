const Product = ({ product }) => {
    return (
        <div className="flex flex-col items-center justify-center w-64 h-96 bg-white rounded-lg shadow-md">
            {/* <img src={product.imageUrl} alt={product.productName} className="w-32 h-32" />
            <div className="flex flex-col items-center justify-center w-48 h-24">
                <h1 className="text-lg font-bold">{product.productName}</h1>
                <p className="text-sm font-light">{product.basePrice}</p>
            </div> */}
            <img src={product.imageUrl} alt={product.productName} className="w-full" />
            <h3 className="text-lg font-semibold mt-2">{product.productName}</h3>
            <p className="text-red-600 font-bold text-2xl">{product.basePrice} $</p>
        </div>
    );
}

export default Product;