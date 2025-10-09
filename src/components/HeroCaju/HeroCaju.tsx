// Hero artwork removed; keep component as a noop to avoid requiring image assets

type Props = {
  className?: string;
  size?: 'large' | 'medium' | 'small';
  decorative?: boolean;
};

export default function HeroCaju({ className = '', size = 'large', decorative = true }: Props) {
  // Hero artwork removed: component intentionally renders nothing to avoid external asset dependencies.
  return null;
}
