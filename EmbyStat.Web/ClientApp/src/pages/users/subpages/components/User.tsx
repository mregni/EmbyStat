import React from 'react';
import {useTranslation} from 'react-i18next';

import {
  Card, CardContent, Stack, Table, TableBody, TableCell, TableRow, Typography,
} from '@mui/material';

import {EsBoolCheck} from '../../../../shared/components/esBoolCheck';
import {EsDateOrNever} from '../../../../shared/components/esDateOrNever';
import {EsTitle} from '../../../../shared/components/esTitle';
import {MediaServerUserDetails} from '../../../../shared/models/mediaServer';

type Props = {
  details: MediaServerUserDetails
}

export function User(props: Props) {
  const {details} = props;
  const {t} = useTranslation();

  const dataRows = [
    {label: 'USERS.ADMIN', value: <EsBoolCheck value={details.isAdministrator} />},
    {label: 'USERS.DISABLED', value: <EsBoolCheck value={details.isDisabled} />},
    {label: 'USERS.HIDDEN', value: <EsBoolCheck value={details.isHidden} />},
    {label: 'USERS.LASTACTIVE', value: <EsDateOrNever value={details.lastActivityDate} />},
    {label: 'USERS.LASTLOGIN', value: <EsDateOrNever value={details.lastLoginDate} />},
  ];

  return (
    <Card>
      <CardContent>
        <Stack spacing={2} direction="row" alignItems="flext-start">
          <img src='' alt="profile"/>
          <Stack spacing={1}>
            <EsTitle content={details.name} isFirst variant="h6" />
            <Table>
              <TableBody>
                {
                  dataRows.map((row) => (
                    <TableRow key={row.label}>
                      <TableCell>
                        <Typography>{t(row.label)}</Typography>
                      </TableCell>
                      <TableCell>{row.value}</TableCell>
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
}
