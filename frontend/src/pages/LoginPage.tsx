import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { LockKeyhole, UserPlus } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { api } from '../services/api';
import { useToast } from '../hooks/useToast';
import { useAuth } from '../hooks/useAuth';

const loginSchema = z.object({
  email: z.string().email('Geçerli bir e-posta girin.'),
  password: z.string().min(6, 'Şifre en az 6 karakter olmalı.'),
});

const registerSchema = loginSchema.extend({
  confirmPassword: z.string().min(6, 'Şifre tekrarı zorunlu.'),
  firstName: z.string().min(2, 'Ad en az 2 karakter olmalı.'),
  lastName: z.string().min(2, 'Soyad en az 2 karakter olmalı.'),
  identityNumber: z.string().regex(/^\d{11}$/, 'TC Kimlik No 11 haneli olmalı.'),
  phoneNumber: z.string().min(10, 'Telefon numarası girin.'),
  address: z.string().optional(),
}).refine((value) => value.password === value.confirmPassword, {
  path: ['confirmPassword'],
  message: 'Şifreler eşleşmiyor.',
});

type LoginForm = z.infer<typeof loginSchema>;
type RegisterForm = z.infer<typeof registerSchema>;

export function LoginPage() {
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { notify } = useToast();
  const { setAuth } = useAuth();

  const login = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  });

  const register = useForm<RegisterForm>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      email: '',
      password: '',
      confirmPassword: '',
      firstName: '',
      lastName: '',
      identityNumber: '',
      phoneNumber: '',
      address: '',
    },
  });

  const submitLogin = login.handleSubmit(async (values) => {
    setError('');
    try {
      const response = await api.login(values.email, values.password);
      if (!response.isSuccess) throw new Error(response.message);
      setAuth(response);
      notify(`Hoş geldiniz, ${[response.firstName, response.lastName].filter(Boolean).join(' ') || response.email}.`, 'success');
      navigate('/dashboard');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Giriş başarısız.');
      notify(err instanceof Error ? err.message : 'Giriş başarısız.', 'error');
    }
  });

  const submitRegister = register.handleSubmit(async (values) => {
    setError('');
    try {
      const response = await api.register({ ...values, address: values.address ?? '' });
      if (!response.isSuccess) throw new Error(response.message);
      setAuth(response);
      notify('Kayıt oluşturuldu ve giriş yapıldı.', 'success');
      navigate('/dashboard');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Kayıt başarısız.');
    }
  });

  return (
    <main className="login-page">
      <section className="login-copy">
        <div className="brand large">
          <div className="brand-mark">DB</div>
          <div>
            <strong>Dijital Banka</strong>
            <span>Güvenli dijital bankacılık</span>
          </div>
        </div>
        <h1>Hesaplarınızı sade, hızlı ve güvenli şekilde yönetin.</h1>
        <p>Giriş yapın, hesaplarınızı görüntüleyin ve para transferlerinizi birkaç adımda tamamlayın.</p>
      </section>
      <Card className="login-card">
        <div className="tabs">
          <button className={mode === 'login' ? 'active' : ''} onClick={() => setMode('login')}>Giriş</button>
          <button className={mode === 'register' ? 'active' : ''} onClick={() => setMode('register')}>Kayıt</button>
        </div>
        {mode === 'login' ? (
          <form onSubmit={submitLogin}>
            <h2>Giriş yap</h2>
            <Input label="E-posta" type="email" {...login.register('email')} error={login.formState.errors.email?.message} />
            <Input label="Şifre" type="password" {...login.register('password')} error={login.formState.errors.password?.message} />
            {error && <div className="form-alert">{error}</div>}
            <Button type="submit" className="full" isLoading={login.formState.isSubmitting} icon={<LockKeyhole size={18} />}>
              Giriş yap
            </Button>
          </form>
        ) : (
          <form onSubmit={submitRegister}>
            <h2>Kayıt oluştur</h2>
            <div className="two-col">
              <Input label="Ad" {...register.register('firstName')} error={register.formState.errors.firstName?.message} />
              <Input label="Soyad" {...register.register('lastName')} error={register.formState.errors.lastName?.message} />
            </div>
            <Input label="E-posta" type="email" {...register.register('email')} error={register.formState.errors.email?.message} />
            <div className="two-col">
              <Input label="Şifre" type="password" {...register.register('password')} error={register.formState.errors.password?.message} />
              <Input label="Şifre tekrar" type="password" {...register.register('confirmPassword')} error={register.formState.errors.confirmPassword?.message} />
            </div>
            <Input label="TC Kimlik No" inputMode="numeric" maxLength={11} {...register.register('identityNumber')} error={register.formState.errors.identityNumber?.message} />
            <Input label="Telefon" {...register.register('phoneNumber')} error={register.formState.errors.phoneNumber?.message} />
            <Input label="Adres" {...register.register('address')} error={register.formState.errors.address?.message} />
            {error && <div className="form-alert">{error}</div>}
            <Button type="submit" className="full" isLoading={register.formState.isSubmitting} icon={<UserPlus size={18} />}>
              Kayıt ol
            </Button>
          </form>
        )}
      </Card>
    </main>
  );
}
