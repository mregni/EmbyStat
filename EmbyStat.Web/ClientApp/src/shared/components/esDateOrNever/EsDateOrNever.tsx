import {format} from 'date-fns';
import React from 'react';
import {useTranslation} from 'react-i18next';

import Typography from '@mui/material/Typography';

import {useLocale} from '../../hooks';

type Props = {
  value: string | null
}

export function EsDateOrNever(props: Props) {
  const {value} = props;
  const {locale} = useLocale();
  const {t} = useTranslation();

  return (value !== null ?
    <Typography>{format(new Date(value), 'Pp', {locale})}</Typography> :
    <Typography>{t('COMMON.NEVER')}</Typography>
  );
}
