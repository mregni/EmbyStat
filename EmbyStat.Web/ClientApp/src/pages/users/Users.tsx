import React from 'react';
import {useTranslation} from 'react-i18next';

import {Stack} from '@mui/material';

import {EsLoading} from '../../shared/components/esLoading';
import {EsTitle} from '../../shared/components/esTitle';
import {MediaServerUserContextProvider} from '../../shared/context/mediaServerUser';
import {EsUserStatistics, EsUserTable} from './components';

const UserContainer = () => {
  const {t} = useTranslation();

  // TODO: Fix the loadin state here!
  return (
    <EsLoading label={t('USERS.LOADER')} loading={false}>
      <Stack spacing={2}>
        <EsTitle content={t('USERS.NUMBERS')} isFirst />
        <EsUserStatistics />
        <EsTitle content={t('USERS.USERS')} />
        <EsUserTable/>
      </Stack>
    </EsLoading>
  );
};

export const Users = () => {
  return (
    <MediaServerUserContextProvider>
      <UserContainer/>
    </MediaServerUserContextProvider>
  );
};
