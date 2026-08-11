import type { HTMLAttributes } from 'react';
import { classNames } from '../../utils/format';

export function Card({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return <div className={classNames('card', className)} {...props} />;
}
