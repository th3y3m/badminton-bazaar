import { useParams } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux';
import { fetchSingleNews } from "../../redux/slice/newsSlice";
import { useEffect } from "react";
import { viewNews } from "../../api/newsAxios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

const NewsPage = () => {
    const { id: newsId } = useParams();
    const dispatch = useDispatch();

    const news = useSelector((state) => state.news.singleNews);
    const newsStatus = useSelector((state) => state.news.status);
    const newsError = useSelector((state) => state.news.error);

    useEffect(() => {
        dispatch(fetchSingleNews(newsId)).catch(error => {
            console.error("Error fetching news:", error);
        });

    }, [dispatch, newsId]);
    useEffect(() => {
        viewNews(newsId);
    }, []);

    return (
        <div className="container mx-auto p-6">
            <div className="grid grid-cols-3 gap-6">
                <div className="col-span-1">

                </div>
                <div className="col-span-2 bg-white p-6 shadow-md rounded-lg">
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
                            <h1 className="text-3xl font-bold mb-4">{news.title}</h1>
                            <div className="flex items-center mb-4 text-gray-600">
                                <p className="mr-4">{new Date(news.publicationDate).toLocaleDateString()}</p>
                                <p>{news.views} views</p>
                            </div>
                            <img src={news.image} alt={news.title} className="w-full h-auto mb-4 rounded-lg shadow-lg" />
                            <p className="text-lg leading-relaxed">{news.content}</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default NewsPage;
