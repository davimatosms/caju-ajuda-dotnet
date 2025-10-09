import React from 'react';
import styles from './HeroCaju.module.css';

type Props = {
  className?: string;
  size?: 'large' | 'medium' | 'small';
  decorative?: boolean;
};

export default function HeroCaju({ className = '', size = 'large', decorative = true }: Props) {
  const imgClass = `${styles.heroCaju} ${styles[size]} ${className}`.trim();
  return (
    <picture className={styles.wrap}>
      <source srcSet="/assets/caju-hero.webp" type="image/webp" />
      <img src="/assets/caju-hero.png" alt={decorative ? '' : 'Caju Ajuda'} aria-hidden={decorative} className={imgClass} loading="eager" width={600} height={380} />
    </picture>
  );
}
