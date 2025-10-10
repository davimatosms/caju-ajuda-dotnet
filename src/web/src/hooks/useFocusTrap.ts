import { useEffect, RefObject } from 'react';

function getFocusableElements(container: HTMLElement | null) {
  if (!container) return [] as HTMLElement[];
  const selectors = [
    'a[href]',
    'area[href]',
    'input:not([disabled])',
    'select:not([disabled])',
    'textarea:not([disabled])',
    'button:not([disabled])',
    'iframe',
    'object',
    'embed',
    '[tabindex]:not([tabindex="-1"])',
    '[contenteditable]'
  ];
  const nodes = Array.from(container.querySelectorAll(selectors.join(',')));
  return nodes.filter((n): n is HTMLElement => n instanceof HTMLElement);
}

export default function useFocusTrap(
  containerRef: RefObject<HTMLElement | null>,
  isOpen: boolean,
  onClose?: () => void
) {
  useEffect(() => {
    if (!isOpen) return;

    const container = containerRef.current;
    if (!container) return;

    const previousActive = document.activeElement as HTMLElement | null;

    // prevent background scrolling
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const focusable = getFocusableElements(container);
    const first = focusable[0];
    const last = focusable[focusable.length - 1];

    if (first) first.focus();

    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') {
        e.preventDefault();
        onClose && onClose();
        return;
      }

      if (e.key === 'Tab') {
        if (!focusable || focusable.length === 0) {
          e.preventDefault();
          return;
        }
        const active = document.activeElement as HTMLElement;
        if (e.shiftKey) {
          // shift + tab
          if (active === first || active === container) {
            e.preventDefault();
            last && last.focus();
          }
        } else {
          // tab
          if (active === last) {
            e.preventDefault();
            first && first.focus();
          }
        }
      }
    }

    document.addEventListener('keydown', handleKeyDown);

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = previousOverflow;
      previousActive && previousActive.focus();
    };
  }, [containerRef, isOpen, onClose]);
}
