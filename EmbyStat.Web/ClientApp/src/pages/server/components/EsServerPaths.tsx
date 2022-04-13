import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';

import {
  Card, CardContent, Stack, Table, TableBody, TableCell, TableRow, Typography,
} from '@mui/material';

import {EsTitle} from '../../../shared/components/esTitle';
import {ServerContext} from '../../../shared/context/server';

type EsTextRowProps = {
  value: string;
  label: string;
}

const EsTextRow = (props: EsTextRowProps) => {
  const {value, label} = props;
  const {t} = useTranslation();

  return (
    <TableRow>
      <TableCell>
        <Typography>
          {t(label)}
        </Typography>
      </TableCell>
      <TableCell>
        <Typography>
          {value}
        </Typography>
      </TableCell>
    </TableRow>
  );
};

export const EsServerPaths = () => {
  const {serverInfo} = useContext(ServerContext);

  const dataRows = [
    {label: 'SERVER.CACHEPATH', value: serverInfo.cachePath},
    {label: 'SERVER.METADATAPATH', value: serverInfo.internalMetadataPath},
    {label: 'SERVER.ITEMSBYNAMEPATH', value: serverInfo.itemsByNamePath},
    {label: 'SERVER.LOGPATH', value: serverInfo.logPath},
    {label: 'SERVER.PROGRAMDATAPATH', value: serverInfo.programDataPath},
    {label: 'SERVER.TRANSCODINGPATH', value: serverInfo.transcodingTempPath},
  ];

  return (
    <Card>
      <CardContent>
        <Stack spacing={2}>
          <EsTitle content="COMMON.PATHS" isFirst variant="h6" />
          <Table>
            <TableBody>
              {dataRows.map((item) => (<EsTextRow key={item.label} label={item.label} value={item.value} />))}
            </TableBody>
          </Table>
        </Stack>
      </CardContent>
    </Card>
  );
};
