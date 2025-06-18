import { UserDTO } from "./index";

declare global {
  namespace Express {
    interface Request {
      user?: UserDTO;
    }
  }
}
