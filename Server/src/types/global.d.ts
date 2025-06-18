import { JwtPayload } from "jsonwebtoken";
import { UserDTO } from "./index";

declare global {
  namespace Express {
    interface Request {
      payload?: JwtPayload;
    }
  }
}
