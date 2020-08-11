import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';
import Grid from '@material-ui/core/Grid';
import Zoom from '@material-ui/core/Zoom';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
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
  setServerAddress,
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
  italic: {
    fontStyle: 'italic',
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

  const [selectedAddress, setSelectedAddress] = useState(generateServerAddress());
  const handleAddressChange = (event: React.ChangeEvent<{ value: unknown }>) => {
    setSelectedAddress(event.target.value as string);
    var addressArray = (event.target.value as string).split('://');
    const protocol = addressArray[0] === 'https' ? 0 : 1;
    const address = addressArray[1].split(':')[0];
    const port = parseInt(addressArray[1].split(':')[1].split('/')[0], 10);
    dispatch(setServerAddress(address, port, protocol));
  };

  return (
    <Grid item container xs={12} className="m-t-32" direction="column">
      <Grid item>
        <MediaServerHeader
          serverType={wizard.serverType}
          serverInfo={serverInfo}
          serverAddress={generateServerAddress()} />
      </Grid>
      <Grid item>
        <Zoom in={true} style={{ transitionDelay: '300ms' }}>
          <Card
            elevation={7}
            square
            className={classNames(classes.root, 'm-t-32')}
          >
            <CardContent>
              <Grid container direction="column">
                <Grid item>
                  {t('WIZARD.LANWANCHOISE', { type: wizard.serverType === 0 ? "Emby" : "Jellyfin" })}
                </Grid>
                <Grid item>
                  <Select
                    className="m-t-16"
                    value={selectedAddress}
                    variant="standard"
                    onChange={handleAddressChange}
                  >
                    {
                      generateServerAddress() !== serverInfo.wanAddress && generateServerAddress() !== serverInfo.localAddress
                        ? <MenuItem value={generateServerAddress()}>Current&nbsp;<span className={classes.italic}>({generateServerAddress()})</span></MenuItem>
                        : null
                    }
                    <MenuItem value={serverInfo.wanAddress}>WAN&nbsp;<span className={classes.italic}>({serverInfo.wanAddress})</span></MenuItem>
                    <MenuItem value={serverInfo.localAddress}>LAN&nbsp;<span className={classes.italic}>({serverInfo.localAddress})</span></MenuItem>
                  </Select>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Zoom>
      </Grid>
      <Grid item>
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
    </Grid>
  );
};

export default TestSuccessFul;
