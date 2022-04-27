import {parseISO} from 'date-fns';
import {format} from 'date-fns-tz';
import React, {useContext} from 'react';

import {Paper, Table, TableBody, TableCell, TableRow, Zoom} from '@mui/material';

import {JobsContext} from '../../shared/context/jobs';
import {useLocale} from '../../shared/hooks';
import {JobLogLine} from '../../shared/models/jobs';

export function JobLogs() {
  const {logLines, jobs} = useContext(JobsContext);
  const {locale} = useLocale();

  return (
    <Zoom in style={{transitionDelay: `${25 * jobs.length + 100}ms`}}>
      <Paper sx={{p: 2, minHeight: 482}}>
        <Table size="small">
          <TableBody>
            {
              logLines.slice(0).reverse().map((line: JobLogLine, i: number) => (
                <TableRow
                  // eslint-disable-next-line react/no-array-index-key
                  key={i}
                  sx={{
                    fontWeight: line.type === 0 ? 'normal' : 'bold',
                    color: (theme) => {
                      switch (line.type) {
                      case 1: return theme.palette.warning.main;
                      case 2: return theme.palette.error.main;
                      default: return theme.palette.text.primary;
                      }
                    },
                  }}>
                  <TableCell sx={{
                    width: '130px',
                    color: 'inherit',
                  }}>
                    {format(parseISO(line.dateTimeUtc), 'p P', {locale})}
                  </TableCell>
                  <TableCell sx={{width: '120px', color: 'inherit'}}>{line.jobName}</TableCell>
                  <TableCell sx={{color: 'inherit'}}>{line.value}</TableCell>
                </TableRow>
              ))
            }
          </TableBody>
        </Table>
      </Paper>
    </Zoom>
  );
}
