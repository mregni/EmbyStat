import React from 'react';
import {useTranslation} from 'react-i18next';

import {Stack} from '@mui/material';

import {EsLoading} from '../../shared/components/esLoading';
import {EsTitle} from '../../shared/components/esTitle';
import {MediaServerUserContextProvider} from '../../shared/context/mediaServerUser';
import {EsUserStatistics, EsUserTable} from './components';

function UserContainer() {
  const {t} = useTranslation();

  // TODO: Fix the loadin state here!
  return (
    <EsLoading label={t('USERS.LOADER')} loading={false}>
      <Stack spacing={2}>
        <EsTitle content={t('COMMON.NUMBERS')} isFirst={true} />
        <EsUserStatistics />
        <EsTitle content={t('USERS.USERS')} />
        <EsUserTable/>
      </Stack>
    </EsLoading>
  );
}

export function Users() {
  return (
    <MediaServerUserContextProvider>
      <UserContainer/>
    </MediaServerUserContextProvider>
  );
}
