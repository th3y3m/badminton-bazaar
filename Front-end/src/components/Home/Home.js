import React, { useEffect, useState } from 'react';
import SlideImage from './Slide/SlideImage'; // Ensure this is your path to SlideImage component
import { getTopSeller, numOfProductRemaining as fetchProductRemaining, fetchPaginatedProducts } from '../../api/productAxios';
import { fetchPaginatedNews } from '../../api/newsAxios';
import Product from '../Product/Product';
import News from '../News/News';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';

const HomePage = () => {
    
    const dispatch = useDispatch();
    const productsList = useSelector((state) => state.product.products);
    const productsListStatus = useSelector((state) => state.product.status);
    const productsListError = useSelector((state) => state.product.error);




    const [topSellers, setTopSellers] = useState([]);
    const [banners, setBanners] = useState([]);
    const [rackets, setRackets] = useState([]);
    const [news, setNews] = useState([]);
    const [productRemaining, setProductRemaining] = useState({});
    const navigate = useNavigate();

    const getBanners = async () => {
        try {
            const data = await fetchPaginatedNews({
                status: true,
                isHomePageSlideShow: false,
                isHomePageBanner: true,
                pageIndex: 1,
                pageSize: 10
            });
            setBanners(data.items);
        } catch (error) {
            console.error("Error fetching banners:", error);
        }
    }

    const getNews = async () => {
        try {
            const data = await fetchPaginatedNews({
                status: true,
                isHomePageSlideShow: false,
                isHomePageBanner: false,
                pageIndex: 1,
                pageSize: 10
            });
            setNews(data.items);
        } catch (error) {
            console.error("Error fetching banners:", error);
        }
    }

    const getRackets = async () => {
        try {
            const data = await fetchPaginatedProducts({
                sortBy: "name_asc",
                status: true,
                categoryId: "C001",
                pageIndex: 1,
                pageSize: 10
            });
            setRackets(data.items);
        } catch (error) {
            console.error("Error fetching banners:", error);
        }
    }

    const getTopSellerProduct = async (n) => {
        try {
            const data = await getTopSeller(n);
            setTopSellers(data);

            // Fetch the remaining products count for each top seller product
            const remainingCounts = await Promise.all(
                data.map(async (product) => {
                    const count = await fetchProductRemaining(product.productId);
                    return { productId: product.productId, count };
                })
            );

            // Create a mapping of productId to remaining count
            const remainingCountMap = remainingCounts.reduce((acc, { productId, count }) => {
                acc[productId] = count;
                return acc;
            }, {});

            setProductRemaining(remainingCountMap);

        } catch (error) {
            console.error("Error fetching top seller:", error);
        }
    }

    useEffect(() => {
        getTopSellerProduct(10);
        getBanners();
        getRackets();
        getNews();
    }, []);

    return (
        <div>
            <SlideImage />
            <div className="container mx-auto p-4">
                <div className="grid grid-cols-3 gap-4">
                    {/* Left Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg">
                            <img src="https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/bracket.png?alt=media&token=190b7139-0589-4851-9f14-f176d61dea8f" alt="Cầu lông" className="w-full" />
                        </div>
                    </div>

                    {/* Middle Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg">
                            <h2 className="text-xl font-bold mb-4">HOT DEALS</h2>
                            {topSellers.length > 0 && (
                                <div key={topSellers[0].productId} className="mb-4 cursor-pointer" onClick={() => navigate(`/product-details/${topSellers[0].productId}`)}>
                                    <img src={topSellers[0].imageUrl} alt={topSellers[0].productName} className="w-full" />
                                    <h3 className="text-lg font-semibold mt-2">{topSellers[0].productName}</h3>
                                    <p className="text-red-600 font-bold text-2xl">{topSellers[0].basePrice} $</p>
                                    <p className="text-gray-600">Remain {productRemaining[topSellers[0].productId] || 0}</p>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Right Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg">
                            <h2 className="text-xl font-bold mb-4">TOP SELLERS</h2>
                            <ul>
                                {topSellers.length > 0 && topSellers.slice(1, 4).map((product) => (
                                    <li key={product.productId} className="flex items-center justify-between border-b border-gray-200 py-2 cursor-pointer" onClick={() => navigate(`/product-details/${topSellers[0].productId}`)}>
                                        <div className="flex items-center">
                                            <img src={product.imageUrl} alt={product.productName} className="w-16" />
                                            <div className="ml-4">
                                                <h3 className="text-lg font-semibold">{product.productName}</h3>
                                                <p className="text-red-600 font-bold text-2xl">{product.basePrice} $</p>
                                            </div>
                                        </div>
                                        <div>
                                            <p className="text-gray-600">Remain {productRemaining[product.productId] || 0}</p>
                                        </div>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    </div>
                </div>

                <div className='banner1 flex justify-center'>
                    {banners.length > 0 && <img src={banners[0].image} alt="banner1" />}
                </div>

                <div className='topsell my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'>BEST SELLER</p>
                    {topSellers.length > 0 && <div className='clothes grid grid-cols-5 gap-1'>
                        {topSellers.slice(0, 11).map((product) => (
                            <div key={product.productId} className="col-span-1 flex flex-col items-center justify-center w-64 h-96 bg-white rounded-lg shadow-md">
                                <Product product={product} />
                            </div>
                        ))}
                    </div>}
                </div>

                <div className='racket my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'>RACKETS</p>

                    {rackets.length > 0 && <div className='clothes grid grid-cols-5 gap-1'>
                        {rackets.slice(0, 11).map((product) => (
                            <div key={product.productId} className="col-span-1 flex flex-col items-center justify-center w-64 h-96 bg-white rounded-lg shadow-md">
                                <Product product={product} />
                            </div>
                        ))}
                    </div>}
                </div>

                <div className='banner2 flex justify-center'>
                    {banners.length > 0 && <img src={banners[1].image} alt="banner1" />}
                </div>
                <div className='news my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05]'>News</p>
                    {news.length > 0 && <div className='clothes grid grid-cols-5 gap-1'>
                        {news.slice(0, 5).map((news) => (
                            <div key={news.newId}>
                                <News news={news} />
                            </div>
                        ))}
                    </div>}
                </div>
            </div>
        </div>
    );
};

export default HomePage;
