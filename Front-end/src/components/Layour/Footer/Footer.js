import tick_image from "../../../assets/logo_tick.png";

const Footer = () => {

    return (
        <>
            <footer className="bg-[#2f2f2f] mt-20">
                <div className="mgx-[15px] container mx-auto">
                    <div className="flex justify-center">
                        <div className="w-[25%] mx-3 ">
                            <h3 className="text-white text-2xl font-bold">CONTACT</h3>
                            <div className="text-[#888888]">
                                <p><i className="fas fa-map-marker-alt"></i> 123, Đường ABC, Quận XYZ, TP.HCM</p>
                                <p><i className="fas fa-phone-alt"></i> 0123 456 789</p>
                                <p><i className="fas fa-envelope"></i>
                                    <a href="mailto:bazaarb43@gmail.com" className="text-white">bazaarb43@gmail.com</a>
                                </p>
                                <img src={tick_image} alt="tick" className="h-16" />
                            </div>
                        </div>
                        <div className="w-[25%] mx-3">
                            <h3 className="text-white text-2xl font-bold">SUPPORTS</h3>
                            <div className="text-[#888888]">
                                <p>
                                    Là kênh phân phối và bán lẻ chính hãng dụng cụ, phụ kiện và trang phục , giày dép môn thể thao cầu lông thương hiệu Li-Ning tại Việt NamĐịa chỉ :
                                    Địa chỉ: Số 3B Trịnh Hoài Đức,Phường Cát Linh, Quận Đống Đa, Hà Nội Mã số thuế : 8032237260 Ngày 01/06/2023 Tại Phòng Tài Chính -Kế Hoạch
                                </p>
                            </div>
                        </div>
                        <div className="w-[25%] mx-3">
                            <h3 className="text-white text-2xl font-bold">PRODUCT PORTFOLIO</h3>
                            <div className="text-[#888888]">
                                <ul>
                                    <li className="mb-2 hover:text-red-500">Top Seller</li>
                                    <li className="mb-2 hover:text-red-500">Racket</li>
                                    <li className="mb-2 hover:text-red-500">Sports Bag</li>
                                    <li className="mb-2 hover:text-red-500">Clothes</li>
                                    <li className="mb-2 hover:text-red-500">Shoes</li>
                                    <li className="mb-2 hover:text-red-500">Tools</li>
                                </ul>
                            </div>
                        </div>
                        <div className="w-[25%] mx-3">
                            <h3 className="text-white text-2xl font-bold">BRANDS</h3>
                            <div className="text-[#888888]">
                                <ul>
                                    <li className="mb-2 hover:text-red-500">Li-Ning</li>
                                    <li className="mb-2 hover:text-red-500">Victor</li>
                                    <li className="mb-2 hover:text-red-500">Yonex</li>
                                    <li className="mb-2 hover:text-red-500">Kawasaki</li>
                                    <li className="mb-2 hover:text-red-500">Fleet</li>
                                    <li className="mb-2 hover:text-red-500">Babolat</li>
                                </ul>
                            </div>
                        </div>


                    </div>

                </div>
            </footer>
        </>
    )
};

export default Footer;