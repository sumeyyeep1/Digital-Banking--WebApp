import { useCallback, useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Banknote, MinusCircle } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { api } from '../services/api';
import type { Account } from '../types/banking';
import { useAuth } from '../hooks/useAuth';
import { useToast } from '../hooks/useToast';
import { formatCurrency, maskIban } from '../utils/format';

const schema = z.object({
  accountId: z.coerce.number().min(1, 'Hesap seçin.'),
  amount: z.coerce.number().positive('Tutar 0’dan büyük olmalı.').max(1_000_000),
  description: z.string().max(120).optional(),
});

type MoneyForm = z.infer<typeof schema>;

export function TransactionsPage() {
  const { token } = useAuth();
  const { notify } = useToast();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(false);
  const [resultMessage, setResultMessage] = useState('');
  const form = useForm<MoneyForm>({
    resolver: zodResolver(schema),
    defaultValues: { accountId: 0, amount: 100, description: '' },
  });

  const refresh = useCallback(async () => {
    if (!token) return;
    const items = await api.getMyAccounts(token);
    setAccounts(items);
    if (items[0] && !form.getValues('accountId')) form.setValue('accountId', items[0].id);
  }, [form, token]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const submit = (kind: 'deposit' | 'withdraw') =>
    form.handleSubmit(async (values) => {
      if (!token) return;
      setLoading(true);
      try {
        const result =
          kind === 'deposit'
            ? await api.deposit(token, values.accountId, values.amount, values.description)
            : await api.withdraw(token, values.accountId, values.amount, values.description);
        const message = `${result.message} Güncel bakiye: ${formatCurrency(result.currentBalance ?? 0)}`;
        setResultMessage(message);
        notify(message, 'success');
        await refresh();
      } catch (err) {
        notify(err instanceof Error ? err.message : 'İşlem başarısız.', 'error');
      } finally {
        setLoading(false);
      }
    });

  return (
    <div className="page">
      <div className="page-heading">
        <div>
          <span className="eyebrow">Para yatır / çek</span>
          <h1>Para yatır / çek</h1>
        </div>
      </div>
      <section className="form-grid single">
        <Card>
          <form className="stack">
            <Select
              label="Hesap"
              {...form.register('accountId')}
              options={accounts.map((account) => ({ value: String(account.id), label: `${account.accountType} · ${formatCurrency(account.balance)} · ${maskIban(account.iban)}` }))}
              error={form.formState.errors.accountId?.message}
            />
            <Input label="Tutar" type="number" step="0.01" {...form.register('amount')} error={form.formState.errors.amount?.message} />
            <Input label="Açıklama" {...form.register('description')} error={form.formState.errors.description?.message} />
            <div className="button-row">
              <Button type="button" isLoading={loading} icon={<Banknote size={18} />} onClick={submit('deposit')}>Para yatır</Button>
              <Button type="button" variant="secondary" isLoading={loading} icon={<MinusCircle size={18} />} onClick={submit('withdraw')}>Para çek</Button>
            </div>
            {resultMessage && <div className="success-panel">{resultMessage}</div>}
          </form>
        </Card>
      </section>
    </div>
  );
}
