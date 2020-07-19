import React, { useState, useEffect } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import classNames from 'classnames';
import CardMedia from '@material-ui/core/CardMedia';
import Grid from '@material-ui/core/Grid';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import Typography from '@material-ui/core/Typography';
import CardContent from '@material-ui/core/CardContent';
import { makeStyles } from '@material-ui/core/styles';
import { useDispatch } from 'react-redux';

import {
  MediaServerInfo,
  MediaServerUser,
  Library,
} from '../../../../shared/models/mediaServer';
import { Wizard } from '../../../../shared/models/wizard';
import Emby from '../../../../shared/assets/images/emby.png';
import Jellyfin from '../../../../shared/assets/images/jellyfin.png';
import EmbyStatSelect from '../../../../shared/components/inputs/select/EmbyStatSelect';
import {
  setAdminId,
  setAllLibraries,
  setMediaServerId,
} from '../../../../store/WizardSlice';

interface Props {
  serverInfo: MediaServerInfo;
  administrators: MediaServerUser[];
  libraries: Library[];
  wizard: Wizard;
}

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    padding: '8px',
    position: 'relative',
  },
  content: {
    flex: '1 0 auto',
  },
  cover: {
    width: 80,
    height: 80,
    padding: '10px',
  },
  server__details__header: {
    color:
      theme.palette.type === 'dark'
        ? theme.palette.grey[400]
        : theme.palette.grey[600],
    fontSize: '0.8rem',
  },
  server__details__name: {
    paddingLeft: '8px',
  },
  server__details__icon: {
    width: '20px',
    position: 'absolute',
    right: 5,
    top: 5,
    '&:hover': {
      cursor: 'pointer',
    },
  },
}));

const TestSuccessFul = (props: Props) => {
  const classes = useStyles();
  const { serverInfo, administrators, libraries, wizard } = props;
  const [selectedAdmin, setSelectedAdmin] = useState(administrators[0].id);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const openServer = () => {
    const protocol = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    window.open(
      `${protocol}${wizard.serverAddress}:${wizard.serverPort}${
      wizard?.serverBaseurl ?? ''
      }`,
      '_blank'
    );
  };

  useEffect(() => {
    dispatch(setAdminId(selectedAdmin));
  }, [selectedAdmin, dispatch]);

  useEffect(() => {
    dispatch(setMediaServerId(serverInfo.id));
  }, [serverInfo, dispatch]);

  useEffect(() => {
    dispatch(setAllLibraries(libraries));
  }, [dispatch, libraries]);

  const adminChanged = (event) => {
    console.log(event.target.value);
    setSelectedAdmin(event.target.value);
  };

  const getReleaseString = (updatelevel: number): string => {
    if (serverInfo.systemUpdateLevel === 0) {
      return 'Release';
    }
    else if (serverInfo.systemUpdateLevel === 1) {
      return 'Beta';
    }
    return 'Dev';
  }

  return (
    <Grid item xs={12} className="m-t-32">
      <Zoom in={true} style={{ transitionDelay: '100ms' }}>
        <Card elevation={7} square className={classes.root}>
          <CardMedia
            className={classes.cover}
            component="img"
            image={wizard.serverType === 0 ? Emby : Jellyfin}
            title="Media server logo"
          />
          <CardContent>
            <Grid item container direction="row">
              <Grid
                item
                md={4}
                container
                direction="column"
                justify="flex-start"
                className="m-l-32"
              >
                <Typography
                  variant="body1"
                  className={classes.server__details__header}
                >
                  <Trans i18nKey="COMMON.SERVERNAME" />
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__name}
                >
                  {serverInfo.serverName}
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__header}
                >
                  <Trans i18nKey="COMMON.VERSION" />
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__name}
                >
                  {serverInfo.version}
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__header}
                >
                  <Trans i18nKey="COMMON.OS" />
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__name}
                >
                  {serverInfo.operatingSystem}
                </Typography>
              </Grid>
              <Grid
                item
                md={4}
                container
                direction="column"
                justify="flex-start"
              >
                <Typography
                  variant="body1"
                  className={classes.server__details__header}
                >
                  <Trans i18nKey="COMMON.LANADDRESS" />
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__name}
                >
                  {serverInfo.localAddress}
                </Typography>
                {serverInfo.wanAddress !== null ? (
                  <>
                    <Typography
                      variant="body1"
                      className={classes.server__details__header}
                    >
                      <Trans i18nKey="COMMON.WANADDRESS" />
                    </Typography>
                    <Typography
                      variant="body1"
                      className={classes.server__details__name}
                    >
                      {serverInfo.wanAddress}
                    </Typography>
                  </>
                ) : null}

                <Typography
                  variant="body1"
                  className={classes.server__details__header}
                >
                  <Trans i18nKey="COMMON.UPDATELEVEL" />
                </Typography>
                <Typography
                  variant="body1"
                  className={classes.server__details__name}
                >
                  {getReleaseString(serverInfo.systemUpdateLevel)}
                </Typography>
              </Grid>
            </Grid>

            <OpenInNewIcon
              className={classes.server__details__icon}
              onClick={() => openServer()}
            />
          </CardContent>
        </Card>
      </Zoom>
      {wizard.serverType === 1 ? (
        <Zoom in={true} style={{ transitionDelay: '300ms' }}>
          <Card
            elevation={7}
            square
            className={classNames(classes.root, 'm-t-32')}
          >
            <CardContent>
              {t('WIZARD.JELLYFIN.ADMINTEXT')}
              <EmbyStatSelect
                className="m-t-16"
                value={selectedAdmin}
                variant="standard"
                onChange={adminChanged}
                menuItems={administrators.map((admin) => {
                  return { id: admin.id, value: admin.id, label: admin.name };
                })}
              />
            </CardContent>
          </Card>
        </Zoom>
      ) : null}
    </Grid>
  );
};

export default TestSuccessFul;
