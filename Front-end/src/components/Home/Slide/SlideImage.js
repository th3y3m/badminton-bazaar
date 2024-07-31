import React, { useEffect, useState } from "react";
import { Fade } from 'react-slideshow-image';
import 'react-slideshow-image/dist/styles.css';
import { fetchPaginatedNews } from "../../../api/newsAxios";
import { useDispatch, useSelector } from 'react-redux';
import { fetchSlideNews } from "../../../redux/slice/newsSlice";


const SlideImage = () => {
    const [slideNews, setSlideNews] = useState([]);

    // const dispatch = useDispatch();
    // const slideNews = useSelector((state) => state.news.newsSlide);
    // const slideNewsStatus = useSelector((state) => state.news.status);
    // const slideNewsError = useSelector((state) => state.news.error);
    // console.log(slideNews);
    
    useEffect(() => {
        // dispatch(fetchSlideNews({
        //     status: true,
        //     isHomePageSlideShow: true,
        //     isHomePageBanner: false,
        //     pageIndex: 1,
        //     pageSize: 10
        // }));

        const getSlideImage = async () => {
            try {
                const data = await fetchPaginatedNews({
                    status: true,
                    isHomePageSlideShow: true,
                    isHomePageBanner: false,
                    pageIndex: 1,
                    pageSize: 10
                });
                setSlideNews(data.items);
            } catch (error) {
                console.error("Error fetching slide images:", error);
            }
        };
        getSlideImage();
    }, []);


    // if (slideNewsStatus === 'failed') {
    //     return <div>Error: {slideNewsError}</div>
        
    // }

    // if (slideNewsStatus === 'loading') {
    //     return <div>Loading...</div>
    // }

    return (
        <div className="slide-container">
            <Fade duration={5000} transitionDuration={500}>
                {slideNews.map((fadeImage, index) => (
                    <div key={index}>
                        <img
                            style={{ width: '100%', height: '700px', objectFit: 'cover' }}
                            src={fadeImage.image}
                            alt={fadeImage.title}
                        />
                    </div>
                ))}
            </Fade>
        </div>
    );
};

export default SlideImage;
