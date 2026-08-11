import type { ReactNode } from 'react';
import { classNames } from '../../utils/format';

export function Badge({ children, tone = 'neutral' }: { children: ReactNode; tone?: 'success' | 'warning' | 'danger' | 'neutral' }) {
  return <span className={classNames('badge', `badge-${tone}`)}>{children}</span>;
}
