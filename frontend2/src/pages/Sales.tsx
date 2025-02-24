import { useEffect, useState } from 'react'
import { fetchSales } from '../api'

interface Sales {
  saleId: number
  productName: string
  customerName: string  
  saleDate: string
  quantitySold: number
  totalSalePrice: number
  salesPersonName: string
  salesPersonCommission: number
}

function Sales() {
  const [salespersons, setSales] = useState<Sales[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const getSales = async () => {
      try {
        const data = await fetchSales()
        setSales(data)
      } catch (err) {
        setError('Failed to fetch sales')
      } finally {
        setLoading(false)
      }
    }

    getSales()
  }, [])

  if (loading) return <p>Loading sales...</p>
  if (error) return <p style={{ color: 'red' }}>{error}</p>

  return (
    <div>
      <h2>Sales</h2>
      <table>
        <thead>
          <tr>
            <th>Sales Id</th>
            <th>Product Name</th>
            <th>Customer Name</th>
            <th>Sale Date</th>
            <th>Quantity Sold</th>
            <th>Total Sale Price</th>
            <th>Salesperson Name</th>
            <th>Salesperson Commission</th>
          </tr>
        </thead>
        <tbody>
          {salespersons.map((sp) => (
            <tr key={sp.saleId}>
              <td>{sp.saleId}</td>
              <td>{sp.productName}</td>
              <td>{sp.customerName}</td>
              <td>{sp.saleDate}</td>
              <td>{sp.quantitySold}</td>
              <td>${sp.totalSalePrice}</td>
              <td>{sp.salesPersonName}</td>
              <td>${sp.salesPersonCommission}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default Sales
