import { memo } from "react"
import Header from "./Header/Header";
import Footer from "./Footer/Footer";


const masterLayout = ({ children, ...props }) => {
    return (
        <div {...props}>
            <Header />
            {children}
            <Footer />
        </div>
    );
};

export default memo(masterLayout);