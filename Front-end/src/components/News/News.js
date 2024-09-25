import { useNavigate } from "react-router-dom";

const News = ({ news }) => {
    const navigate = useNavigate();
    return (
        <div
            key={news.id}
            className="flex flex-col md:flex-row items-center space-y-4 md:space-y-0 md:space-x-4 bg-white shadow-2xl rounded-lg p-4 h-full"
            onClick={() => navigate(`/news-details/${news.newId}`)}
        >
            <img
                src={news.image}
                alt={news.title}
                className="h-40 w-full md:w-auto object-cover rounded-lg"
            />
            <div className="flex flex-col justify-center text-center md:text-left">
                <h3 className="text-lg md:text-xl font-semibold">{news.title}</h3>
                <p className="text-gray-600 text-sm md:text-base">{news.description}</p>
            </div>
        </div>
    );
}

export default News;