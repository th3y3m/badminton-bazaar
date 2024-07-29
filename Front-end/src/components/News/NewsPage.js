import { useParams } from "react-router-dom";

const NewsPage = () => {
    const { id: newsId } = useParams();
    return (
        <div>
            <h1>News Page</h1>
        </div>
    );
}

export default NewsPage;