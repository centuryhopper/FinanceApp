import { GeneralResponse, PlaidItemDTO } from "../types";

export interface IPlaidItemService {
  storePlaidItemAsync(dto?: PlaidItemDTO): Promise<GeneralResponse>;
}
