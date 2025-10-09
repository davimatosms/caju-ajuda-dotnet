import React from 'react';
import styles from './Button.module.css';

type Props = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: 'primary' | 'secondary' | 'ghost';
};

const Button: React.FC<Props> = ({ variant = 'primary', children, className = '', ...rest }) => {
  const cls = `${styles.btn} ${variant === 'primary' ? styles.primary : variant === 'secondary' ? styles.secondary : styles.ghost} ${className}`.trim();
  return (
    <button className={cls} {...rest}>
      {children}
    </button>
  );
};

export default Button;
