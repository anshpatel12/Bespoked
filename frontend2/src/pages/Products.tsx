import { useEffect, useState } from 'react'
import { fetchProducts } from '../api'

interface Product {
  productId: number
  name: string
  manufacturer: string
  style: string
  purchasePrice: number
  salePrice: number
  qtyOnHand: number
  commissionPct: number
}

function Products() {
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const getProducts = async () => {
      try {
        const data = await fetchProducts()
        setProducts(data)
      } catch (err) {
        setError('Failed to fetch products')
      } finally {
        setLoading(false)
      }
    }

    getProducts()
  }, [])

  if (loading) return <p>Loading products...</p>
  if (error) return <p style={{ color: 'red' }}>{error}</p>

  return (
    <div>
      <h2>Products</h2>
      <table>
        <thead>
          <tr>
            <th>Product Id</th>
            <th>Name</th>
            <th>Manufacturer</th>
            <th>Style</th>
            <th>Purchase Price</th>
            <th>Sale Price</th>
            <th>Quantity on Hand</th>
            <th>Commission Percentage</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.productId}>
              <td>{product.productId}</td>
              <td>{product.name}</td>
              <td>{product.manufacturer}</td>
              <td>{product.style}</td>
              <td>${product.purchasePrice}</td>
              <td>${product.salePrice}</td>
              <td>{product.qtyOnHand}</td>
              <td>{product.commissionPct}%</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default Products
