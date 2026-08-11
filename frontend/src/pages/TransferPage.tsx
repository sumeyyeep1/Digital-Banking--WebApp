import { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { CheckCircle2, Send, XCircle } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { api } from '../services/api';
import type { Account } from '../types/banking';
import { formatCurrency, maskIban } from '../utils/format';
import { useAuth } from '../hooks/useAuth';

const schema = z.object({
  senderAccountId: z.coerce.number().min(1, 'Kaynak hesap seçin.'),
  receiverIban: z.string().regex(/^TR\d{16,24}$/, 'TR ile başlayan geçerli IBAN girin.'),
  amount: z.coerce.number().positive('Tutar 0’dan büyük olmalı.').max(1_000_000, 'En fazla 1.000.000 TRY gönderilebilir.'),
  description: z.string().max(120, 'Açıklama en fazla 120 karakter olabilir.').optional(),
});

type TransferForm = z.infer<typeof schema>;

export function TransferPage() {
  const { token } = useAuth();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [step, setStep] = useState<'form' | 'confirm' | 'success' | 'error'>('form');
  const [message, setMessage] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const form = useForm<TransferForm>({
    resolver: zodResolver(schema),
    defaultValues: { senderAccountId: 0, receiverIban: '', amount: 100, description: '' },
  });
  const values = form.watch();
  const source = useMemo(() => accounts.find((account) => account.id === Number(values.senderAccountId)), [accounts, values.senderAccountId]);

  useEffect(() => {
    if (!token) return;
    api.getMyAccounts(token).then((items) => {
      setAccounts(items);
      if (items[0]) form.setValue('senderAccountId', items[0].id);
    });
  }, [form, token]);

  const prepare = form.handleSubmit(() => setStep('confirm'));

  const confirm = async () => {
    if (!token) return;
    setSubmitting(true);
    try {
      const result = await api.transfer(token, Number(values.senderAccountId), values.receiverIban, Number(values.amount), values.description);
      setMessage(`${result.message} Güncel bakiye: ${formatCurrency(result.currentBalance ?? 0)}`);
      setStep('success');
    } catch (err) {
      setMessage(err instanceof Error ? err.message : 'Transfer başarısız.');
      setStep('error');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="page">
      <div className="page-heading">
        <div>
          <span className="eyebrow">Para transferi</span>
          <h1>Para gönder</h1>
        </div>
      </div>
      {step === 'success' || step === 'error' ? (
        <Card className="result-card">
          {step === 'success' ? <CheckCircle2 className="result-success" size={48} /> : <XCircle className="result-error" size={48} />}
          <h2>{step === 'success' ? 'Transfer başarılı' : 'Transfer başarısız'}</h2>
          <p>{message}</p>
          <Button onClick={() => setStep('form')}>Yeni işlem başlat</Button>
        </Card>
      ) : (
        <section className="form-grid single">
          <Card>
            {step === 'form' ? (
              <form onSubmit={prepare} className="stack">
                <Select
                  label="Kaynak hesap"
                  {...form.register('senderAccountId')}
                  options={accounts.map((account) => ({ value: String(account.id), label: `${account.accountType} · ${formatCurrency(account.balance)} · ${maskIban(account.iban)}` }))}
                  error={form.formState.errors.senderAccountId?.message}
                />
                <Input label="Alıcı IBAN" {...form.register('receiverIban')} error={form.formState.errors.receiverIban?.message} />
                <Input label="Tutar" type="number" step="0.01" {...form.register('amount')} error={form.formState.errors.amount?.message} />
                <Input label="Açıklama" {...form.register('description')} error={form.formState.errors.description?.message} />
                {source && Number(values.amount) > source.balance && <div className="form-alert">Seçilen hesap bakiyesi bu tutar için yeterli değil.</div>}
                <Button type="submit" icon={<Send size={18} />} disabled={!source || Number(values.amount) > source.balance}>
                  Özete geç
                </Button>
              </form>
            ) : (
              <div className="stack">
                <h2>İşlem özeti</h2>
                <div className="summary-row"><span>Kaynak hesap</span><strong>{source ? maskIban(source.iban) : '-'}</strong></div>
                <div className="summary-row"><span>Alıcı IBAN</span><strong>{maskIban(values.receiverIban)}</strong></div>
                <div className="summary-row"><span>Tutar</span><strong>{formatCurrency(Number(values.amount))}</strong></div>
                <div className="button-row">
                  <Button variant="secondary" onClick={() => setStep('form')}>Düzenle</Button>
                  <Button isLoading={submitting} onClick={confirm}>Transferi onayla</Button>
                </div>
              </div>
            )}
          </Card>
        </section>
      )}
    </div>
  );
}
