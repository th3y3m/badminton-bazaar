import React, { useEffect, useState } from "react";
import { Fade } from 'react-slideshow-image';
import 'react-slideshow-image/dist/styles.css';
import { fetchPaginatedNews } from "../../../api/newsAxios";

const SlideImage = () => {
    const [slideImage, setSlideImage] = useState([]);

    
    useEffect(() => {
        const getSlideImage = async () => {
            try {
                const data = await fetchPaginatedNews({
                    status: true,
                    isHomePageSlideShow: true,
                    isHomePageBanner: false,
                    pageIndex: 1,
                    pageSize: 10
                });
                setSlideImage(data.items);
            } catch (error) {
                console.error("Error fetching slide images:", error);
            }
        };
        getSlideImage();
    }, []);

    return (
        <div className="slide-container">
            <Fade duration={5000} transitionDuration={500}>
                {slideImage.map((fadeImage, index) => (
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
