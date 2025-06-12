import axios from 'axios';
import { useEffect, useState } from 'react';
import { usePlaidLink } from 'react-plaid-link';

// axios.defaults.baseURL = "http://localhost:5003"

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

/*
plaid api interaction steps:
  - client asks server for link token
  - server will ask plaid for link token to give to client
  - client is now ready to open up plaid ui link and pass link token to plaid
  - plaid will give a public token to client and client will send that public token to server 
  - server sends that public token to plaid and if all goes well, then plaid will create an access token to send back to server
  - server can then store that access token in the db and tie it to the currently logged in user. Send that access token to the client should for debugging purposes only and not in a real world app!!


CHAT GPT summary:
  ✅ Plaid API Integration Flow (Link Token → Access Token)
🧩 Step-by-step:

    Client → Server: "I want to link my bank account."

        The client sends a request to the server to create a Link Token.

    Server → Plaid: Create a Link Token

        The server calls POST /link/token/create on Plaid’s API, optionally including:

            user object (with unique user ID)

            products, client_name, etc.

    Server → Client: Send Link Token

        Server returns the Link Token to the client.

        The client uses this token to initialize Plaid Link (the UI popup).

    Client → Plaid (via Link UI): User authenticates & links account

        User interacts with the Plaid Link interface.

        On success, Plaid returns a Public Token to the client.

    Client → Server: Send Public Token

        The client sends the Public Token to the server (e.g., via API).

    Server → Plaid: Exchange Public Token for Access Token

        Server calls POST /item/public_token/exchange with the Public Token.

        Plaid responds with:

            Access Token (long-lived)

            Item ID (useful for tracking linked accounts)

    Server: Store Access Token securely

        Save the Access Token in your DB, tied to the authenticated user.

        ⚠️ Do NOT send this back to the client (except in development/debug mode).

*/

export default function Home() {
  const [linkToken, setLinkToken] = useState('');
  const [publicToken, setpublicToken] = useState('');

  const { open, ready } = usePlaidLink({
    token: linkToken,
    onSuccess: (public_token, _) => {
      setpublicToken(public_token);
      // send public_token to server
      (async function () {
        
        const accessToken = await axios.post('api/Plaid/exchange-public-token', {
          publicToken: public_token
        });
      })();
    },
  });

  // componentDidMount()
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
