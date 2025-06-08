import axios from 'axios';
import { useEffect, useState } from 'react';
import { usePlaidLink } from 'react-plaid-link';
import './App.css';
import Navbar from './components/Navbar';

axios.defaults.baseURL = "http://localhost:5003"

function PlaidAuth({publicToken}: { publicToken: string })
{
  useEffect(()=>{
    (async function(){
      const responseFromPublicToken = await axios.post('api/Plaid/exchange-public-token', {
          publicToken: publicToken
        })
        // console.log('responseFromPublicToken: ' + JSON.stringify(responseFromPublicToken));
      const response = await axios.post('api/Plaid/get-auth', {accessToken: responseFromPublicToken.data.access_token.access_token})
      console.log(response);
    })()
  },[])

  return (<span>{publicToken}</span>)
}

function App()
{
  const [linkToken, setLinkToken] = useState('')
  const [publicToken, setpublicToken] = useState('')
  
  const { open, ready } = usePlaidLink({
    token: linkToken,
    onSuccess: (public_token, metadata) => {
      // console.log('success', public_token, metadata);
      setpublicToken(public_token);
      
      // send public_token to server
      (async function() {
        const response = await axios.post('api/Plaid/exchange-public-token', {
          publicToken: public_token
        })
        // console.log(response.data);
      })()

    },
  });

  useEffect(() => {
    (async function() {
      const response = await axios.get("api/Plaid/get-link-token");
      // console.log(response.data);
      setLinkToken(response.data.link_token)
    })()
  }, [])

  // open Link immediately when ready
  useEffect(() => {
    
    if (ready) {
      open();
    }
  }, [ready, open]);

  return (
    publicToken ? (<PlaidAuth publicToken={publicToken}/>):
    <>
      <Navbar />
      <div className="flex items-center justify-center h-screen">
        <button onClick={() => open()} disabled={!ready}>
          Connect a bank account
        </button>
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
