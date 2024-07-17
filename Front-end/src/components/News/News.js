const News = ({ news }) => {
    return (
        <div className='news w-full'> {/* Adjust this class if the news container should also be wider */}
            <div key={news.id} className="flex items-center space-x-4 bg-white rounded-lg shadow-md p-4 w-96"> {/* Adjust width here */}
                <img src={news.image} alt={news.title} className="w-48 h-auto flex-none rounded-lg" />
                <div className="flex flex-col justify-center">
                    <h3 className="text-lg font-semibold">{news.title}</h3>
                    <p className="text-gray-600">{news.description}</p>
                </div>
            </div>
        </div>
    );
}

export default News;