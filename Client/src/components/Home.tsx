import axios from 'axios';
import { useEffect, useState } from 'react';
import { usePlaidLink } from 'react-plaid-link';

axios.defaults.baseURL = "http://localhost:5003"

/*

    transaction model:
    {
        date: ...,
        amount: ...,
        merchant: ...,
        category
    }
                            
*/

function PlaidAuth({ publicToken }: { publicToken: string }) {
  useEffect(() => {
    (async function () {
      const responseFromPublicToken = await axios.post('api/Plaid/exchange-public-token', {
        publicToken: publicToken
      });
      await axios.post('api/Plaid/get-auth', {
        accessToken: responseFromPublicToken.data.access_token.access_token
      });
    })();
    
  }, []);

  return (<span>{publicToken}</span>);
}


export default function Home() {
  const [linkToken, setLinkToken] = useState('');
  const [publicToken, setpublicToken] = useState('');

  const { open, ready } = usePlaidLink({
    token: linkToken,
    onSuccess: (public_token, _) => {
      setpublicToken(public_token);

      // send public_token to server
      (async function () {
        await axios.post('api/Plaid/exchange-public-token', {
          publicToken: public_token
        });
      })();
    },
  });

  useEffect(() => {
    (async function () {
      const response = await axios.get("api/Plaid/get-link-token");
      setLinkToken(response.data.link_token);
    })();
  }, []);

  return (
    publicToken ? (<PlaidAuth publicToken={publicToken} />) :
      (
        <>
          <h1 className="fw-bold fs-3 mt-5 mb-3">Welcome to My Finance App</h1>

          <button
            className="btn btn-primary mb-4"
            onClick={() => open()}
            disabled={!ready}
          >
            Connect a bank account
          </button>

          {/* First row of cards */}
          <div className="row g-4">
            <div className="col-md-6">
              <div className="bg-secondary text-white p-4 rounded shadow">
                <div className="text-center">
                  <h2 className="fs-4 fw-semibold mb-2">My accounts</h2>
                  <p>Display list of bank accounts you are connected to</p>
                </div>
              </div>
            </div>
            <div className="col-md-6">
              <div className="bg-secondary text-white p-4 rounded shadow">
                <div className="text-center">
                  <h2 className="fs-4 fw-semibold mb-2">Recent transactions</h2>
                  <p>
                    List the five most recent transactions as a preview to the user

                    {/*
                      transaction model:
                      {
                          date: ...,
                          amount: ...,
                          merchant: ...,
                          category
                      }
                    */}
                  </p>
                </div>
              </div>
            </div>
          </div>

          {/* Second row of cards */}
          <div className="row g-4 mt-4">
            <div className="col-md-3"></div>
            <div className="col-md-6">
              <div className="bg-secondary text-white p-4 rounded shadow">
                <div className="text-center">
                  <h2 className="fs-4 fw-semibold mb-2">Inflow & Income</h2>
                  <p>Display list of bank accounts you are connected to</p>
                </div>
              </div>
            </div>
            <div className="col-md-3"></div>
          </div>
        </>
      )
  );
}
