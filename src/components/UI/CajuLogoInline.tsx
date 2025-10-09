import React from 'react';

// Small, optimized inline SVG for the Caju logo that preserves the brand orange.
// By default this component renders only the cashew mark (icon-only). Pass
// `showWordmark={true}` to render the accompanying wordmark text.
export default function CajuLogoInline({
  width = 120,
  height = 32,
  showWordmark = false,
}: {
  width?: number;
  height?: number;
  showWordmark?: boolean;
}) {
  return (
    <svg width={width} height={height} viewBox="0 0 120 32" xmlns="http://www.w3.org/2000/svg" role="img" aria-label="Caju Ajuda">
      <title>Caju Ajuda</title>
      <rect width="120" height="32" fill="none" />
      {/* Simple cashew-like mark */}
      <path d="M12 8c4-6 16-6 20 0s-2 12-6 14-14-2-14-6 0-6 0-8z" fill="#FD6906" />
      {/* Optional wordmark */}
      {showWordmark && (
        <>
          <text x="40" y="21" fontFamily="Inter, Arial, sans-serif" fontSize="12" fontWeight="600" fill="#3B2512">Caju</text>
          <text x="78" y="21" fontFamily="Inter, Arial, sans-serif" fontSize="12" fill="#6D4A23">Ajuda</text>
        </>
      )}
    </svg>
  );
}
