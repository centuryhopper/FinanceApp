// composables/usePlaid.ts
import axios from "axios";

export function usePlaid() {
  const linkPlaid = async () => {
    const { data } = await axios.get("/api/plaid/get-link-token/1");

    const handler = window.Plaid.create({
      token: data.link_token,
      onSuccess: async (public_token, metadata) => {
        console.log("Plaid linked:", metadata);

        const jwtToken =
          localStorage.getItem("token") ?? sessionStorage.getItem("token");

        const bankstuff = await axios.post(
          "/api/plaid/exchange-public-token",
          { public_token },
          {
            headers: {
              Authorization: `Bearer ${jwtToken}`,
            },
          }
        );

        console.log(bankstuff);
      },
      onExit: (err, metadata) => {
        if (err) console.error("Plaid exited:", err);
        if (metadata) console.log("metadata:", metadata);
      },
    });

    handler.open();
  };

  return { linkPlaid };
}
