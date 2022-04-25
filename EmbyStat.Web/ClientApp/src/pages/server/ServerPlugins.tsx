import {t} from 'i18next';
import React, {useContext} from 'react';

import {Paper, Table, TableBody, TableCell, TableHead, TableRow} from '@mui/material';

import {ServerContext} from '../../shared/context/server';

type Props = {}

export function ServerPlugins(props: Props) {
  const {isLoaded, plugins} = useContext(ServerContext);

  if (!isLoaded) {
    return (<></>);
  }

  const headers = [
    {label: 'COMMON.NAME'},
    {label: 'COMMON.VERSION'},
    {label: 'COMMON.DESCRIPTION'},
  ];

  return (
    <Paper sx={{p: 2}}>
      <Table>
        <TableHead>
          <TableRow>
            {headers.map((head) => (<TableCell key={head.label}>{t(head.label)}</TableCell>))}
          </TableRow>
        </TableHead>
        <TableBody>
          {
            plugins.map((plugin) => (
              <TableRow key={plugin.id}>
                <TableCell>{plugin.name}</TableCell>
                <TableCell>{plugin.version}</TableCell>
                <TableCell>{plugin.description}</TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </Paper>

  );
}
