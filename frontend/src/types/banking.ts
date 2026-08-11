export type Theme = 'light' | 'dark';

export interface Account {
  id: number;
  iban: string;
  accountType: string;
  currency: string;
  balance: number;
}

export interface LoginResponse {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  token: string;
  tokenExpiry: string;
  isSuccess: boolean;
  message: string;
}

export interface TransactionResponse {
  isSuccess: boolean;
  message: string;
  transactionId?: number;
  currentBalance?: number;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  identityNumber: string;
  phoneNumber: string;
  address: string;
}
