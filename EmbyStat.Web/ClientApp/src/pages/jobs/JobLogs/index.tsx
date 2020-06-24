import React from 'react'
import uuid from 'react-uuid';
import { Grid, Card, CardContent, makeStyles, Typography, Zoom } from '@material-ui/core';
import { useSelector } from 'react-redux';
import orange from '@material-ui/core/colors/orange';
import red from '@material-ui/core/colors/red';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { RootState } from '../../../store/RootReducer';
import { EnhancedJobLogLine } from '../../../shared/models/jobs';

const useStyles = makeStyles((theme) => ({
  card: {
    minHeight: 432,
    [theme.breakpoints.up('lg')]: {
      marginLeft: '16px',
    },
    [theme.breakpoints.down('md')]: {
      marginTop: '16px',
    }
  },
  text__container: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'flex-start',
    fontSize: '0.85rem',
  },
  text__accent: {
    color: orange['A400'],
    fontWeight: 'bold',
  },
  text__warn: {
    color: red['A700'],
    fontWeight: 'bold',
  }
}));

const JobLogs = () => {
  const classes = useStyles();
  const { t } = useTranslation();
  const lines = useSelector((state: RootState) => state.jobLogs);

  return (
    <Zoom in={true} style={{ transitionDelay: '225ms' }}>
      <Card className={classes.card}>
        <CardContent>
          <h2 className="m-t-0">{t('JOB.JOBLOGS')}</h2>
          <Grid container direction="column-reverse">
            {
              lines.map((line: EnhancedJobLogLine) =>
                <div key={uuid()}
                  className={
                    classNames(
                      classes.text__container,
                      { [classes.text__accent]: line.type === 1 },
                      { [classes.text__warn]: line.type === 2 })
                  }>
                  {line.left}  - {line.right}
                </div>
              )}
          </Grid>
        </CardContent>
      </Card >
    </Zoom>
  )
}

export default JobLogs
