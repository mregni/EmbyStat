import React, {useContext} from 'react';

import {Stack} from '@mui/material';

import {ServerContext} from '../../shared/context/server';
import {EsServerDetails, EsServerExtraInfo, EsServerFeatures, EsServerPaths} from './components';

export function ServerInfo() {
  const {serverInfo} = useContext(ServerContext);

  if (serverInfo === null) {
    return (null);
  }

  return (
    <>
      <Stack spacing={2} direction="row">
        <EsServerDetails />
        <EsServerFeatures />
        <EsServerPaths />
      </Stack>
      <EsServerExtraInfo />
    </>
  );
}
