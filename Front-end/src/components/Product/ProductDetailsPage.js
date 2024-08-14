import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner, faInfo } from "@fortawesome/free-solid-svg-icons";
import {
    fetchProduct, fetchRelatedProducts
} from "../../redux/slice/productSlice";
import {
    fetchColorsByProduct, fetchSingleColor
} from "../../redux/slice/colorSlice";
import {
    fetchSize, fetchSizesForProduct
} from "../../redux/slice/sizeSlice";
import { fetchSingleCategory } from "../../redux/slice/categorySlice";
import { fetchSupplier } from "../../redux/slice/supplierSlice";
import {
    fetchAllProductVariants, fetchProductVariant
} from "../../redux/slice/productVariantSlice";
import { fetchNumberOfItems } from "../../redux/slice/cartSlice";
import {
    createReview,
    fetchAllReviews, fetchAverageRating
} from '../../redux/slice/reviewSlice';
import { saveCartToCookie } from "../../api/cartAxios";
import Product from './Product';
import { Rating, Typography } from '@mui/material';
import { fetchUserDetailById } from "../../api/userDetailAxios";

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

    const reviewDetailStatus = useSelector((state) => state.review.status);
    const reviewDetailError = useSelector((state) => state.review.error);

    const [rating, setRating] = useState(0);
    const [reviewers, setReviewers] = useState({});
    const [content, setContent] = useState("");

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
            dispatch(fetchAllReviews({
                productId: product.productId,
            }));
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

    const handleReview = () => {
        if (user && user.id) {
            dispatch(createReview({
                reviewText: content,
                rating: rating,
                productId: product.productId,
                userId: user.id
            })).then(() => {
                toast.success("Review successfully added");
                dispatch(fetchAverageRating(product.productId));
            }).then(() => {
                dispatch(fetchAllReviews({ productId: product.productId }));
            })
            // .catch((error) => {
            //     toast.error("Error adding review");
            // });
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
        const fetchReviewers = async () => {
            if (reviews && reviews.items) {
                const reviewerDetails = await Promise.all(
                    reviews.items.map(async (item) => ({
                        userId: item.userId,
                        details: await fetchUserDetailById(item.userId)
                    }))
                );

                const reviewerDetailsMap = {};
                reviewerDetails.forEach(({ userId, details }) => {
                    reviewerDetailsMap[userId] = details;
                });

                setReviewers(reviewerDetailsMap);
            }
        };

        fetchReviewers();
    }, [reviews]);

    if (productStatus === 'failed') {
        return <div className="text-red-500">Error: {productError}</div>;
    }
    if (productStatus === 'loading') {
        return <div className="text-blue-500">
            <FontAwesomeIcon icon={faSpinner} spin />
        </div>;
    }

    return (
        <div className="container mx-auto p-6 mb-10 bg-white rounded-lg shadow-lg">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="relative">
                    <img src={product.imageUrl} alt={product.productName} className="w-full rounded-lg shadow-md" />
                </div>
                <div>
                    <h1 className="text-4xl font-extrabold text-gray-800 mb-4">{product.productName}</h1>
                    <p className="text-2xl font-semibold text-red-600 mb-4">${productVariant.price || product.basePrice}</p>
                    <p className="text-lg font-medium mt-2">Supplier:</p>
                    {supplierStatus === 'failed' && (
                        <div className="text-red-500">Error: {supplierError}</div>
                    )}
                    {supplierStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {supplierStatus === 'succeeded' && (
                        <p className="text-lg font-light">{supplier.companyName}</p>
                    )}

                    <p className="text-lg font-medium mt-2">Category:</p>
                    {categoryStatus === 'failed' && (
                        <div className="text-red-500">Error: {categoryError}</div>
                    )}
                    {categoryStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {categoryStatus === 'succeeded' && (
                        <p className="text-lg font-light">{category.categoryName}</p>
                    )}

                    {sizeOfProduct && sizeOfProduct.length > 0 && (
                        <div className="mt-4">
                            <p className="text-lg font-medium">Size:</p>
                            {sizeOfProductStatus === 'failed' && (
                                <div className="text-red-500">Error: {sizeOfProductError}</div>
                            )}
                            {sizeOfProductStatus === 'loading' && (
                                <div className="text-blue-500">
                                    <FontAwesomeIcon icon={faSpinner} spin />
                                </div>
                            )}
                            {sizeOfProductStatus === 'succeeded' && (
                                sizeOfProduct.map((size) => (
                                    <button
                                        key={size.sizeId}
                                        onClick={() => dispatch(fetchSize(size.sizeId))}
                                        className={`px-4 py-2 mt-2 mr-2 rounded-lg border transition duration-200 ease-in-out transform hover:-translate-y-1 hover:bg-blue-500 hover:text-white ${selectedSize?.sizeId === size.sizeId ? 'bg-blue-500 text-white cursor-not-allowed' : 'bg-gray-200 text-gray-700'}`}
                                        disabled={selectedSize?.sizeId === size.sizeId}
                                    >
                                        {size.sizeName}
                                    </button>
                                ))
                            )}
                        </div>
                    )}

                    <p className="text-lg font-medium mt-4">Color:</p>
                    {colorOfProductStatus === 'failed' && (
                        <div className="text-red-500">Error: {colorOfProductError}</div>
                    )}
                    {colorOfProductStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {colorOfProductStatus === 'succeeded' && (
                        colorOfProduct.map((color) => (
                            <button
                                key={color.colorId}
                                onClick={() => dispatch(fetchSingleColor(color.colorId))}
                                className={`px-4 py-2 mt-2 mr-2 rounded-lg border transition duration-200 ease-in-out transform hover:-translate-y-1 ${selectedColor?.colorId === color.colorId ? 'bg-gray-700 text-white cursor-not-allowed' : 'bg-gray-200 text-gray-700'}`}
                                style={{
                                    backgroundColor: selectedColor?.colorId === color.colorId ? color.colorName : 'gray',
                                    borderColor: selectedColor?.colorId === color.colorId ? color.colorName : 'gray'
                                }}
                                disabled={selectedColor?.colorId === color.colorId}
                            >
                                {color.colorName}
                            </button>
                        ))
                    )}
                    <div className="mt-6">
                        <button onClick={handleAddCart}
                            className="bg-red-600 hover:bg-red-700 text-white px-6 py-3 rounded-lg shadow-md transition duration-200 ease-in-out transform hover:-translate-y-1">
                            Add to cart
                        </button>
                    </div>
                </div>
            </div>
            <div className="border-t-4 border-red-500 mt-12">
                <h2 className="text-2xl font-bold p-3 bg-gray-100 text-gray-800 flex items-center"><FontAwesomeIcon icon={faInfo} className="mr-2" /> Description</h2>
                <p className="text-lg font-light p-4 text-gray-600">{product.productDescription}</p>
            </div>

            <div className="border-t-4 border-red-500 mt-12">
                <h2 className="text-2xl font-bold p-3 bg-gray-100 text-gray-800">Reviews</h2>
                <div className="p-4">
                    <Typography component="legend">Rating</Typography>
                    {averageReviewsStatus === 'failed' && (
                        <div className="text-red-500">Error: {averageReviewsError}</div>
                    )}
                    {averageReviewsStatus === 'loading' && (
                        <div className="text-blue-500">
                            <FontAwesomeIcon icon={faSpinner} spin />
                        </div>
                    )}
                    {averageReviewsStatus === 'succeeded' && (
                        <div>
                            <Rating name="half-rating-read" defaultValue={averageReviews} precision={0.2} readOnly /><span className='ml-3'>{averageReviews.toFixed(1)}</span>
                        </div>
                    )}
                    <div className='mt-8'>
                        {reviewsStatus === 'failed' && (
                            <div className="text-red-500">Error: {reviewsError}</div>
                        )}
                        {reviewsStatus === 'loading' && (
                            <div className="text-blue-500">
                                <FontAwesomeIcon icon={faSpinner} spin />
                            </div>
                        )}
                        {reviewsStatus === 'succeeded' && (
                            <div>
                                <div className="mb-4">
                                    <Typography component="legend">Write a review</Typography>
                                    <Rating
                                        name="half-rating"
                                        defaultValue={0}
                                        precision={0.5}
                                        onChange={(e, newValue) => setRating(newValue)}
                                    />
                                    <textarea
                                        className='border border-gray-300 p-3 rounded-lg w-full mt-2'
                                        placeholder='Write a review'
                                        onChange={(e) => setContent(e.target.value)}
                                    ></textarea>
                                    <button
                                        className='bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg mt-2 shadow-md transition duration-200 ease-in-out transform hover:-translate-y-1'
                                        onClick={handleReview}
                                    >
                                        Submit
                                    </button>
                                    <span>
                                        {reviewDetailStatus === 'failed' && (
                                            <div className="text-red-500">Error: {reviewDetailError}</div>
                                        )}
                                        {reviewDetailStatus === 'loading' && (
                                            <div className="text-blue-500">
                                                <FontAwesomeIcon icon={faSpinner} spin />
                                            </div>
                                        )}
                                    </span>
                                </div>
                                {reviews && reviews.items && reviews.items.map((review) => (
                                    <div key={review.reviewId} className='border border-gray-300 p-4 rounded-lg mb-4 shadow-md'>
                                        <div className='flex justify-between'>
                                            {reviewers[review.userId] && (
                                                <p className='text-lg font-semibold'>
                                                    {`${reviewers[review.userId].fullName}`}
                                                </p>
                                            )}
                                            <Rating name="half-rating-read" defaultValue={review.rating} precision={0.2} readOnly />
                                        </div>
                                        <p className='text-gray-600 mt-2'>{review.reviewText}</p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>
            <div className='mt-12'>
                <div className='border-t-4 border-red-500'>
                    <h2 className='bg-gray-100 text-2xl font-bold p-3 mb-10 text-gray-800'>Related Products</h2>
                    <div>
                        {relatedProductsStatus === 'failed' && (
                            <div className="text-red-500">Error: {relatedProductsError}</div>
                        )}
                        {relatedProductsStatus === 'loading' && (
                            <div className="text-blue-500">
                                <FontAwesomeIcon icon={faSpinner} spin />
                            </div>
                        )}
                        {relatedProductsStatus === 'succeeded' && (
                            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
                                {relatedProducts && relatedProducts.map((product) => (
                                    <div key={product.productId} className="border border-gray-300 p-4 rounded-lg shadow-md hover:shadow-lg transition duration-200 ease-in-out transform hover:-translate-y-1">
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
