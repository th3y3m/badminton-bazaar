import { useEffect, useState } from "react";
import { fetchPaginatedProducts } from "../../api/productAxios";
import Product from "./Product";
import ReactPaginate from 'react-paginate';
import Slider from '@mui/material/Slider';


const ProductPage = () => {
    const [products, setProducts] = useState([]);
    const [category, setCategory] = useState(null);
    const [sort, setSort] = useState("name_asc");
    const [value, setValue] = useState([0, 500]);
    const [totalProduct, setTotalProduct] = useState(0);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1); // Added state for current page


    const handlePageClick = (data) => {
        let selectedPage = data.selected + 1; // ReactPaginate is zero-indexed
        setCurrentPage(selectedPage);
    }
    const handleChange = (event, newValue) => {
        setValue(newValue); // newValue is an array [minPrice, maxPrice]
    };
    function valuetext(value) {
        return `$${value[0]} - $${value[1]}`;
    }
    useEffect(() => {
        const getProduct = async () => {
            try {
                const data = await fetchPaginatedProducts({
                    start: value[0] || null,
                    end: value[1] || null,
                    searchQuery: "",
                    sortBy: sort || "name_asc",
                    status: true,
                    supplierId: "",
                    categoryId: category,
                    pageIndex: currentPage,
                    pageSize: 12
                });

                setProducts(data.items || []);
                setTotalProduct(data.totalCount || 0);
                setTotalPage(data.totalPages || 0);

            } catch (error) {
                console.error("Error fetching products:", error);
                setProducts([]);
            }
        };
        getProduct();
    }, [category, sort, currentPage, value]);

    return (
        <div className="container mx-auto">
            <div className="grid grid-cols-4 mt-7 gap-5">
                <div className="col-span-1">
                    <div>
                        <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'><bold>PRODUCT PORTFOLIO</bold></p>
                    </div>
                    <div className="my-4">
                        <ul>
                            <li className="mb-2 hover:text-red-500" onClick={() => { setCategory(null); setSort("bestseller_asc") }}>Top Seller</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C001")}>Racket</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C005")}>Sports Bag</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C004")}>Clothes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C003")}>Shoes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C002")}>Tools</li>
                            {/* Add more categories as needed */}
                        </ul>
                    </div>
                    <div>
                        <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'><bold>Brands</bold></p>
                    </div>
                    <div className="my-4">
                        <ul>
                            <li className="mb-2 hover:text-red-500" onClick={() => { setCategory(null); setSort("bestseller_asc") }}>Li-ning</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C001")}>Racket</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C005")}>Sports Bag</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C004")}>Clothes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C003")}>Shoes</li>
                            <li className="mb-2 hover:text-red-500" onClick={() => setCategory("C002")}>Tools</li>
                            {/* Add more categories as needed */}
                        </ul>
                    </div>
                    <div>
                        <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'><bold>Price</bold></p>
                    </div>
                    <div className="my-4">
                        <Slider
                            getAriaLabel={() => 'Price range'}
                            value={value} // This should be an array, e.g., [minPrice, maxPrice]
                            onChange={handleChange} // Update to handle range changes
                            valueLabelDisplay="auto"
                            getAriaValueText={valuetext} // Ensure this function can handle range values
                            min={0}
                            max={500}
                        />
                    </div>
                </div>

                <div className="col-span-3">
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
                    <div className="grid grid-cols-4 gap-4 mt-4">
                        {products.length > 0 && products.map((product) => (
                            <div key={product.productId} className="border border-gray-300 p-2">
                                <Product product={product} />
                            </div>
                        ))}
                    </div>

                    <div className="flex justify-center">
                        <ReactPaginate
                            breakLabel="..."
                            nextLabel="next >"
                            onPageChange={handlePageClick}
                            pageRangeDisplayed={3}
                            pageCount={totalPage}
                            previousLabel="< previous"

                            pageClassName="page-item"
                            pageLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                            previousClassName="page-item"
                            previousLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                            nextClassName="page-item"
                            nextLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                            breakClassName="page-item"
                            breakLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                            containerClassName="pagination flex list-none space-x-2"
                            activeClassName="active bg-blue-500 text-white"
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}


export default ProductPage;