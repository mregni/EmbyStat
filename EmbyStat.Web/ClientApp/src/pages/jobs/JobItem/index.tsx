import React, { useState, useEffect } from 'react'
import { Paper, LinearProgress, makeStyles, Zoom, IconButton, Menu, MenuItem } from '@material-ui/core';
import RoundIconButton from '../../../shared/components/buttons/RoundIconButton'
import { Job } from '../../../shared/models/jobs'
import moment from 'moment'
import PlayCircleOutlineRoundedIcon from '@material-ui/icons/PlayCircleOutlineRounded';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import { useTranslation } from 'react-i18next'
import classNames from 'classnames';

import { fireJob } from '../../../shared/services/JobService';
import JobSettingsDialog from '../JobSettingsDialog';
import { useServerType } from '../../../shared/hooks';


const useStyles = makeStyles((theme) => ({
  paper: {
    display: 'flex',
    position: 'relative',
    padding: 8,
    '&:not(:first-child)': {
      marginTop: 16,
    }
  },
  paper__details: {
    display: 'flex',
    flexDirection: 'column',
    flex: 1,
    justifyContent: 'center',
  },
  "paper__details--top": {
    display: 'flex',
    flex: '0 1 auto',
    height: 30,
    paddingTop: 5,
  },
  "paper__details--bottom": {
    display: 'flex',
    flex: '1 1 auto',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingRight: 5,
  },
  paper__menu: {
    position: 'absolute',
    right: 0,
    top: 0,
  },
  progress: {
    width: 'calc(100% - 65px)',
    marginRight: 20
  },
  title: {
    fontWeight: 'bold',
    [theme.breakpoints.down('sm')]: {
      width: '200px'
    },
    [theme.breakpoints.up('md')]: {
      width: '250px'
    }
  },
  hidden: {
    visibility: 'hidden',
  },
  progess__height: {
    height: 22
  }
}));

interface Props {
  job: Job,
  i: number
}

const JobItem = (props: Props) => {
  const { job, i } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const serverType = useServerType();
  const [loading, setLoading] = useState(job.state === 1);
  const [openSettings, setOpenSettings] = useState(false);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const open = Boolean(anchorEl);

  useEffect(() => {
    setLoading(job.state === 1);
  }, [job]);

  const stateSwitch = (job: Job) => {
    switch (job.state) {
      case 0: return t('JOB.NORUN');
      case 1: return "Processing";
      case 2: return `${t('JOB.LASTRUN')} ${moment(job.endTimeUtcIso).from(moment())}`;
      case 3: return `${t('JOB.LASTRUN')} ${moment(job.endTimeUtcIso).from(moment())}`;
      default: return '';
    }
  }

  const fireJobAction = () => {
    console.log(`job ${job.id} fired`);
    fireJob(job.id);
    setLoading(true);
  }

  const handleOpenMenu = (event) => {
    setAnchorEl(event.currentTarget);
    setOpenSettings(false);
  };

  const handleCloseMenu = (option: string | null) => {
    setAnchorEl(null);
    if (option === 'settings') {
      setOpenSettings(true);
    }
  };

  return (
    <Zoom in={true} style={{ transitionDelay: (25 * i) + 100 + 'ms' }}>
      <Paper className={classes.paper}>
        <div className="m-r-16">
          <RoundIconButton
            onClick={fireJobAction}
            Icon={<PlayCircleOutlineRoundedIcon />}
            disabled={loading}
          />
        </div>
        <div className={classes.paper__menu}>
          <IconButton size='small' className="m-t-8 m-r-4" onClick={handleOpenMenu}>
            <MoreVertIcon />
          </IconButton>
          <Menu
            id="long-menu"
            anchorEl={anchorEl}
            keepMounted
            open={open}
            onClose={() => handleCloseMenu(null)}
          >
            <MenuItem onClick={() => handleCloseMenu('settings')}>Settings</MenuItem>
          </Menu>
          <JobSettingsDialog openSettingsDialog={openSettings} job={job} />
        </div>
        <div className={classes.paper__details}>
          <div className={classes["paper__details--top"]}>
            <div className={classes.title}>
              {t(`JOB.INFO.${job.title}`, { type: serverType })}
            </div>
            <div>
              {stateSwitch(job)}
            </div>
          </div>
          {
            loading ?
              job.state === 1
                ? <div className={classNames(classes["paper__details--bottom"], [classes.progess__height])}>
                  <LinearProgress color="secondary" variant="determinate" value={job.currentProgressPercentage} className={classes.progress} />
                  <div><i>{job.currentProgressPercentage} %</i></div>
                </div>
                :
                <div className={classNames(classes["paper__details--bottom"], [classes.progess__height])} >
                  <LinearProgress color="secondary" variant="indeterminate" className={classes.progress} />
                  <div><i>0 %</i></div>
                </div>
              : <div className={classes.progess__height} />

          }
        </div>
      </Paper>
    </Zoom >
  );
};

export default JobItem
