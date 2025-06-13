import axios from "axios";
import LoginComponent from "./LoginComponent";

export default function Login() {
  return (
    <>
      <div className="p-5 m-5">
        <LoginComponent
          noticeText="Please login to view your information. To request that your account be deleted, please email me."
          loginCallback={async (model) => {
            // console.log(model.email);
            // console.log(model.password);
            // console.log(model.rememberMe);
            let response;
            try {
              response = await axios.post("/api/Account/login", {
                email: model.email,
                password: model.password,
                rememberMe: model.rememberMe,
              });

              // console.log(response.data);
              return {
                flag: response.data.flag,
                message: response.data.message,
              };
            } catch (e: unknown) {
              let msg = "An unknown error occurred";

              if (axios.isAxiosError(e) && e.response) {
                console.log(e.response.status);

                switch (e.response.status) {
                  case 400:
                    msg = "Invalid email/password";
                    break;
                  default:
                    msg =
                      e.response.data?.message || "Unexpected error occurred";
                    break;
                }
              } else {
                console.log("Unknown error:", e);
                msg = String(e);
              }
              return {
                flag: false,
                message: msg,
              };
            }
          }}
        />
      </div>
    </>
  );
}
