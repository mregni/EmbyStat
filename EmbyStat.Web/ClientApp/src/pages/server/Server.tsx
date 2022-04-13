import React, {useContext, useEffect} from 'react';
import {useTranslation} from 'react-i18next';

import {Stack} from '@mui/material';

import {EsTitle} from '../../shared/components/esTitle';
import {ServerContext, ServerContextProvider} from '../../shared/context/server';
import {ServerInfo} from './ServerInfo';
import {ServerPlugins} from './ServerPlugins';

const ServerContainer = () => {
  const {t} = useTranslation();
  const {isLoaded, load} = useContext(ServerContext);

  useEffect(() => {
    (async () => {
      if (!isLoaded) {
        await load();
      }
    })();
  }, [isLoaded]);

  return (
    <Stack spacing={2}>
      <EsTitle content={t('SERVER.INFO')} isFirst />
      <ServerInfo />
      <EsTitle content={t('SERVER.PLUGINS')} />
      <ServerPlugins />
    </Stack>
  );
};

export const Server = () => {
  return (
    <ServerContextProvider>
      <ServerContainer />
    </ServerContextProvider>
  );
};
