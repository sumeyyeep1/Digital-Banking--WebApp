import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { Bell, ChevronLeft, LogOut, Menu, Moon, Search, Sun, UserRound, X } from 'lucide-react';
import { useState } from 'react';
import { navItems } from '../data/navigation';
import { useTheme } from '../hooks/useTheme';
import { Button } from '../components/ui/Button';
import { classNames } from '../utils/format';
import { useAuth } from '../hooks/useAuth';
import { useToast } from '../hooks/useToast';

export function AppLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const { theme, toggleTheme } = useTheme();
  const { auth, logout } = useAuth();
  const { notify } = useToast();
  const navigate = useNavigate();
  const displayName = [auth?.firstName, auth?.lastName].filter(Boolean).join(' ') || auth?.email;

  const handleLogout = () => {
    notify('Çıkış yapıldı.', 'success');
    logout();
    navigate('/');
  };

  const sidebar = (
    <aside className={classNames('sidebar', collapsed && 'sidebar-collapsed')}>
      <div className="brand">
        <div className="brand-mark" aria-hidden="true">
          DB
        </div>
        {!collapsed && (
          <div>
            <strong>Dijital Banka</strong>
            <span>Güvenli bankacılık</span>
          </div>
        )}
      </div>
      <nav className="nav" aria-label="Ana menü">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              to={item.path}
              key={item.path}
              className={({ isActive }) => classNames('nav-link', isActive && 'active')}
              onClick={() => setDrawerOpen(false)}
              title={collapsed ? item.label : undefined}
            >
              <Icon size={20} aria-hidden="true" />
              {!collapsed && <span>{item.label}</span>}
            </NavLink>
          );
        })}
      </nav>
      <div className="sidebar-footer">
        <Button
          variant="ghost"
          icon={<ChevronLeft size={18} />}
          onClick={() => setCollapsed((value) => !value)}
          aria-label={collapsed ? 'Menüyü genişlet' : 'Menüyü daralt'}
        >
          {!collapsed && 'Daralt'}
        </Button>
      </div>
    </aside>
  );

  return (
    <div className="app-shell">
      <div className="desktop-sidebar">{sidebar}</div>
      {drawerOpen && (
        <div className="drawer-layer" role="presentation" onMouseDown={() => setDrawerOpen(false)}>
          <div onMouseDown={(event) => event.stopPropagation()}>{sidebar}</div>
        </div>
      )}
      <div className="main-shell">
        <header className="topbar">
          <Button variant="ghost" className="mobile-only" aria-label="Menüyü aç" icon={<Menu size={20} />} onClick={() => setDrawerOpen(true)} />
          <div className="search-box">
            <Search size={18} aria-hidden="true" />
            <input aria-label="Uygulamada ara" placeholder="İşlem, hesap veya fatura ara" />
          </div>
          <div className="topbar-actions">
            <Button variant="ghost" aria-label="Bildirimler" icon={<Bell size={19} />} />
            <Button
              variant="ghost"
              aria-label={theme === 'dark' ? 'Açık temaya geç' : 'Koyu temaya geç'}
              icon={theme === 'dark' ? <Sun size={19} /> : <Moon size={19} />}
              onClick={toggleTheme}
            />
            <button className="user-menu" onClick={() => navigate('/dashboard')} aria-label="Kullanıcı bilgileri">
              <UserRound size={18} aria-hidden="true" />
              <span>{displayName}</span>
            </button>
            <Button variant="ghost" aria-label="Çıkış yap" icon={<LogOut size={18} />} onClick={handleLogout} />
          </div>
        </header>
        <main className="content">
          <Outlet />
        </main>
      </div>
      {drawerOpen && (
        <Button className="drawer-close" variant="secondary" aria-label="Menüyü kapat" icon={<X size={20} />} onClick={() => setDrawerOpen(false)} />
      )}
      <Button className="logout-floating" variant="ghost" icon={<LogOut size={18} />} onClick={handleLogout}>
        Çıkış
      </Button>
    </div>
  );
}
