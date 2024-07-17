import { useEffect, useState } from "react";
import { fetchPaginatedProducts } from "../../api/productAxios";

const ProductPage = () => {
    const [products, setProducts] = useState([]);
    const [category, setCategory] = useState(null);
    const [sort, setSort] = useState(null);



    useEffect(() => {
        const getProduct = async () => {
            try {
                const data = await fetchPaginatedProducts({
                    start: null,
                    end: null,
                    searchQuery: "",
                    sortBy: "name_asc",
                    status: true,
                    supplierId: "",
                    categoryId: category,
                    pageIndex: 1,
                    pageSize: 10
                });
                // Ensure data.items is always an array
                setProducts(data.items || []);
            } catch (error) {
                console.error("Error fetching products:", error);
                setProducts([]); // Set products to an empty array in case of error
            }
        };
        getProduct();
    }, [category, sort]);

    return (
        <div className="container mx-auto">
            <div className="grid grid-cols-4 mt-7">
                <div className="col-span-1">
                    <div>
                        <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'><bold>RODUCT PORTFOLIO</bold></p>
                    </div>
                    <div>
                        <ul>
                            <li className="mb-2 hover:text-red-500" onClick={() => { setCategory(null); setSort("bestseller_asc") }}>Sản phẩm nổi bật</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C001")}>Racket</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C005")}>Sports Bag</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C004")}>Clothes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C003")}>Shoes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C002")}>Tools</li>
                            {/* Add more categories as needed */}
                        </ul>
                    </div>
                </div>

                <div className="col-span-3">
                    <div className="flex justify-between items-center">
                        <div className="bg-[#b4b3b3] flex justify-end">
                            <div>
                                <label>Sort by:</label>
                                <select
                                    className="border border-gray-300 rounded-lg p-1 ml-2"
                                    onChange={(e) => setSort(e.target.value)}
                                >
                                    <option value="name_asc">Name (A-Z)</option>
                                    <option value="name_desc">Name (Z-A)</option>
                                    <option value="price_asc">Price (Low to High)</option>
                                    <option value="price_desc">Price (High to Low)</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div className="grid grid-cols-4 gap-4 mt-4">
                        {products.length > 0 && products.map((product) => (
                            <div key={product.productId} className="border border-gray-300 p-2">
                                <img
                                    src={product.productImageUrl}
                                    alt={product.productName}
                                    className="w-full h-48 object-cover"
                                />
                                <p className="text-center font-semibold">{product.productName}</p>
                                <p className="text-center font-semibold">{product.price}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}


export default ProductPage;