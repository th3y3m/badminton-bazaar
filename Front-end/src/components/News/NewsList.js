import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { fetchNews, fetchTopView } from "../../redux/slice/newsSlice";
import ReactPaginate from "react-paginate";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const NewsList = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const topView = useSelector((state) => state.news.topView);
    const topViewStatus = useSelector((state) => state.news.status);
    const topViewError = useSelector((state) => state.news.error);

    const news = useSelector((state) => state.news.news);
    const newsStatus = useSelector((state) => state.news.status);
    const newsError = useSelector((state) => state.news.error);

    const [totalPage, setTotalPage] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    const handlePageClick = (data) => {
        const selectedPage = data.selected + 1;
        setCurrentPage(selectedPage);
    };

    useEffect(() => {
        if (news && news.totalPages) {
            setTotalPage(news.totalPages);
        }
    }, [news]);

    useEffect(() => {
        dispatch(fetchNews({
            status: true,
            isHomePageSlideShow: false,
            isHomePageBanner: false,
            sortBy: 'publicationdate_desc',
            pageIndex: currentPage,
            pageSize: 6
        })).catch(error => {
            console.error("Error fetching news:", error);
        });
    }, [dispatch, currentPage]);

    useEffect(() => {
        dispatch(fetchTopView({
            status: true,
            isHomePageSlideShow: false,
            isHomePageBanner: false,
            sortBy: 'views_desc',
            pageIndex: 1,
            pageSize: 3
        })).catch(error => {
            console.error("Error fetching top view news:", error);
        });
    }, [dispatch]);

    return (
        <div className="mx-auto mt-5 container">
            <h1 className="text-4xl font-bold text-center text-blue-600 mb-8">NEWS</h1>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="col-span-1">
                    <h3 className="bg-slate-400 border-t-2 border-t-red-400 pl-2 text-lg font-semibold">MOST VIEWS</h3>
                    <div className="mt-6">
                        {topViewStatus === 'failed' && (
                            <div className="text-red-500">Error: {topViewError}</div>
                        )}
                        {topViewStatus === 'loading' && (
                            <div className="text-blue-500">
                                <FontAwesomeIcon icon={faSpinner} spin />
                            </div>
                        )}
                        {topViewStatus === 'succeeded' && (
                            <div className="space-y-4">
                                {topView && topView.map((news) => (
                                    <div key={"topView" + news.newId} className="flex items-center space-x-4 bg-white shadow-lg rounded-lg p-4 transition transform hover:-translate-y-1 hover:shadow-xl cursor-pointer"
                                        onClick={() => navigate(`/news-details/${news.newId}`)}>
                                        <img src={news.image} alt={news.title} className="h-20 w-20 object-cover rounded-lg" />
                                        <div className="flex flex-col justify-center">
                                            <h3 className="text-lg font-semibold text-gray-800">{news.title}</h3>
                                            <p className="text-gray-600 text-sm">{news.description}</p>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
                <div className="col-span-2">
                    <div className="mt-6">
                        {newsStatus === 'failed' && (
                            <div className="text-red-500">Error: {newsError}</div>
                        )}
                        {newsStatus === 'loading' && (
                            <div className="text-blue-500">
                                <FontAwesomeIcon icon={faSpinner} spin />
                            </div>
                        )}
                        {newsStatus === 'succeeded' && (
                            <div>
                                <div className="space-y-4">
                                    {news && news.items.map((newsItem) => (
                                        <div key={"news" + newsItem.newId} className="flex items-center space-x-4 bg-white shadow-lg rounded-lg p-4 transition transform hover:-translate-y-1 hover:shadow-xl cursor-pointer"
                                            onClick={() => navigate(`/news-details/${newsItem.newId}`)}>
                                            <img src={newsItem.image} alt={newsItem.title} className="h-40 w-40 object-cover rounded-lg" />
                                            <div className="flex flex-col justify-center">
                                                <h3 className="text-lg font-semibold text-gray-800">{newsItem.title}</h3>
                                                <p className="text-gray-600 text-sm">{newsItem.description}</p>
                                            </div>
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
                                        pageClassName="page-item"
                                        pageLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                                        previousClassName="page-item"
                                        previousLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                                        nextClassName="page-item"
                                        nextLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                                        breakClassName="page-item"
                                        breakLinkClassName="page-link inline-block py-2 px-3 leading-tight bg-white border border-gray-300 text-blue-700 hover:bg-gray-200"
                                        containerClassName="pagination flex list-none space-x-2"
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

export default NewsList;
