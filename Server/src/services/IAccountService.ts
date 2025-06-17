import { GeneralResponse, LoginDTO, LoginResponse } from "src/types";

export interface IAccountService {
  logout(userId: number): Promise<GeneralResponse>;
  loginAccount(loginDTO: LoginDTO): Promise<LoginResponse>;
}
