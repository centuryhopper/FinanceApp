import { Router } from "express";
import { authenticateToken } from "../middlewares/auth";
import { AccountService } from "../services/AccountService";
import { logToDb } from "../utils/logger";

const router = Router();
const accountService = new AccountService();

router.get("/test", async (req, res) => {
  await logToDb("info", `Incoming request: ${req.method} ${req.url}`);
  res.status(200).json({
    message: "Logging test completed. Check your PostgreSQL LOGS table.",
  });
});

router.post("/login", async (req, res) => {
  const { email, password, rememberMe } = req.body;

  // console.log(email, password, rememberMe);

  const response = await accountService.loginAccount({
    email,
    password,
    rememberMe,
  });

  if (!response.flag) {
    res.status(400).json(response);
    return;
  }

  res.status(200).json(response);
});

router.post("/logout/:userId", authenticateToken, async (req, res) => {
  const userId = parseInt(req.params.userId, 10);

  if (isNaN(userId)) {
    res.status(400).json({ flag: false, message: "Invalid user ID" });
    return;
  }

  const response = await accountService.logout(userId);

  if (!response.flag) {
    res.status(400).json(response);
    return;
  }
  res.status(200).json(response);
});

export default router;
