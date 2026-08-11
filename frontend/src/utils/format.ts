export const formatCurrency = (value: number, hidden = false) => {
  if (hidden) return '••••••';

  return new Intl.NumberFormat('tr-TR', {
    style: 'currency',
    currency: 'TRY',
    maximumFractionDigits: 2,
  }).format(value);
};

export const formatDate = (value: string) =>
  new Intl.DateTimeFormat('tr-TR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  }).format(new Date(value));

export const maskIban = (iban: string) => `${iban.slice(0, 8)} •••• •••• ${iban.slice(-4)}`;

export const maskCard = (number: string) => `${number.slice(0, 4)} •••• •••• ${number.slice(-4)}`;

export const classNames = (...classes: Array<string | false | null | undefined>) =>
  classes.filter(Boolean).join(' ');
