export function Skeleton({ lines = 1 }: { lines?: number }) {
  return (
    <div className="skeleton-stack" aria-hidden="true">
      {Array.from({ length: lines }).map((_, index) => (
        <span className="skeleton" key={index} />
      ))}
    </div>
  );
}
