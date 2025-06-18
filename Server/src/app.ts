
import express from 'express'
import accountRoutes from './routes/account_routes'
import plaidRoutes from './routes/plaid_routes'
import rateLimiter from './middlewares/rateLimit'
import path from 'path'

const app = express()

app.use(express.json())
app.use(express.urlencoded({ extended: true }))
app.use(rateLimiter)


app.use((req, res, next) => {
  console.log(`Incoming request: ${req.method} ${req.url}`);
  next();
});


app.use('/api/account', accountRoutes)
app.use('/api/plaid', plaidRoutes)

// Serve static files from the 'dist' directory
// app.use(express.static('../dist'));
// Serve static files from Vue build
// console.log(path.join(__dirname, '../dist'));
app.use(express.static(path.join(__dirname, '../dist')));

// Catch-all route to index.html (for Vue router)
// must use this regex syntax for express v5+
app.get(/(.*)/, (req, res) => {
  res.sendFile(path.join(__dirname, '../dist/index.html'));
});

export default app
  
  