import { useEffect, useMemo, useState } from 'react';
import { Eye, EyeOff, Landmark, Send, Wallet } from 'lucide-react';
import { Link } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Skeleton } from '../components/ui/Skeleton';
import { EmptyState } from '../components/ui/EmptyState';
import { api } from '../services/api';
import type { Account } from '../types/banking';
import { formatCurrency, maskIban } from '../utils/format';
import { useAuth } from '../hooks/useAuth';

export function DashboardPage() {
  const { auth, token } = useAuth();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(true);
  const [hidden, setHidden] = useState(false);
  const displayName = [auth?.firstName, auth?.lastName].filter(Boolean).join(' ') || auth?.email;

  useEffect(() => {
    if (!token) return;
    api.getMyAccounts(token).then(setAccounts).finally(() => setLoading(false));
  }, [token]);

  const totalBalance = useMemo(() => accounts.reduce((sum, account) => sum + account.balance, 0), [accounts]);

  return (
    <div className="page">
      <div className="page-heading">
        <div>
          <span className="eyebrow">Genel bakış</span>
          <h1>Merhaba, {displayName}</h1>
        </div>
        <Button variant="secondary" icon={hidden ? <Eye size={18} /> : <EyeOff size={18} />} onClick={() => setHidden((value) => !value)}>
          {hidden ? 'Bakiyeyi göster' : 'Bakiyeyi gizle'}
        </Button>
      </div>
      <section className="metric-grid">
        <Card>
          <span className="metric-label">Toplam bakiye</span>
          <strong className="metric-value">{formatCurrency(totalBalance, hidden)}</strong>
          <p className="muted">Aktif hesaplarınızdaki toplam tutar</p>
        </Card>
        <Card>
          <span className="metric-label">Hesap sayısı</span>
          <strong className="metric-value">{loading ? '...' : accounts.length}</strong>
          <p className="muted">Aktif hesaplarım</p>
        </Card>
        <Card>
          <span className="metric-label">Rol</span>
          <strong className="metric-value small">{auth?.role}</strong>
          <p className="muted">Hesap erişim türünüz</p>
        </Card>
      </section>
      <section className="dashboard-grid">
        <Card>
          <h2>Hesaplarım</h2>
          {loading ? (
            <Skeleton lines={4} />
          ) : accounts.length ? (
            <div className="stack">
              {accounts.map((account) => (
                <div className="compact-row" key={account.id}>
                  <div>
                    <strong>{account.accountType}</strong>
                    <span>{maskIban(account.iban)}</span>
                  </div>
                  <b>{formatCurrency(account.balance, hidden)}</b>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState title="Henüz hesap yok" text="Yeni hesap oluşturarak bankacılık işlemlerine başlayabilirsiniz." />
          )}
        </Card>
        <Card>
          <h2>Desteklenen işlemler</h2>
          <div className="quick-grid">
            <Link to="/accounts" className="quick-action"><Landmark size={22} /><span>Hesap aç</span></Link>
            <Link to="/transfer" className="quick-action"><Send size={22} /><span>Transfer</span></Link>
            <Link to="/transactions" className="quick-action"><Wallet size={22} /><span>Yatır / çek</span></Link>
          </div>
        </Card>
      </section>
    </div>
  );
}
