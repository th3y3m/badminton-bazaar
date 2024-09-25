import React, { useEffect, useState } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import SlideImage from './Slide/SlideImage';
import { numOfProductRemaining as fetchProductRemaining } from '../../api/productAxios';
import Product from '../Product/Product';
import News from '../News/News';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { fetchBannerNews, fetchNews } from '../../redux/slice/newsSlice';
import { fetchRackets, fetchTopSeller } from '../../redux/slice/productSlice';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const HomePage = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const topSellerList = useSelector((state) => state.product.topSellers);
    const topSellerListStatus = useSelector((state) => state.product.status);
    const topSellerListError = useSelector((state) => state.product.error);

    const bannerList = useSelector((state) => state.news.banners);
    const bannerListStatus = useSelector((state) => state.news.status);
    const bannerListError = useSelector((state) => state.news.error);

    const racketList = useSelector((state) => state.product.rackets);
    const racketListStatus = useSelector((state) => state.product.status);
    const racketListError = useSelector((state) => state.product.error);

    const newsList = useSelector((state) => state.news.news);
    const newsListStatus = useSelector((state) => state.news.status);
    const newsListError = useSelector((state) => state.news.error);

    const [productRemaining, setProductRemaining] = useState({});
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7173/productHub")
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        console.log('Connecting to SignalR...');
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected to SignalR!');

                    connection.on('ReceiveProductStockUpdate', (productId, newStockQuantity) => {
                        console.log('Stock update received:', productId, newStockQuantity);
                        setProductRemaining(prevState => {
                            const updatedState = {
                                ...prevState,
                                [productId]: newStockQuantity
                            };
                            console.log('Updated productRemaining:', updatedState);
                            return updatedState;
                        });
                    });

                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);


    useEffect(() => {
        const getTopSellerProduct = async () => {
            try {
                const remainingCounts = await Promise.all(
                    topSellerList.map(async (product) => {
                        const count = await fetchProductRemaining(product.productId);
                        return { productId: product.productId, count };
                    })
                );

                const remainingCountMap = remainingCounts.reduce((acc, { productId, count }) => {
                    acc[productId] = count;
                    return acc;
                }, {});

                setProductRemaining(remainingCountMap);
            } catch (error) {
                console.error("Error fetching top seller:", error);
            }
        };

        if (topSellerList.length > 0) {
            getTopSellerProduct();
        }
    }, [topSellerList]);

    useEffect(() => {
        dispatch(fetchBannerNews({
            status: true,
            isHomePageSlideShow: false,
            isHomePageBanner: true,
            pageIndex: 1,
            pageSize: 10
        }));
        dispatch(fetchNews({
            status: true,
            isHomePageSlideShow: false,
            isHomePageBanner: false,
            pageIndex: 1,
            pageSize: 10
        }));
        dispatch(fetchRackets({
            sortBy: "name_asc",
            status: true,
            categoryId: "C001",
            pageIndex: 1,
            pageSize: 10
        }));
        dispatch(fetchTopSeller(10));
    }, [dispatch]);

    return (
        <div className="bg-gray-100">
            <SlideImage />
            <div className="container mx-auto p-4">
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
                    {/* Left Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg transform transition duration-300 hover:scale-105">
                            <img src="https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/bracket.png?alt=media&token=190b7139-0589-4851-9f14-f176d61dea8f" alt="Cầu lông" className="w-full" />
                        </div>
                    </div>

                    {/* Middle Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg transform transition duration-300 hover:scale-105">
                            <h2 className="text-xl font-bold mb-4 text-center text-red-600">HOT DEALS</h2>

                            {topSellerListStatus === 'failed' && (
                                <div className="text-red-500">Error: {topSellerListError}</div>
                            )}

                            {topSellerListStatus === 'loading' && (
                                <div className="text-blue-500">
                                    <FontAwesomeIcon icon={faSpinner} spin />
                                </div>
                            )}

                            {topSellerList.length > 0 && (
                                <div key={topSellerList[0].productId} className="mb-4 cursor-pointer" onClick={() => navigate(`/product-details/${topSellerList[0].productId}`)}>
                                    <img src={topSellerList[0].imageUrl} alt={topSellerList[0].productName} className="w-full rounded-lg" />
                                    <h3 className="text-lg font-semibold mt-2">{topSellerList[0].productName}</h3>
                                    <p className="text-red-600 font-bold text-2xl">{topSellerList[0].basePrice} $</p>
                                    <p className="text-gray-600">Remain {productRemaining[topSellerList[0].productId] || 0}</p>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Right Column */}
                    <div className="col-span-1">
                        <div className="bg-white p-4 shadow-md rounded-lg transform transition duration-300 hover:scale-105">
                            <h2 className="text-xl font-bold mb-4 text-center text-red-600">TOP SELLERS</h2>
                            <ul>
                                {topSellerListStatus === 'failed' && (
                                    <div className="text-red-500">Error: {topSellerListError}</div>
                                )}

                                {topSellerListStatus === 'loading' && (
                                    <div className="text-blue-500">
                                        <FontAwesomeIcon icon={faSpinner} spin />
                                    </div>
                                )}
                                {topSellerList.length > 0 && topSellerList.slice(1, 4).map((product) => (
                                    <li key={product.productId} className="flex items-center justify-between border-b border-gray-200 py-2 cursor-pointer hover:bg-gray-100" onClick={() => navigate(`/product-details/${product.productId}`)}>
                                        <div className="flex items-center">
                                            <img src={product.imageUrl} alt={product.productName} className="w-16 rounded-lg" />
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

                <div className='my-6 flex justify-center'>
                    {bannerListStatus === 'failed' && (
                        <div className="text-red-500">Error: {bannerListError}</div>
                    )}

                    {bannerListStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {bannerList.length > 0 && <img src={bannerList[0].image} alt="banner1" className="w-full rounded-lg shadow-lg" />}
                </div>

                <div className='topsell my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05] text-center font-bold text-2xl py-2'>BEST SELLER</p>
                    {topSellerListStatus === 'failed' && (
                        <div className="text-red-500">Error: {topSellerListError}</div>
                    )}

                    {topSellerListStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {topSellerList.length > 0 && (
                        <div className='grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4'>
                            {topSellerList.slice(0, 11).map((product) => (
                                <div key={product.productId} className="col-span-1 flex flex-col items-center justify-center bg-white rounded-lg shadow-md p-4 transform transition duration-300 hover:scale-105">
                                    <Product product={product} />
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                <div className='racket my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05] text-center font-bold text-2xl py-2'>RACKETS</p>
                    {racketListStatus === 'failed' && (
                        <div className="text-red-500">Error: {racketListError}</div>
                    )}

                    {racketListStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {racketList && (
                        <div className='grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4'>
                            {racketList.slice(0, 11).map((product) => (
                                <div key={product.productId} className="col-span-1 flex flex-col items-center justify-center bg-white rounded-lg shadow-md p-4 transform transition duration-300 hover:scale-105">
                                    <Product product={product} />
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                <div className='my-6 flex justify-center'>
                    {bannerListStatus === 'failed' && (
                        <div className="text-red-500">Error: {bannerListError}</div>
                    )}

                    {bannerListStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {bannerList.length > 0 && <img src={bannerList[1].image} alt="banner2" className="w-full rounded-lg shadow-lg" />}
                </div>

                <div className='news my-6'>
                    <p className='bg-[#f5f5f5] border-t-2 border-t-[#ed3b05] text-center font-bold text-2xl py-2'>News</p>
                    {newsListStatus === 'failed' && (
                        <div className="text-red-500">Error: {newsListError}</div>
                    )}

                    {newsListStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {newsList && newsList.items && newsList.items.length > 0 && (
                        <div className='grid grid-cols-1 sm:grid-cols-2 gap-4'>
                            {newsList.items.slice(0, 4).map((news) => (
                                <div key={news.newId} className='col-span-1 rounded-lg p-4 transform transition duration-300 hover:scale-105'>
                                    <News news={news} />
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default HomePage;
