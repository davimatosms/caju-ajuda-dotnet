import React from 'react';
import styles from './PageLayout.module.css';

type PageLayoutProps = {
  title?: string;
  children: React.ReactNode;
  hero?: React.ReactNode; // optional hero section above the container
  variant?: 'default' | 'centered';
};

export default function PageLayout({ title, children, hero, variant = 'default' }: PageLayoutProps) {
  const containerClass = variant === 'centered' ? `${styles.container} ${styles.centered}` : styles.container;

  return (
    <div className={styles.pageRoot}>
      {hero}
      <div className={containerClass}>
        {title && <h1 className={styles.pageTitle}>{title}</h1>}
        <div className={styles.content}>{children}</div>
      </div>
    </div>
  );
}
