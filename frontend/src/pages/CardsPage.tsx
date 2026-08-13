import { useCallback, useEffect, useMemo, useState } from 'react';
import { CreditCard, Plus } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { Card as Panel } from '../components/ui/Card';
import { EmptyState } from '../components/ui/EmptyState';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { Skeleton } from '../components/ui/Skeleton';
import { api } from '../services/api';
import type { Account, Card } from '../types/banking';
import { useAuth } from '../hooks/useAuth';
import { useToast } from '../hooks/useToast';
import { maskIban } from '../utils/format';

const cardTypeLabels: Record<string, string> = {
  Debit: 'Banka kartı',
  Credit: 'Kredi kartı',
};

export function CardsPage() {
  const { auth, token } = useAuth();
  const { notify } = useToast();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [cards, setCards] = useState<Card[]>([]);
  const [accountId, setAccountId] = useState('');
  const [cardType, setCardType] = useState('1');
  const [cardHolderName, setCardHolderName] = useState(() => [auth?.firstName, auth?.lastName].filter(Boolean).join(' '));
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);

  const accountOptions = useMemo(
    () => accounts.map((account) => ({ value: String(account.id), label: `${account.accountType} - ${maskIban(account.iban)}` })),
    [accounts],
  );

  const loadCards = useCallback(async () => {
    if (!token) return;

    setLoading(true);
    try {
      const [accountItems, cardItems] = await Promise.all([api.getMyAccounts(token), api.getMyCards(token)]);
      setAccounts(accountItems);
      setCards(cardItems);
      if (accountItems[0] && !accountId) setAccountId(String(accountItems[0].id));
    } catch (err) {
      notify(err instanceof Error ? err.message : 'Kart bilgileri alınamadı.', 'error');
    } finally {
      setLoading(false);
    }
  }, [accountId, notify, token]);

  useEffect(() => {
    void loadCards();
  }, [loadCards]);

  const createCard = async () => {
    if (!token || !accountId) return;

    setCreating(true);
    try {
      const result = await api.createCard(token, Number(accountId), cardHolderName, Number(cardType));
      notify(result.message, 'success');
      await loadCards();
    } catch (err) {
      notify(err instanceof Error ? err.message : 'Kart oluşturulamadı.', 'error');
    } finally {
      setCreating(false);
    }
  };

  return (
    <div className="page">
      <div className="page-heading">
        <div>
          <span className="eyebrow">Kartlar</span>
          <h1>Kartlarım</h1>
        </div>
      </div>
      <section className="form-grid">
        <Panel>
          <div className="stack">
            <h2>Yeni kart oluştur</h2>
            <Select label="Bağlı hesap" value={accountId} onChange={(event) => setAccountId(event.target.value)} options={accountOptions} />
            <Input label="Kart üzerindeki ad" value={cardHolderName} onChange={(event) => setCardHolderName(event.target.value)} />
            <Select
              label="Kart tipi"
              value={cardType}
              onChange={(event) => setCardType(event.target.value)}
              options={[
                { value: '1', label: 'Banka kartı' },
                { value: '2', label: 'Kredi kartı' },
              ]}
            />
            <Button isLoading={creating} disabled={!accountId || !cardHolderName.trim()} icon={<Plus size={18} />} onClick={createCard}>
              Kart oluştur
            </Button>
          </div>
        </Panel>
        <Panel>
          <h2>Kart listesi</h2>
          {loading ? (
            <Skeleton lines={5} />
          ) : cards.length ? (
            <div className="card-grid">
              {cards.map((card) => (
                <div className="virtual-card" key={card.id}>
                  <div>
                    <span>{cardTypeLabels[card.cardType] ?? card.cardType}</span>
                    <CreditCard size={24} />
                  </div>
                  <strong>{card.maskedCardNumber}</strong>
                  <div>
                    <span>{card.cardHolderName}</span>
                    <span>{card.expiryMonth}/{card.expiryYear}</span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState title="Kart bulunamadı" text="Aktif hesabınız için yeni kart oluşturabilirsiniz." />
          )}
        </Panel>
      </section>
    </div>
  );
}
