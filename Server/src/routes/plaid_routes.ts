import { Router } from "express";
import { AuthGetResponse } from "plaid";
import { authenticateToken } from "../middlewares/auth";
import PlaidApiService from "../services/PlaidApiService";
import { PlaidItemService } from "../services/PlaidItemService";
import { logToDb } from "../utils/logger";

const router = Router();

const plaidApiService = new PlaidApiService();
const plaidItemService = new PlaidItemService();

router.get("/test", async (req, res) => {
  await logToDb("info", `Incoming request: ${req.method} ${req.url}`);
  res.status(200).json({
    message: "Logging test completed. Check your PostgreSQL LOGS table.",
  });
});

router.get("/get-link-token/:userId", async (req, res) => {
  const token = await plaidApiService.createLinkToken(req.params.userId);
  res.status(200).json(token);
});

router.post("/exchange-public-token", authenticateToken, async (req, res) => {
  const exchangedResponse = await plaidApiService.exchangePublicToken(
    req.body.public_token
  );

  // console.log('req.user', req.user);
  const userId = parseInt((req.user?.sub as string) || "0", 10);

  // console.log(userId);

  const authInfo: AuthGetResponse = await plaidApiService.getAuthInfo(
    exchangedResponse.access_token
  );

  const bankInfo = authInfo.accounts.map((acc) => ({
    balances: acc.balances,
    name: acc.name,
    official_name: acc.official_name,
    available: acc.balances.available,
    current: acc.balances.current,
  }));

  const response = await plaidItemService.storePlaidItemAsync({
    userid: userId,
    accesstoken: exchangedResponse.access_token,
    institutionname: authInfo.item.institution_name,
    datelinked: new Date(),
  });

  // console.log("response: " + JSON.stringify(response));

  // console.log(exchangedResponse.access_token);

  const transactionsData = await plaidApiService.pollTransactions(
    exchangedResponse.access_token
  );

  // console.log("transactionsData", transactionsData);

  res.status(200).json({
    transactions: transactionsData,
    bankInfo,
    institutionname: authInfo.item.institution_name,
  });
});

export default router;
