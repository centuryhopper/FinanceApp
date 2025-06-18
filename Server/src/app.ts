
import express from 'express'
import accountRoutes from './routes/account_routes'
import plaidRoutes from './routes/plaid_routes'
import path from 'path'
import rateLimiter from './middlewares/rateLimit'

const app = express()

app.use(express.json())
app.use(express.urlencoded({ extended: true }))
app.use(rateLimiter)


app.use('/api/account', accountRoutes)
app.use('/api/plaid', plaidRoutes)

// Serve static files from the 'dist' directory
app.use(express.static('dist'));

export default app
  
  