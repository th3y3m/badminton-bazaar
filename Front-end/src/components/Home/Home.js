import Modal from "react-modal";
import { memo, useState, useEffect } from "react";
import SlideShowHomePage from "./SlideShow/SlideShow";

Modal.setAppElement('#root');


const HomePage = () => {

    return (
        <>
            <div>
                <div>
                    <div className="slideshow-container">
                        {/* <SlideShowHomePage /> */}
                    </div>
                </div>
                <div className="container">
                    <div>

                    </div>
                </div>
            </div>
        </>
    );

};

export default memo(HomePage);