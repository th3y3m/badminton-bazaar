import { fetchCategoryById } from "../../api/categoryAxios";
import { fetchSupplierById } from "../../api/supplierAxios";
import { fetchPaginatedProductVariants } from "../../api/productVariantAxios";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faInfo } from '@fortawesome/free-solid-svg-icons';
import { fetchColorsOfProduct } from "../../api/colorAxios";
import { fetchSizesOfProduct } from "../../api/sizeAxios";
import { addToCart, numberOfItemsInCart, saveCartToCookie } from "../../api/cartAxios";
import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../../AuthContext";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { fetchProductById } from "../../api/productAxios";

const ProductDetailsPage = () => {

    const { id: productId } = useParams();
    const { user, setCartCount, cartCount } = useContext(AuthContext);
    const navigate = useNavigate();
    const [category, setCategory] = useState({});
    const [supplier, setSupplier] = useState({});
    const [sizeOfProduct, setSizeOfProduct] = useState([]);
    const [selectedSize, setSelectedSize] = useState({});
    const [colorOfProduct, setColorOfProduct] = useState([]);
    const [selectedColor, setSelectedColor] = useState({});
    const [productVariant, setProductVariant] = useState({});
    const [product, setProduct] = useState({});

    const getProductById = async () => {
        try {
            const data = await fetchProductById(productId);
            setProduct(data);
            console.log("Product: ", data);
        } catch (error) {
            console.error("Error fetching product:", error);
        }
    };

    const handleAddCart = () => {
        if (user) {
          saveCartToCookie(productVariant.productVariantId, user.userId)
            .then(() => {
              // Update the cart count
              setCartCount(cartCount + 1);

              toast.success("Added to cart successfully");
            })
            .catch((error) => {
              console.error("Error adding to cart:", error);
              toast.error("Error adding to cart");
            });
        } else {
          toast.error('Please login first', {
            position: "top-right",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
            theme: "colored",
          });
          navigate('/login');
        }
      };
      

    const getColorsOfProduct = async () => {
        try {
            const data = await fetchColorsOfProduct(productId);
            setSelectedColor(data[0]);
            setColorOfProduct(data);
        } catch (error) {
            console.error("Error fetching colors:", error);
        }
    };

    const getSizeOfProduct = async () => {
        try {
            const data = await fetchSizesOfProduct(productId);
            setSelectedSize(data[0]);
            setSizeOfProduct(data);
        } catch (error) {
            console.error("Error fetching sizes:", error);
        }
    };

    const getProductVariant = async () => {
        try {
            const data = await fetchPaginatedProductVariants({
                sortBy: "price_asc",
                status: true,
                colorId: selectedColor.colorId || "",
                sizeId: selectedSize.sizeId || "",
                productId: productId,
                pageIndex: 1,
                pageSize: 20
            });
            setProductVariant(data.items[0]);
        } catch (error) {
            console.error("Error fetching variant:", error);
        }
    };

    const fetchCategory = async () => {
        try {
            const data = await fetchCategoryById(product.categoryId);
            setCategory(data);
        } catch (error) {
            console.error("Error fetching category:", error);
        }
    };

    const fetchSupplier = async () => {
        try {
            const data = await fetchSupplierById(product.supplierId); // fixed the parameter from product.categoryId to product.supplierId
            setSupplier(data);
        } catch (error) {
            console.error("Error fetching supplier:", error);
        }
    };

    useEffect(() => {
        getProductById();
    }, [productId]);

    useEffect(() => {
        if (product.productId) {
            getColorsOfProduct();
            getSizeOfProduct();
            fetchCategory();
            fetchSupplier();
        }
        console.log("Product var: ", productVariant);

    }, [product]);

    useEffect(() => {
        if (selectedSize.sizeId || selectedColor.colorId) {
            getProductVariant();
        }
    }, [selectedSize, selectedColor]);

    return (
        <div className="container mx-auto relative mb-10">
            <div>
                <div className="grid grid-cols-12">
                    <div className="col-span-6">
                        <img src={product.imageUrl} alt={product.productName} className="w-full" />
                    </div>
                    <div className="col-span-6">
                        <h1 className="text-4xl font-bold">{product.productName}</h1>
                        <p className="text-2xl font-bold text-red-600">{productVariant.price || product.basePrice} $</p>
                        <p className="text-lg font-semibold mt-2">Supplier:</p>
                        <p className="text-lg font-light">{supplier.companyName}</p>
                        <p className="text-lg font-semibold mt-2">Category:</p>
                        <p className="text-lg font-light">{category.categoryName}</p>
                        <p className="text-lg font-semibold mt-2">Size:</p>
                        {sizeOfProduct.length > 0 && sizeOfProduct.map((size) => (
                            <button
                                key={size.sizeId}
                                onClick={() => setSelectedSize(size)}
                                className={`px-2 py-1 rounded-lg mr-2 ${selectedSize?.sizeId === size.sizeId
                                    ? 'bg-blue-500 text-white cursor-not-allowed'
                                    : 'bg-gray-300 text-black'
                                    }`}
                                disabled={selectedSize?.sizeId === size.sizeId}
                            >
                                {size.sizeName}
                            </button>
                        ))}

                        <p className="text-lg font-semibold mt-2">Color:</p>
                        {colorOfProduct.length > 0 && colorOfProduct.map((color) => (
                            <button
                                key={color.colorId}
                                onClick={() => setSelectedColor(color)}
                                className={`px-2 py-1 rounded-lg mr-2 text-black ${selectedColor?.colorId === color.colorId
                                    ? 'text-white cursor-not-allowed'
                                    : ''
                                    }`}
                                style={{
                                    backgroundColor: selectedColor?.colorId === color.colorId ? color.colorName : 'gray',
                                    borderColor: selectedColor?.colorId === color.colorId ? color.colorName : 'gray'
                                }}
                                disabled={selectedColor?.colorId === color.colorId}
                            >
                                {color.colorName}
                            </button>
                        ))}
                        <button onClick={handleAddCart}
                            className="bg-red-600 text-white px-4 py-2 rounded-lg mt-2">
                            Add to cart
                        </button>
                    </div>
                </div>
                <div className="border border-[#a09f9f]">
                    <h2 className="text-2xl font-bold p-3 bg-[#e4e1e1]"><FontAwesomeIcon icon={faInfo} /> Description</h2>
                    <p className="text-lg font-light">{product.productDescription}</p>
                </div>
            </div>
        </div>
    );
}

export default ProductDetailsPage;
