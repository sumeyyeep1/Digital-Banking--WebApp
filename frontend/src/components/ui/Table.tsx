import type { ReactNode } from 'react';

export function Table({
  headers,
  rows,
  mobileCards,
}: {
  headers: string[];
  rows: ReactNode[][];
  mobileCards?: ReactNode[];
}) {
  return (
    <>
      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              {headers.map((header) => (
                <th key={header}>{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row, rowIndex) => (
              <tr key={rowIndex}>
                {row.map((cell, cellIndex) => (
                  <td key={`${rowIndex}-${cellIndex}`}>{cell}</td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {mobileCards && <div className="mobile-list">{mobileCards}</div>}
    </>
  );
}
