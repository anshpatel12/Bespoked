import { useEffect, useState } from 'react'
import { fetchSalespersons } from '../api'

interface Salesperson {
  salepersonId: number
  firstName: string
  lastName: string
  address: string
  phone: string
  startDate: string
  terminationDate: string
  manager: string
}

function Salespersons() {
  const [salespersons, setSalespersons] = useState<Salesperson[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const getSalespersons = async () => {
      try {
        const data = await fetchSalespersons()
        setSalespersons(data)
      } catch (err) {
        setError('Failed to fetch salespersons')
      } finally {
        setLoading(false)
      }
    }

    getSalespersons()
  }, [])

  if (loading) return <p>Loading salespersons...</p>
  if (error) return <p style={{ color: 'red' }}>{error}</p>

  return (
    <div>
      <h2>Salespersons</h2>
      <table>
        <thead>
          <tr>
            <th>Sale Person Id</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Address</th>
            <th>Phone</th>
            <th>Start Date</th>
            <th>Termination Date</th>
            <th>Manager</th>
          </tr>
        </thead>
        <tbody>
          {salespersons.map((sp) => (
            <tr key={sp.salepersonId}>
              <td>{sp.salepersonId}</td>
              <td>{sp.firstName}</td>
              <td>{sp.lastName}</td>
              <td>{sp.address}</td>
              <td>{sp.phone}</td>
              <td>{sp.startDate}</td>
              <td>{sp.terminationDate}</td>
              <td>{sp.manager}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default Salespersons
