import { useEffect, useState } from "react";
import Product from "./Product";
import ReactPaginate from 'react-paginate';
import Slider from '@mui/material/Slider';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { fetchPaginatedProducts } from "../../api/productAxios";

const ProductPage = () => {
    const [products, setProducts] = useState({});
    const [category, setCategory] = useState(null);
    const [sort, setSort] = useState("name_asc");
    const [value, setValue] = useState([0, 500]);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [priceRange, setPriceRange] = useState([0, 500]);
    const [isLoading, setIsLoading] = useState(false);

    const handlePageClick = (data) => {
        let selectedPage = data.selected + 1;
        setCurrentPage(selectedPage);
    }

    const handleChange = (event, newValue) => {
        console.log("handleChange: " + newValue);
        setPriceRange(newValue);
    };

    const updateProducts = () => {
        setValue(priceRange);
    };

    function valuetext(value) {
        return `$${value[0]} - $${value[1]}`;
    }

    useEffect(() => {
        setIsLoading(true);
        const getProducts = async () => {
            try {
                console.log("data: " + value[0] + " " + value[1]);
                const data = await fetchPaginatedProducts({
                    start: value[0] || 0,
                    end: value[1] || null,
                    searchQuery: "",
                    sortBy: sort || "name_asc",
                    status: true,
                    supplierId: "",
                    categoryId: category,
                    pageIndex: currentPage,
                    pageSize: 12
                });
                setProducts(data);
                if (data && data.totalPages) {
                    setTotalPage(data.totalPages);
                }
            } catch (error) {
                console.error("Error fetching products:", error);
            }
            setIsLoading(false);
        };

        getProducts();
    }, [value, sort, category, currentPage]);

    return (
        <div className="container mx-auto mt-7">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-5">
                <div className="col-span-1">
                    <div className="bg-gray-100 p-4 border-t-4 border-orange-600">
                        <p className="font-bold text-lg">PRODUCT PORTFOLIO</p>
                    </div>
                    <div className="my-4">
                        <ul>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => { setCategory(null); setSort("bestseller_asc") }}>Top Seller</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C001")}>Racket</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C005")}>Sports Bag</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C004")}>Clothes</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C003")}>Shoes</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C002")}>Tools</li>
                        </ul>
                    </div>
                    <div className="bg-gray-100 p-4 border-t-4 border-orange-600">
                        <p className="font-bold text-lg">Brands</p>
                    </div>
                    <div className="my-4">
                        <ul>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => { setCategory(null); setSort("bestseller_asc") }}>Li-ning</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C001")}>Racket</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C005")}>Sports Bag</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C004")}>Clothes</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C003")}>Shoes</li>
                            <li className="mb-2 hover:text-red-500 cursor-pointer" onClick={() => setCategory("C002")}>Tools</li>
                        </ul>
                    </div>
                    <div className="bg-gray-100 p-4 border-t-4 border-orange-600">
                        <p className="font-bold text-lg">Price</p>
                    </div>
                    <div className="my-4">
                        <p className="text-gray-700 mb-2">Price Range: ${priceRange[0]} - ${priceRange[1]}</p>
                        <Slider
                            getAriaLabel={() => 'Price range'}
                            value={priceRange}
                            onChange={handleChange}
                            valueLabelDisplay="auto"
                            getAriaValueText={valuetext}
                            min={0}
                            max={500}
                        />
                        <button
                            className="mt-4 px-4 py-2 bg-blue-500 text-white rounded-lg"
                            onClick={updateProducts}
                        >
                            Search Products
                        </button>
                    </div>
                </div>

                <div className="col-span-3">
                    <div className="bg-gray-300 p-4 flex justify-end items-center">
                        <label className="mr-2">Sort by:</label>
                        <select
                            className="border border-gray-300 rounded-lg p-1"
                            onChange={(e) => setSort(e.target.value)}
                        >
                            <option value="name_asc">Name (A-Z)</option>
                            <option value="name_desc">Name (Z-A)</option>
                            <option value="price_desc">Price (Low to High)</option>
                            <option value="price_asc">Price (High to Low)</option>
                        </select>
                    </div>
                    <div className="mt-4">
                        {isLoading ? (
                            <div className="flex justify-center items-center py-8">
                                <FontAwesomeIcon icon={faSpinner} spin className="text-blue-500 text-3xl" />
                            </div>
                        ) : (
                            <div>
                                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                                    {products && products.items && products.items.length > 0 && products.items.map((product) => (
                                        <div key={product.productId} className="border border-gray-300 p-2 rounded-lg">
                                            <Product product={product} />
                                        </div>
                                    ))}
                                </div>
                                <div className="flex justify-center mt-8">
                                    <ReactPaginate
                                        breakLabel="..."
                                        nextLabel="Next >"
                                        onPageChange={handlePageClick}
                                        pageRangeDisplayed={3}
                                        pageCount={totalPage}
                                        previousLabel="< Previous"
                                        renderOnZeroPageCount={null}
                                        containerClassName="flex items-center space-x-1"
                                        pageClassName="flex items-center"
                                        pageLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                                        previousLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                                        nextLinkClassName="px-3 py-2 border border-gray-300 text-gray-700 hover:bg-gray-200"
                                        breakLinkClassName="px-3 py-2 border border-gray-300 text-gray-700"
                                        activeClassName="bg-blue-500 text-white"
                                        activeLinkClassName="px-3 py-2 border border-gray-300 bg-blue-500 text-white hover:bg-blue-600"
                                    />
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProductPage;
