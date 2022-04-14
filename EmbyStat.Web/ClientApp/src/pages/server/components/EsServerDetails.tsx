import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';

import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import {
  Card, CardContent, Stack, Table, TableBody, TableCell, TableRow, Typography,
} from '@mui/material';

import Emby from '../../../shared/assets/images/emby.png';
import Jellyfin from '../../../shared/assets/images/jellyfin.png';
import {EsChipTitle} from '../../../shared/components/esTitle';
import {ServerContext} from '../../../shared/context/server';
import {useServerType} from '../../../shared/hooks';

export const EsServerDetails = () => {
  const {serverInfo} = useContext(ServerContext);
  const {t} = useTranslation();
  const {serverType} = useServerType();

  const openServer = (): void => {
    window.open(serverInfo?.wanAddress ?? serverInfo.localAddress, '_blank');
  };

  const dataRows = [
    {label: 'COMMON.ID', value: serverInfo.id},
    {label: 'SERVER.LANADDRESS', value: serverInfo.localAddress},
    {label: 'SERVER.WANADDRESS', value: serverInfo.wanAddress ?? t('COMMON.UNKNOWN')},
    {label: 'COMMON.OS', value: `${serverInfo.operatingSystem} / ${serverInfo.operatingSystemDisplayName}`},
    {label: 'COMMON.UPDATELEVEL', value: serverInfo.systemUpdateLevel},
    {label: 'COMMON.HTTPPORT', value: serverInfo.httpServerPortNumber},
    {label: 'COMMON.HTTPSPORT', value: serverInfo.httpsPortNumber},
    {label: 'SERVER.WEBSOCKETPORT', value: serverInfo.webSocketPortNumber},
  ];

  return (
    <Card>
      <CardContent>
        <Stack spacing={2} direction="row" alignItems="flex-start">
          <img src={serverType === 'Emby' ? Emby : Jellyfin} alt="Media server logo" width="80" height="80" />
          <Stack spacing={1}>
            <EsChipTitle
              variant="h4"
              content={serverInfo.serverName}
              isFirst
              chipContent={serverInfo.version}
              icon={<OpenInNewIcon />}
              onClick={openServer}
              tooltip={t('SERVER.OPENSERVER')}
            />
            <Table>
              <TableBody>
                {
                  dataRows.map((row) => (
                    <TableRow key={row.label}>
                      <TableCell>
                        <Typography>{t(row.label)}</Typography>
                      </TableCell>
                      <TableCell>
                        <Typography>{row.value}</Typography>
                      </TableCell>
                    </TableRow>
                  ))
                }
              </TableBody>
            </Table>
          </Stack>
        </Stack>
      </CardContent>
    </Card>
  );
};
