import type { Account, CollectApiResponse, LoginResponse, MarketQuote, RegisterRequest, TransactionResponse } from '../types/banking';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5267/api';

async function request<T>(path: string, options: RequestInit = {}, token?: string) {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...options.headers,
      },
    });
  } catch {
    throw new Error('Sunucuya bağlanılamadı. Lütfen backend uygulamasının çalıştığını kontrol edin.');
  }

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    const validationErrors = data?.errors && typeof data.errors === 'object'
      ? Object.values(data.errors as Record<string, string[]>).flat().join(' ')
      : '';
    const message =
      data?.message ??
      data?.Message ??
      (validationErrors || data?.title || (response.status === 401 ? 'E-posta veya şifre hatalı.' : 'İşlem tamamlanamadı.'));
    throw new Error(message);
  }

  return data as T;
}

export const api = {
  login: (email: string, password: string) =>
    request<LoginResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),

  register: (payload: RegisterRequest) =>
    request<LoginResponse>('/auth/register', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  getMyAccounts: (token: string) => request<Account[]>('/accounts/my', {}, token),

  getGoldPrices: (token: string) => request<CollectApiResponse<MarketQuote>>('/market/gold', {}, token),

  getCurrencyRates: (token: string) => request<CollectApiResponse<MarketQuote>>('/market/currency', {}, token),

  getBistValues: (token: string) => request<CollectApiResponse<MarketQuote>>('/market/bist', {}, token),

  createAccount: (token: string, accountType: number, currency = 1) =>
    request<Account>(
      '/accounts',
      {
        method: 'POST',
        body: JSON.stringify({ accountType, currency }),
      },
      token,
    ),

  deposit: (token: string, accountId: number, amount: number, description?: string) =>
    request<TransactionResponse>(
      '/transactions/deposit',
      {
        method: 'POST',
        body: JSON.stringify({ accountId, amount, description }),
      },
      token,
    ),

  withdraw: (token: string, accountId: number, amount: number, description?: string) =>
    request<TransactionResponse>(
      '/transactions/withdraw',
      {
        method: 'POST',
        body: JSON.stringify({ accountId, amount, description }),
      },
      token,
    ),

  transfer: (token: string, senderAccountId: number, receiverIban: string, amount: number, description?: string) =>
    request<TransactionResponse>(
      '/transactions/transfer',
      {
        method: 'POST',
        body: JSON.stringify({ senderAccountId, receiverIban, amount, description }),
      },
      token,
    ),
};
