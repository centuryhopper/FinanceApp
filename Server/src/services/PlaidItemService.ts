import { GeneralResponse, PlaidItemDTO } from "../types";
import { db } from "../utils/db";
import { IPlaidItemService } from "./IPlaidItemService";

export class PlaidItemService implements IPlaidItemService {
  async storePlaidItemAsync(dto?: PlaidItemDTO): Promise<GeneralResponse> {
    if (!dto) {
      return {
        flag: false,
        message: "dto cannot be null",
      };
    }

    const record = await db.query(
      "SELECT * from plaiditems where userid = $1 AND institutionname = $2",
      [dto.userid, dto.institutionname]
    );

    if (record.rows.length !== 0) {
      return {
        flag: false,
        message: `Plaid item already exists for ${dto.institutionname}!`,
      };
    }

    try {
      await db.query(
        `INSERT INTO plaiditems (userid, accesstoken, institutionname, datelinked)
    VALUES (
        $1, $2, $3, $4
    )
        `,
        [dto.userid, dto.accesstoken, dto.institutionname, dto.datelinked]
      );

      return { flag: true, message: "Plaid item added" };
    } catch (e) {
      return { flag: false, message: e as string };
    }
  }
}
