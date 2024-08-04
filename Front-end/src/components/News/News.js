import { Navigate } from "react-router-dom";

const News = ({ news }) => {
    return (
        <div key={news.id} className="flex items-center space-x-4 bg-white shadow-2xl rounded-lg p-4 h-full"
            onClick={() => Navigate(`/news-details/${news.newId}`)}> {/* Adjusted height */}
            <img src={news.image} alt={news.title} className="h-40 w-auto object-cover rounded-lg" /> {/* Adjusted size */}
            <div className="flex flex-col justify-center">
                <h3 className="text-lg font-semibold">{news.title}</h3>
                <p className="text-gray-600 text-sm">{news.description}</p>
            </div>
        </div>
    );
}

export default News;
