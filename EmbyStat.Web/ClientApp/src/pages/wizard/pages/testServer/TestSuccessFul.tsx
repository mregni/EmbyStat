import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';
import Grid from '@material-ui/core/Grid';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import { makeStyles } from '@material-ui/core/styles';
import { useDispatch } from 'react-redux';

import {
  MediaServerInfo,
  MediaServerUser,
  Library,
} from '../../../../shared/models/mediaServer';
import { Wizard } from '../../../../shared/models/wizard';

import EmbyStatSelect from '../../../../shared/components/inputs/select/EmbyStatSelect';
import MediaServerHeader from '../../../../shared/components/mediaServerHeader';
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
    setSelectedAdmin(event.target.value);
  };

  const generateServerAddress = (): string => {
    const protocol = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    return `${protocol}${wizard.serverAddress}:${wizard.serverPort}${wizard?.serverBaseurl ?? ''}`
  };

  return (
    <Grid item xs={12} className="m-t-32">
      <MediaServerHeader
        serverType={wizard.serverType}
        serverInfo={serverInfo}
        serverAddress={generateServerAddress()} />
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
