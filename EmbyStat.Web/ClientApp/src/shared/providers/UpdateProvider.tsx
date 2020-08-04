import React, { ReactNode, useEffect } from 'react'
import { useSelector } from 'react-redux';
import { makeStyles } from '@material-ui/core/styles';

import { RootState } from '../../store/RootReducer';
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import { UpdateSuccessFull } from '../models/embystat';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    width: '100%',
    height: '100%',
  },
  container: {
    width: 350,
  },
  text: {
    width: 'calc(100% - 40px)'
  },
  title: {
    fontSize: '2rem',
  },
  title__sub: {
    fontSize: '0.8rem',
    fontStyle: 'italic',
  }
}));

interface Props {
  children: ReactNode | ReactNode[];
}

const UpdateProvider = (props: Props) => {
  const { children } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const status = useSelector((state: RootState) => state.serverStatus);
  const settings = useSelector((state: RootState) => state.settings);

  useEffect(() => {
    if (status.updateSuccesfull === UpdateSuccessFull.SuccesFull) {
      window.location.reload();
    }
  }, [status.updateSuccesfull]);

  if (status.updating || settings.updateInProgress) {
    return (
      <div className={classes.root}>
        <Grid container direction="row" className={classes.container} spacing={2} alignItems="center">
          <Grid item container direction="column" spacing={1} className={classes.text} alignItems="center">
            <Grid item className={classes.title}>
              {t('LOADER.UPDATING')}
            </Grid>
            <Grid item className={classes.title__sub}>
              {t('LOADER.UPDATINGSUB')}
            </Grid>
          </Grid>
          <Grid item>
            <div className="sk-chase">
              <div className="sk-chase-dot"></div>
              <div className="sk-chase-dot"></div>
              <div className="sk-chase-dot"></div>
              <div className="sk-chase-dot"></div>
              <div className="sk-chase-dot"></div>
              <div className="sk-chase-dot"></div>
            </div>
          </Grid>
        </Grid>
      </div>
    )
  }

  return (
    <>
      {children}
    </>
  )
}

export default UpdateProvider
