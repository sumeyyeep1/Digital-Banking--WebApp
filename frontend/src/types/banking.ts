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

export interface Card {
  id: number;
  maskedCardNumber: string;
  cardHolderName: string;
  expiryMonth: string;
  expiryYear: string;
  cardType: string;
  accountId: number;
  accountIban: string;
}

export interface CardOperationResponse {
  isSuccess: boolean;
  message: string;
  cardId?: number;
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

export interface MarketQuote {
  name?: string;
  code?: string;
  text?: string;
  buying?: string | number;
  buyingstr?: string;
  selling?: string | number;
  sellingstr?: string;
  price?: string | number;
  lastprice?: string | number;
  lastpricestr?: string;
  current?: string | number;
  currentstr?: string;
  rate?: string | number;
  change?: string | number;
  value?: string | number;
  date?: string;
  time?: string;
}

export interface CollectApiResponse<T> {
  success: boolean;
  result?: T[];
  message?: string;
}
