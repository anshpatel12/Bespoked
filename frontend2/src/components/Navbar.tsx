import { Link } from 'react-router-dom'
import './Navbar.css'

function Navbar() {
  return (
    <nav className='navbar'>
      <h1>Bespoked Bikes</h1>
      <ul>
        <li>
          <Link to='/'>Home</Link>
        </li>
        <li>
          <Link to='/customers'>Customers</Link>
        </li>
        <li>
          <Link to='/salespersons'>Salespersons</Link>
        </li>
        <li>
          <Link to='/products'>Products</Link>
        </li>
        <li>
          <Link to='/sales'>Sales</Link>
        </li>
        <li>
          <Link to='/reports'>Reports</Link>
        </li>
      </ul>
    </nav>
  )
}

export default Navbar
