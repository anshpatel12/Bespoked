import axios from 'axios'

const API_BASE_URL = 'http://localhost:8080' // Adjust this based on your API URL

export const fetchCustomers = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/customer`)
    return response.data.data
  } catch (error) {
    console.error('Error fetching customers:', error)
    throw error
  }
}

export const fetchSalespersons = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/saleperson`)
    return response.data.data
  } catch (error) {
    console.error('Error fetching salespersons:', error)
    throw error
  }
}

export const fetchProducts = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/product`)
    return response.data.data
  } catch (error) {
    console.error('Error fetching products:', error)
    throw error
  }
}

export const fetchSales = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/sales`)
    return response.data.data
  } catch (error) {
    console.error('Error fetching sales:', error)
    throw error
  }
}

export const fetchReports = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/report`)
    return response.data.data
  } catch (error) {
    console.error('Error fetching reports:', error)
    throw error
  }
}
