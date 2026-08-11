import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { Loader2 } from 'lucide-react';
import { classNames } from '../../utils/format';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  isLoading?: boolean;
  icon?: ReactNode;
}

export function Button({ className, variant = 'primary', isLoading, icon, children, disabled, ...props }: ButtonProps) {
  return (
    <button className={classNames('btn', `btn-${variant}`, className)} disabled={disabled || isLoading} {...props}>
      {isLoading ? <Loader2 className="spin" size={18} aria-hidden="true" /> : icon}
      {children && <span>{children}</span>}
    </button>
  );
}
