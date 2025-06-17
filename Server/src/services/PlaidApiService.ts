import dotenv from "dotenv";
import moment from "moment";
import {
  Configuration,
  CountryCode,
  PlaidApi,
  PlaidEnvironments,
  Products,
} from "plaid";

dotenv.config();

class PlaidApiService {
  private client: PlaidApi;

  constructor() {
    const env = process.env.PLAID_ENV || "sandbox";

    const config = new Configuration({
      basePath: PlaidEnvironments.production,
      baseOptions: {
        headers: {
          "PLAID-CLIENT-ID": process.env.PLAID_CLIENT_ID!,
          "PLAID-SECRET": process.env.PLAID_SECRET!,
        },
      },
    });

    this.client = new PlaidApi(config);
  }

  async getPast1YearsTransactions(accessToken: string) {
    // const response = await this.client.transactionsSync({
    //     access_token: accessToken
    // });

    // return response.data;

    const start_date = moment().subtract(1, "year").format("YYYY-MM-DD");
    const end_date = moment().format("YYYY-MM-DD");
    try {
      const response = await this.client.transactionsGet({
        access_token: accessToken,
        start_date,
        end_date,
      });

      const transactions = response.data.transactions;
      console.log(
        `You have ${transactions.length} transactions from ${start_date} to ${end_date}.`
      );

      return transactions;
    } catch (err: any) {
      if (err.response?.data) {
        console.error("Plaid API error:", err.response.data);
      } else {
        console.error("Unknown error occurred:", err);
      }
      throw err; // rethrow so caller can handle it
    }
  }

  async getTransactions(accessToken: string) {
    let allTransactions: any[] = [];
    let cursor: string | undefined = undefined;
    let hasMore = true;

    const accountsResponse = await this.client.accountsGet({
      access_token: accessToken,
    });
    console.log(
      "accountsResponse.data.accounts",
      accountsResponse.data.accounts
    );

    while (hasMore) {
      const response = await this.client.transactionsSync({
        access_token: accessToken,
        cursor, // will be null on first call
      });

      const { accounts, next_cursor, has_more } = response.data;

      allTransactions.push(...accounts);
      cursor = next_cursor;
      hasMore = has_more;
    }

    return allTransactions;
  }

  async createLinkToken(userId: string) {
    try {
      const response = await this.client.linkTokenCreate({
        user: { client_user_id: userId },
        client_name: "Finance App",
        products: [Products.Auth, Products.Transactions],
        country_codes: [CountryCode.Us],
        language: "en",
        redirect_uri: undefined, // optional
      });

      return response.data;
    } catch (err: any) {
      throw new Error(
        `Failed to create link token: ${
          err.response?.data?.error_message || err.message
        }`
      );
    }
  }

  async exchangePublicToken(publicToken: string) {
    try {
      const response = await this.client.itemPublicTokenExchange({
        public_token: publicToken,
      });
      return response.data;
    } catch (err: any) {
      throw new Error(
        `Failed to exchange public token: ${
          err.response?.data?.error_message || err.message
        }`
      );
    }
  }

  async getAuthInfo(accessToken: string) {
    try {
      const response = await this.client.authGet({ access_token: accessToken });
      return response.data;
    } catch (err: any) {
      throw new Error(
        `Failed to retrieve auth info: ${
          err.response?.data?.error_message || err.message
        }`
      );
    }
  }
}

export default PlaidApiService;
