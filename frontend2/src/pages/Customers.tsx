import { useEffect, useState } from 'react'
import { fetchCustomers } from '../api'

interface Customer {
  customerId: number
  firstName: string
  lastName: string
  phone: string
  startDate: string
}

function Customers() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const getCustomers = async () => {
      try {
        const data = await fetchCustomers()
        setCustomers(data)
      } catch (err) {
        setError('Failed to fetch customers')
      } finally {
        setLoading(false)
      }
    }

    getCustomers()
  }, [])

  if (loading) return <p>Loading customers...</p>
  if (error) return <p style={{ color: 'red' }}>{error}</p>

  return (
    <div>
      <h2>Customers</h2>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Phone</th>
            <th>StartDate</th>
          </tr>
        </thead>
        <tbody>
          {customers.map((customer) => (
            <tr key={customer.customerId}>
              <td>{customer.customerId}</td>
              <td>{customer.firstName}</td>
              <td>{customer.lastName}</td>
              <td>{customer.phone}</td>
              <td>{customer.startDate}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default Customers
