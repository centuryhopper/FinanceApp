export type HandyLoginModel = {
  email: string;
  password: string;
  rememberMe: boolean;
}

export type HandyLoginResponse = {
  flag: boolean;
  message: string;
}

export type LoginPageProps = {
  cooldownSeconds?: number;
  maxAttempts?: number;
  noticeText?: string;
  redirectLink?: string;
  loginCallback: (model: HandyLoginModel) => Promise<HandyLoginResponse>;
}