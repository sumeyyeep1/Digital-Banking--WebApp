import { forwardRef, type InputHTMLAttributes } from 'react';
import { classNames } from '../../utils/format';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(({ label, error, className, id, ...props }, ref) => {
  const inputId = id ?? props.name;
  return (
    <label className="field" htmlFor={inputId}>
      <span>{label}</span>
      <input
        id={inputId}
        ref={ref}
        className={classNames(error && 'input-error', className)}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${inputId}-error` : undefined}
        {...props}
      />
      {error && (
        <small id={`${inputId}-error`} className="field-error">
          {error}
        </small>
      )}
    </label>
  );
});

Input.displayName = 'Input';
