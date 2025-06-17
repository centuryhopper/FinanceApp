
import express from 'express'
import userRoutes from './src/routes/user.routes'
import accountRoutes from './src/routes/account_routes'
import plaidRoutes from './src/routes/plaid_routes'
import path from 'path'
import rateLimiter from './src/middlewares/rateLimit'

const app = express()

app.use(express.json())
app.use(express.urlencoded({ extended: true }))
app.use(rateLimiter)


app.use('/api/users', userRoutes)
app.use('/api/account', accountRoutes)
app.use('/api/plaid', plaidRoutes)

// Serve static files from the 'dist' directory
app.use(express.static('dist'));

export default app
  
  