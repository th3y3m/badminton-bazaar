import { useEffect, useState } from "react";
import Product from "./Product";
import ReactPaginate from 'react-paginate';
import Slider from '@mui/material/Slider';
import { useDispatch, useSelector } from 'react-redux';
import { fetchProducts } from "../../redux/slice/productSlice";

const ProductPage = () => {
    const dispatch = useDispatch();

    const products = useSelector((state) => state.product.products);
    const productsStatus = useSelector((state) => state.product.status);
    const productsError = useSelector((state) => state.product.error);

    const [category, setCategory] = useState(null);
    const [sort, setSort] = useState("name_asc");
    const [value, setValue] = useState([0, 500]);
    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [priceRange, setPriceRange] = useState([0, 500]);

    const handlePageClick = (data) => {
        let selectedPage = data.selected + 1;
        setCurrentPage(selectedPage);
    }

    const handleChange = (event, newValue) => {
        setPriceRange(newValue);
    };

    const updateProducts = () => {
        setValue(priceRange);
    };

    function valuetext(value) {
        return `$${value[0]} - $${value[1]}`;
    }

    useEffect(() => {
        dispatch(fetchProducts({
            start: value[0] || null,
            end: value[1] || null,
            searchQuery: "",
            sortBy: sort || "name_asc",
            status: true,
            supplierId: "",
            categoryId: category,
            pageIndex: currentPage,
            pageSize: 12
        }));
    }, [dispatch, value, sort, category, currentPage]);

    useEffect(() => {
        if (products && products.totalPages) {
            setTotalPage(products.totalPages);
        }
    }, [products]);

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
                            Update Products
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
                            <option value="price_asc">Price (Low to High)</option>
                            <option value="price_desc">Price (High to Low)</option>
                        </select>
                    </div>
                    <div className="mt-4">
                        {productsStatus === 'failed' && (
                            <div className="text-red-500">Error: {productsError}</div>
                        )}
                        {productsStatus === 'loading' && (
                            <div className="text-blue-500">Loading...</div>
                        )}
                        {productsStatus === 'succeeded' && (
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
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProductPage;
