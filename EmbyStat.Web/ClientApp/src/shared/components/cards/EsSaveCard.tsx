import React, {ReactNode} from 'react';
import {SubmitHandler} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Button, Card, CardContent, Stack, Typography} from '@mui/material';

type Props<T> = {
  title: string;
  children: ReactNode | ReactNode[];
  saveForm: SubmitHandler<T>;
  handleSubmit: (onValid: SubmitHandler<T>) => (e?: React.BaseSyntheticEvent) =>Promise<void>;
}

export function EsSaveCard<T, >(props: Props<T>) {
  const {title, children, saveForm, handleSubmit} = props;
  const {t} = useTranslation();

  return (
    <form autoComplete="off" onSubmit={handleSubmit(saveForm)} style={{height: '100%'}}>
      <Card sx={{width: '100%', height: '100%'}}>
        <CardContent sx={{height: '100%'}}>
          <Typography
            variant="h5"
            color="primary"
            gutterBottom
            sx={{textTransform: 'capitalize', mb: 2}}
          >
            {t(title)}
          </Typography>
          <Stack spacing={2} justifyContent="space-between" sx={{height: 'calc(100% - 38px)'}}>
            <Stack spacing={1}>
              {children}
            </Stack>
            <Stack direction="row" justifyContent="flex-end">
              <Button variant="contained" color="primary" type="submit">
                {t('COMMON.SAVE')}
              </Button>
            </Stack>
          </Stack>
        </CardContent>
      </Card>
    </form>
  );
}
