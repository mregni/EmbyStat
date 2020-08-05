import React, { useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import TableCell from '@material-ui/core/TableCell';
import TableBody from '@material-ui/core/TableBody';
import TableContainer from '@material-ui/core/TableContainer';
import Paper from '@material-ui/core/Paper';
import Button from '@material-ui/core/Button';
import Grid from '@material-ui/core/Grid';
import GetAppRoundedIcon from '@material-ui/icons/GetAppRounded';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

import { getLogList, downloadLogFile } from '../../shared/services/LogService';
import { LogFile } from '../../shared/models/logs';

const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
  button__padding: {
    paddingTop: 5,
  }
});

const Logs = () => {
  const classes = useStyles();
  const [logs, setLogs] = useState<LogFile[]>([]);
  const { t } = useTranslation();

  useEffect(() => {
    getLogList()
      .then((response: LogFile[]) => setLogs(response))
  }, []);

  const convertToSize = (value: number): string => {
    if (value < 1024) {
      return `${String(value)} b`;
    } else if (value < 1024 * 1024) {
      return `${Math.floor(value / 1024)} Kb`;
    } else {
      return `${Math.floor(value / (1024 * 1024))} Mb`;
    }
  }

  return (
    <Grid container item xs={12} lg={8} xl={6}>
      <TableContainer component={Paper}>
        <Table className={classes.table} size="small" aria-label="a dense table">
          <TableHead>
            <TableRow>
              <TableCell>{t('COMMON.NAME')}</TableCell>
              <TableCell align="right">{t('LOGS.SIZE')}</TableCell>
              <TableCell align="right">{t('LOGS.CREATIONDATE')}</TableCell>
              <TableCell align="right">{t('LOGS.ACTIONS')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {logs.map((row) => (
              <TableRow key={row.fileName}>
                <TableCell component="th" scope="row">
                  {row.fileName}
                </TableCell>
                <TableCell align="right">{convertToSize(row.size)}</TableCell>
                <TableCell align="right">{moment(row.createdDate).format('L')}</TableCell>
                <TableCell align="right">
                  <Button
                    onClick={() => downloadLogFile(row.fileName, false)}
                    color="secondary"
                    size="small"
                    variant="outlined"
                    startIcon={<GetAppRoundedIcon />}
                    classes={{
                      outlinedSizeSmall: classes.button__padding
                    }}
                  >
                    {t('COMMON.DOWNLOAD')}
                  </Button>
                  <Button
                    onClick={() => downloadLogFile(row.fileName, true)}
                    className="m-l-8"
                    color="secondary"
                    size="small"
                    variant="outlined"
                    startIcon={<GetAppRoundedIcon />}
                    classes={{
                      outlinedSizeSmall: classes.button__padding
                    }}
                  >
                    {t('LOGS.ANONYMISED')}
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Grid>
  )
}

export default Logs
