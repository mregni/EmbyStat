import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Typography} from '@mui/material';

import {EsFilterNumberField} from '..';

type Props = {
  onValueChanged: (val0: string) => void;
  errors: any;
  register: Function;
  unit?: string;
}

export function EsFilterRangeField(props: Props) {
  const {onValueChanged, errors, register, unit = ''} = props;
  const {t} = useTranslation();
  const [betweenValue, setBetweenValue] = useState({
    left: '',
    right: '',
  });

  useEffect(() => {
    onValueChanged(`${betweenValue.left}|${betweenValue.right}`);
  }, [betweenValue, onValueChanged]);

  const onValueLeftChanged = (value: string) => {
    if (!errors.betweenLeft) {
      setBetweenValue((state) => ({...state, left: value}));
    }
  };

  const onValueRightChanged = (value: string) => {
    if (!errors.betweenRight) {
      setBetweenValue((state) => ({...state, right: value}));
    }
  };


  return (
    <>
      <EsFilterNumberField
        errors={errors}
        register={register}
        onValueChanged={onValueLeftChanged}
        unit={unit}
      />
      <Typography sx={{pt: '6px'}}>{t('COMMON.AND')}</Typography>
      <EsFilterNumberField
        errors={errors}
        register={register}
        onValueChanged={onValueRightChanged}
        unit={unit}
        fieldName='text2'
      />
    </>
  );
}
