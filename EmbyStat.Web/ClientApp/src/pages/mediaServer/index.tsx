import React, { useEffect, useState } from 'react'
import { useSelector } from 'react-redux';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import Zoom from '@material-ui/core/Zoom';
import { makeStyles } from '@material-ui/core/styles';

import { getServerInfo, getPlugins } from '../../shared/services/MediaServerService';
import { MediaServerInfo, MediaServerPlugin } from '../../shared/models/mediaServer';
import MediaServerHeader from '../../shared/components/mediaServerHeader';
import { RootState } from '../../store/RootReducer';
import construction from '../../shared/assets/images/under-construction.webp';
import getFullMediaServerUrl from "../../shared/utils/GetFullMediaServerUtil";
import fallbackImg from "../../shared/assets/images/no-image.png";

const useStyles = makeStyles((theme) => ({
  image: {
    borderTopRightRadius: 4,
    borderTopLeftRadius: 4,
  },
  version: {
    fontStyle: 'italic',
    fontSize: '0.8rem',
    paddingBottom: 8
  },
}));

interface Props {

}

const MediaServer = (props: Props) => {
  const [serverInfo, setServerInfo] = useState<MediaServerInfo | null>(null);
  const [plugins, setPlugins] = useState<MediaServerPlugin[] | null>(null);
  const settings = useSelector((state: RootState) => state.settings);
  const classes = useStyles();

  useEffect(() => {
    const fetchInfo = async () => {
      setServerInfo(await getServerInfo(false));
      setPlugins(await getPlugins());
    }

    fetchInfo();
  }, []);

  const addDefaultSrc = (e) => {
    e.target.src = fallbackImg
  }

  return (
    <Grid container direction="column" spacing={2}>
      <Grid item>
        <img src={construction} alt="under construction" width={100} />
      </Grid>

      {serverInfo != null
        ? <Grid item>
          <MediaServerHeader
            serverType={settings?.mediaServer.serverType}
            serverInfo={serverInfo}
            serverAddress={serverInfo?.wanAddress} />
        </Grid>
        : null}
      {
        plugins != null
          ?
          <Grid container spacing={2} item>
            {plugins.map((plugin: MediaServerPlugin, i: number) =>
              <Grid
                item
                key={plugin.id}
                xl={2}
                md={3}
                sm={6}
                xs={12}
              >
                <Zoom in={true} style={{ transitionDelay: `${10 * i + 100}ms` }}>
                  <Paper elevation={5}>
                    <Grid container direction="column" alignItems="center">
                      <Grid item container>
                        <img
                          className={classes.image}
                          src={`${getFullMediaServerUrl(settings)}/emby/plugins/${plugin.id}/thumb`}
                          alt="plugin logo"
                          onError={addDefaultSrc}
                          style={{ width: '100%' }}
                        />
                      </Grid>
                      <Grid item className="m-t-8">
                        {plugin.name}
                      </Grid>
                      <Grid item className={classes.version}>
                        {plugin.version}
                      </Grid>
                    </Grid>
                  </Paper>
                </Zoom>
              </Grid>)}
          </Grid>
          : null
      }
    </Grid>
  )
}

export default MediaServer
