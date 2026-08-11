import { useCallback, useEffect, useState } from 'react';
import { Copy, Plus } from 'lucide-react';
import { Badge } from '../components/ui/Badge';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { EmptyState } from '../components/ui/EmptyState';
import { Select } from '../components/ui/Select';
import { Skeleton } from '../components/ui/Skeleton';
import { Table } from '../components/ui/Table';
import { api } from '../services/api';
import type { Account } from '../types/banking';
import { useAuth } from '../hooks/useAuth';
import { useToast } from '../hooks/useToast';
import { formatCurrency, maskIban } from '../utils/format';

const accountTypeLabels: Record<string, string> = {
  Checking: 'Vadesiz',
  Savings: 'Birikim',
  Investment: 'Yatırım',
};

export function AccountsPage() {
  const { token } = useAuth();
  const { notify } = useToast();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [accountType, setAccountType] = useState('1');
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);

  const loadAccounts = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    try {
      setAccounts(await api.getMyAccounts(token));
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    void loadAccounts();
  }, [loadAccounts]);

  const createAccount = async () => {
    if (!token) return;
    setCreating(true);
    try {
      await api.createAccount(token, Number(accountType));
      notify('Hesap oluşturuldu.', 'success');
      await loadAccounts();
    } catch (err) {
      notify(err instanceof Error ? err.message : 'Hesap oluşturulamadı.', 'error');
    } finally {
      setCreating(false);
    }
  };

  return (
    <div className="page">
      <div className="page-heading">
        <div>
          <span className="eyebrow">Hesaplar</span>
          <h1>Hesaplarım</h1>
        </div>
      </div>
      <Card>
        <div className="card-header">
          <h2>Yeni hesap aç</h2>
          <div className="filters compact">
            <Select
              label="Hesap türü"
              value={accountType}
              onChange={(event) => setAccountType(event.target.value)}
              options={[
                { value: '1', label: 'Vadesiz' },
                { value: '2', label: 'Birikim' },
                { value: '3', label: 'Yatırım' },
              ]}
            />
            <Button isLoading={creating} icon={<Plus size={18} />} onClick={createAccount}>Hesap aç</Button>
          </div>
        </div>
      </Card>
      <Card>
        <h2>Hesap listesi</h2>
        {loading ? (
          <Skeleton lines={5} />
        ) : accounts.length ? (
          <Table
            headers={['IBAN', 'Tür', 'Para birimi', 'Bakiye', '']}
            rows={accounts.map((account) => [
              maskIban(account.iban),
              <Badge>{accountTypeLabels[account.accountType] ?? account.accountType}</Badge>,
              account.currency,
              <strong>{formatCurrency(account.balance)}</strong>,
              <Button
                variant="ghost"
                icon={<Copy size={17} />}
                aria-label="IBAN kopyala"
                onClick={() => {
                  void navigator.clipboard?.writeText(account.iban);
                  notify('IBAN kopyalandı.', 'success');
                }}
              />,
            ])}
            mobileCards={accounts.map((account) => (
              <div className="mobile-card" key={account.id}>
                <strong>{accountTypeLabels[account.accountType] ?? account.accountType}</strong>
                <span>{maskIban(account.iban)} · {account.currency}</span>
                <b>{formatCurrency(account.balance)}</b>
              </div>
            ))}
          />
        ) : (
          <EmptyState title="Hesap bulunamadı" text="Yeni hesap aç butonuyla ilk hesabınızı oluşturabilirsiniz." />
        )}
      </Card>
    </div>
  );
}
