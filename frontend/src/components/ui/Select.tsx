import { forwardRef, type SelectHTMLAttributes } from 'react';

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  error?: string;
  options: Array<{ value: string; label: string }>;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(({ label, error, options, id, ...props }, ref) => {
  const selectId = id ?? props.name;
  return (
    <label className="field" htmlFor={selectId}>
      <span>{label}</span>
      <select id={selectId} ref={ref} aria-invalid={Boolean(error)} {...props}>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      {error && <small className="field-error">{error}</small>}
    </label>
  );
});

Select.displayName = 'Select';
