import { GeneralResponse, PlaidItemDTO } from "src/types";

export interface IPlaidItemService {
  storePlaidItemAsync(dto?: PlaidItemDTO): Promise<GeneralResponse>;
}
