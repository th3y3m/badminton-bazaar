import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faInfo } from '@fortawesome/free-solid-svg-icons';
import { saveCartToCookie } from "../../api/cartAxios";
import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useDispatch, useSelector } from 'react-redux';
import { fetchProduct, fetchRelatedProducts } from "../../redux/slice/productSlice";
import { fetchColorsByProduct, fetchSingleColor } from "../../redux/slice/colorSlice";
import { fetchSize, fetchSizesForProduct } from "../../redux/slice/sizeSlice";
import { fetchSingleCategory } from "../../redux/slice/categorySlice";
import { fetchSupplier } from "../../redux/slice/supplierSlice";
import { fetchAllProductVariants, fetchProductVariant } from "../../redux/slice/productVariantSlice";
import { fetchNumberOfItems } from "../../redux/slice/cartSlice";
import Product from './Product';
import { Rating, Typography } from '@mui/material';
import { fetchAverageRating } from '../../redux/slice/reviewSlice';

const ProductDetailsPage = () => {
    const { id: productId } = useParams();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const user = useSelector((state) => state.auth.token);

    const product = useSelector((state) => state.product.product);
    const productStatus = useSelector((state) => state.product.status);
    const productError = useSelector((state) => state.product.error);

    const sizeOfProduct = useSelector((state) => state.size.sizes);
    const sizeOfProductStatus = useSelector((state) => state.size.status);
    const sizeOfProductError = useSelector((state) => state.size.error);
    const selectedSize = useSelector((state) => state.size.sizeDetail);

    const colorOfProduct = useSelector((state) => state.color.colors);
    const colorOfProductStatus = useSelector((state) => state.color.status);
    const colorOfProductError = useSelector((state) => state.color.error);
    const selectedColor = useSelector((state) => state.color.singleColor);

    const category = useSelector((state) => state.category.singleCategory);
    const categoryStatus = useSelector((state) => state.category.status);
    const categoryError = useSelector((state) => state.category.error);

    const supplier = useSelector((state) => state.supplier.supplierDetail);
    const supplierStatus = useSelector((state) => state.supplier.status);
    const supplierError = useSelector((state) => state.supplier.error);

    const productVariants = useSelector((state) => state.productVariant.productVariants);
    const productVariantStatus = useSelector((state) => state.productVariant.status);
    const productVariantError = useSelector((state) => state.productVariant.error);
    const productVariant = useSelector((state) => state.productVariant.productVariantDetail);

    const relatedProducts = useSelector((state) => state.product.relatedProducts);
    const relatedProductsStatus = useSelector((state) => state.product.status);
    const relatedProductsError = useSelector((state) => state.product.error);

    const reviews = useSelector((state) => state.review.reviews);
    const reviewsStatus = useSelector((state) => state.review.status);
    const reviewsError = useSelector((state) => state.review.error);

    const averageReviews = useSelector((state) => state.review.averageRating);
    const averageReviewsStatus = useSelector((state) => state.review.status);
    const averageReviewsError = useSelector((state) => state.review.error);

    const handleAddCart = () => {
        if (user && user.id) {
            saveCartToCookie(productVariant.productVariantId, user.id)
                .then(() => {
                    dispatch(fetchNumberOfItems(user.id)); // Dispatch here after adding to cart
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

    useEffect(() => {
        dispatch(fetchProduct(productId));
    }, [productId, dispatch]);

    useEffect(() => {
        if (product.productId) {
            dispatch(fetchColorsByProduct(product.productId));
            dispatch(fetchSizesForProduct(product.productId));
            dispatch(fetchSingleCategory(product.categoryId));
            dispatch(fetchSupplier(product.supplierId));
            dispatch(fetchRelatedProducts(product.productId));
            dispatch(fetchAverageRating(product.productId));
        }
    }, [product, dispatch]);

    useEffect(() => {
        if (colorOfProduct.length > 0) {
            dispatch(fetchSingleColor(colorOfProduct[0].colorId));
        }
    }, [colorOfProduct, dispatch]);

    useEffect(() => {
        if (sizeOfProduct.length > 0) {
            dispatch(fetchSize(sizeOfProduct[0].sizeId));
        }
    }, [sizeOfProduct, dispatch]);

    useEffect(() => {
        if (selectedSize.sizeId || selectedColor.colorId) {
            dispatch(fetchAllProductVariants({
                sortBy: "price_asc",
                status: true,
                colorId: selectedColor.colorId || "",
                sizeId: selectedSize.sizeId || "",
                productId: productId,
                pageIndex: 1,
                pageSize: 20
            }));
        }
    }, [selectedSize, selectedColor, dispatch, productId]);

    useEffect(() => {
        if (productVariants.length > 0) {
            dispatch(fetchProductVariant(productVariants[0].productVariantId));
        }
    }, [productVariants, dispatch]);

    if (productStatus === 'failed') {
        return <div>Error: {productError}</div>;
    }
    if (productStatus === 'loading') {
        return <div>Loading...</div>;
    }

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
                        {supplierStatus === 'failed' && (
                            <div>Error: {supplierError}</div>
                        )}

                        {supplierStatus === 'loading' && (
                            <div>Loading...</div>
                        )}
                        {supplierStatus === 'succeeded' && (
                            <p className="text-lg font-light">{supplier.companyName}</p>
                        )}

                        <p className="text-lg font-semibold mt-2">Category:</p>
                        {categoryStatus === 'failed' && (
                            <div>Error: {categoryError}</div>
                        )}

                        {categoryStatus === 'loading' && (
                            <div>Loading...</div>
                        )}
                        {categoryStatus === 'succeeded' && (
                            <p className="text-lg font-light">{category.categoryName}</p>
                        )}

                        {sizeOfProduct && sizeOfProduct.length > 0 &&
                            <div>
                                <p className="text-lg font-semibold mt-2">Size:</p>
                                {sizeOfProductStatus === 'failed' && (
                                    <div>Error: {sizeOfProductError}</div>
                                )}

                                {sizeOfProductStatus === 'loading' && (
                                    <div>Loading...</div>
                                )}
                                {sizeOfProductStatus === 'succeeded' && (
                                    sizeOfProduct.map((size) => (
                                        <button
                                            key={size.sizeId}
                                            onClick={() => dispatch(fetchSize(size.sizeId))}
                                            className={`px-2 py-1 rounded-lg mr-2 ${selectedSize?.sizeId === size.sizeId
                                                ? 'bg-blue-500 text-white cursor-not-allowed'
                                                : 'bg-gray-300 text-black'
                                                }`}
                                            disabled={selectedSize?.sizeId === size.sizeId}
                                        >
                                            {size.sizeName}
                                        </button>
                                    ))
                                )}
                            </div>
                        }

                        <p className="text-lg font-semibold mt-2">Color:</p>
                        {colorOfProductStatus === 'failed' && (
                            <div>Error: {colorOfProductError}</div>
                        )}

                        {colorOfProductStatus === 'loading' && (
                            <div>Loading...</div>
                        )}
                        {colorOfProductStatus === 'succeeded' && (
                            colorOfProduct.map((color) => (
                                <button
                                    key={color.colorId}
                                    onClick={() => dispatch(fetchSingleColor(color.colorId))}
                                    className={`px-2 py-1 rounded-lg mr-2 text-black ${selectedColor?.colorId === color.colorId
                                        ? 'text-white cursor-not-allowed'
                                        : ''
                                        }`}
                                    style={{
                                        backgroundColor: selectedColor?.colorId === color.colorId ? color.colorName : 'gray',
                                        borderColor: selectedColor?.colorId === color.colorName ? color.colorName : 'gray'
                                    }}
                                    disabled={selectedColor?.colorId === color.colorId}
                                >
                                    {color.colorName}
                                </button>
                            ))
                        )}
                        <div>
                            <button onClick={handleAddCart}
                                className="bg-red-600 text-white px-4 py-2 rounded-lg mt-2">
                                Add to cart
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div className="border-t-4 border-red-500">
                <h2 className="text-2xl font-bold p-3 bg-[#e4e1e1]"><FontAwesomeIcon icon={faInfo} /> Description</h2>
                <p className="text-lg font-light">{product.productDescription}</p>
            </div>

            <div className="border-t-4 border-red-500">
                <h2 className="text-2xl font-bold p-3 bg-[#e4e1e1]">Reviews</h2>
                <div className=''>
                    <Typography component="legend">Rating</Typography>
                    <Rating name="half-rating-read" defaultValue={averageReviews} precision={0.2} readOnly /><span className='ml-3'>{averageReviews.toFixed(1)}</span>
                    <div className='mt-8'>
                        {reviewsStatus === 'failed' && (
                            <div>Error: {reviewsError}</div>
                        )}

                        {reviewsStatus === 'loading' && (
                            <div>Loading...</div>
                        )}
                        {reviewsStatus === 'succeeded' && (
                            <div>
                                {reviews && reviews.map((review) => (
                                    <div key={review.reviewId} className='border border-gray-300 p-2 rounded-lg'>
                                        <div className='flex justify-between'>
                                            <p className='text-lg font-semibold'>{review.title}</p>
                                            <Rating name="half-rating-read" defaultValue={review.rating} precision={0.2} readOnly />
                                        </div>
                                        <p className='text-gray-600'>{review.content}</p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>
            <div className='mt-10'>
                <div className='border-t-4 border-red-500'>
                    <h2 className='bg-[#e4e1e1] text-2xl font-bold p-3 mb-10'>Related Products</h2>
                    <div>
                        {relatedProductsStatus === 'failed' && (
                            <div>Error: {relatedProductsError}</div>
                        )}

                        {relatedProductsStatus === 'loading' && (
                            <div>Loading...</div>
                        )}
                        {relatedProductsStatus === 'succeeded' && (
                            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                                {relatedProducts && relatedProducts.map((product) => (
                                    <div key={product.productId} className="border border-gray-300 p-2 rounded-lg">
                                        <Product product={product} />
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                </div>

            </div>
        </div>
    );
}

export default ProductDetailsPage;