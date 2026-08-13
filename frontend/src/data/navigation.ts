import { Banknote, CreditCard, Gauge, Landmark, LogIn, Send } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';

export interface NavItem {
  label: string;
  path: string;
  icon: LucideIcon;
}

export const navItems: NavItem[] = [
  { label: 'Genel bakış', path: '/dashboard', icon: Gauge },
  { label: 'Hesaplar', path: '/accounts', icon: Landmark },
  { label: 'Kartlar', path: '/cards', icon: CreditCard },
  { label: 'Para transferi', path: '/transfer', icon: Send },
  { label: 'Para yatır / çek', path: '/transactions', icon: Banknote },
];

export const quickActions = [
  { label: 'Para gönder', icon: Send, path: '/transfer' },
  { label: 'Para yatır', icon: Banknote, path: '/transactions' },
  { label: 'Hesap aç', icon: Landmark, path: '/accounts' },
  { label: 'Giriş bilgileri', icon: LogIn, path: '/' },
];
