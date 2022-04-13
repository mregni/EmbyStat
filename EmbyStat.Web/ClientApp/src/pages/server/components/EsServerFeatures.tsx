import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';

import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import {
  Card, CardContent, Stack, Table, TableBody, TableCell, TableRow, Typography,
} from '@mui/material';

import {EsTitle} from '../../../shared/components/esTitle';
import {ServerContext} from '../../../shared/context/server';

type EsBoolListItemProps = {
  label: string;
  value: boolean;
}

const EsBoolRow = (props: EsBoolListItemProps) => {
  const {label, value} = props;
  const {t} = useTranslation();

  return (
    <TableRow>
      <TableCell>
        {value && (<CheckCircleOutlineIcon color="success" />)}
        {!value && (<HighlightOffIcon color="error" />)}
      </TableCell>
      <TableCell>
        <Typography>
          {t(label)}
        </Typography>
      </TableCell>
    </TableRow>
  );
};


export const EsServerFeatures = () => {
  const {serverInfo} = useContext(ServerContext);

  const dataRows = [
    {label: 'SERVER.HTTPSSUPPORT', value: serverInfo.supportsHttps},
    {label: 'SERVER.AUTORUNSUPPORT', value: serverInfo.supportsAutoRunAtStartup},
    {label: 'SERVER.MONITORSUPPORT', value: serverInfo.supportsLibraryMonitor},
    {label: 'SERVER.CANLAUNCHBROWSER', value: serverInfo.canLaunchWebBrowser},
    {label: 'SERVER.CANSELFRESTART', value: serverInfo.canSelfRestart},
    {label: 'SERVER.CANSELFUPDATE', value: serverInfo.canSelfUpdate},
    {label: 'SERVER.PENDINGRESTART', value: serverInfo.hasPendingRestart},
    {label: 'SERVER.UPDATEAVAILABLE', value: serverInfo.hasUpdateAvailable},
  ];

  return (
    <Card>
      <CardContent>
        <Stack spacing={2}>
          <EsTitle content="SERVER.FEATURES" isFirst variant="h6" />
          <Table>
            <TableBody>
              {dataRows.map((item) => (<EsBoolRow key={item.label} label={item.label} value={item.value} />))}
            </TableBody>
          </Table>
        </Stack>
      </CardContent>
    </Card>
  );
};
