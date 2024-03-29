import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import GetAppIcon from '@mui/icons-material/GetApp';
import {
  Button, Grid, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
} from '@mui/material';

import {LogFile} from '../../shared/models/logs';
import {downloadLogFile, getLogList} from '../../shared/services/logService';

export function Logs() {
  const [logs, setLogs] = useState<LogFile[]>([]);
  const {t} = useTranslation();

  useEffect(() => {
    getLogList()
      .then((response: LogFile[]) => setLogs(response))
      .catch(() => {});
  }, []);

  const convertToSize = (value: number): string => {
    if (value < 1024) {
      return `${String(value)} b`;
    } else if (value < 1024 * 1024) {
      return `${Math.floor(value / 1024)} Kb`;
    }
    return `${Math.floor(value / (1024 * 1024))} Mb`;
  };

  return (
    <Grid container item xs={12} lg={6} xl={4}>
      <TableContainer component={Paper} sx={{p: 2}}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>{t('COMMON.NAME')}</TableCell>
              <TableCell align="right">{t('COMMON.SIZE')}</TableCell>
              <TableCell align="right">{t('LOGS.ACTIONS')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {logs.map((row) => (
              <TableRow key={row.name}>
                <TableCell component="th" scope="row">
                  {row.name}
                </TableCell>
                <TableCell align="right">{convertToSize(row.size)}</TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={1} justifyContent="flex-end">
                    <Button
                      onClick={() => downloadLogFile(row.name, false)}
                      color="secondary"
                      size="small"
                      variant="outlined"
                      startIcon={<GetAppIcon />}
                    >
                      {t('COMMON.DOWNLOAD')}
                    </Button>
                    <Button
                      onClick={() => downloadLogFile(row.name, true)}
                      className="m-l-8"
                      color="secondary"
                      size="small"
                      variant="outlined"
                      startIcon={<GetAppIcon />}
                    >
                      {t('LOGS.ANONYMISED')}
                    </Button>
                  </Stack>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Grid>
  );
}
