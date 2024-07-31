import axios from './customizeAxios';

const fetchPaginatedPayments = async (params) => {
  const {
      searchQuery = "",
      sortBy = "paymentdate_asc",
      status = "",
      pageIndex = 1,
      pageSize = 10
  } = params;

  const queryParams = new URLSearchParams();
  // if (start !== undefined || start !== null) queryParams.append('start', start);
  // if (end !== undefined || end !== null) queryParams.append('end', end);
  if (searchQuery) queryParams.append('searchQuery', searchQuery);
  if (sortBy) queryParams.append('sortBy', sortBy);
  if (status !== undefined) queryParams.append('status', status);
  if (pageIndex) queryParams.append('pageIndex', pageIndex);
  if (pageSize) queryParams.append('pageSize', pageSize);
  console.log(`Payment?${queryParams.toString()}`);
  return axios.get(`Payment?${queryParams.toString()}`);
};

const fetchPaymentById = async (id) => {
  try {
      const response = await axios.get(`Payment/${id}`);
      return response;
  } catch (error) {
      console.error("Error fetching payment by ID:", error);
      throw error;
  }
};

// Add a new payment
const addPayment = async (payment) => {
  try {
      const response = await axios.post('Payment', payment);
      return response;
  } catch (error) {
      console.error("Error adding payment:", error);
      throw error;
  }
};

// Update an existing payment
const updatePayment = async (payment) => {
  try {
      const response = await axios.put('Payment', payment);
      return response;
  } catch (error) {
      console.error("Error updating payment:", error);
      throw error;
  }
};

// Delete a payment by its ID
const deletePayment = async (id) => {
  try {
      const response = await axios.delete(`Payment/${id}`);
      return response;
  } catch (error) {
      console.error("Error deleting payment:", error);
      throw error;
  }
};

// Generate a payment token for a specific booking
const generatePaymentToken = async (bookingId) => {
  try {
      const response = await axios.get(`Payment/GeneratePaymentToken/${bookingId}`);
      return response;
  } catch (error) {
      console.error("Error generating payment token:", error);
      throw error;
  }
};

// Process a payment with role and token
const processPayment = async (role, token) => {
  try {
      const response = await axios.post('Payment/ProcessPayment', null, { params: { role, token } });
      return response;
  } catch (error) {
      console.error("Error processing payment:", error);
      throw error;
  }
};

export {
  fetchPaginatedPayments,
  fetchPaymentById,
  addPayment,
  updatePayment,
  deletePayment,
  generatePaymentToken,
  processPayment
};
