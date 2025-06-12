

import LoginComponent from './LoginComponent';

export default function Login() {
  return (
    <>
      <div className="mt-5"></div>
      <LoginComponent noticeText="Please login to view your information. To request that your account be deleted, please email me." loginCallback={async (model) => {
        // console.log(model.email);
        // console.log(model.password);
        // console.log(model.rememberMe);
        return {
          flag: false,
          message: "fail!",
        }
      }}/>
    </>
  )
}
