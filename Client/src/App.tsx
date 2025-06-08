// import { useState } from 'react'
import './App.css'
import Navbar from './components/Navbar'

function App() {
  // const [count, setCount] = useState(0)

  return (
    <>
      <Navbar />
      <div className="flex items-center justify-center h-screen">
        {/* <div className="text-red-500 text-5xl">If you see this in red, Tailwind works</div> */}
        <div className="text-center text-3xl font-bold">
          <div onClick={() => console.log('clicked!')} className="bg-green-600 text-white p-6 rounded-lg">
            Tailwind is working in App! yooo
          </div>
        </div>
      </div>
    </>
  )
}

export default App
