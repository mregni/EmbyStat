import React, { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import Typography from '@material-ui/core/Typography';
import Grid from '@material-ui/core/Grid';

import SettingsCard from '../SettingsCard';
import { Library } from '../../../shared/models/mediaServer';
import { RootState } from '../../../store/RootReducer';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { getLibraries } from '../../../shared/services/MediaServerService';
import Loading from '../../../shared/components/loading';
import LibrarySelector from '../../../shared/components/librarySelector';
import getFullMediaServerUrl from '../../../shared/utils/GetFullMediaServerUtil';

interface Props {
  delay: number;
}

const MovieLibraryCard = (props: Props) => {
  const { delay } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const settings = useSelector((state: RootState) => state.settings);
  const [libraries, setLibraries] = useState([] as Library[]);
  const [selectedLibraries, setSelectedLibraries] = useState(settings.movieLibraries);
  const [isLoading, setIsLoading] = useState(true);


  useEffect(() => {
    const loadLibraries = async () => {
      const libs = await getLibraries();
      if (libs == null) {
        SnackbarUtils.error(t('COMMON.LIBRARYERROR'));
        setIsLoading(false);
        return;
      }

      setLibraries(libs);
      setIsLoading(false);
    }

    loadLibraries();
  }, [t]);

  const saveList = () => {
    const newSettings = { ...settings };
    newSettings.movieLibraries = selectedLibraries;
    dispatch(saveSettings(newSettings));
    SnackbarUtils.info(t('SETTINGS.MOVIE.LIBRARY.SAVING'));
  }

  return (
    <SettingsCard
      delay={delay}
      title={t('COMMON.LIBRARIES')}
      saveForm={saveList}
    >
      <Grid item>
        <Typography>
          {t('SETTINGS.MOVIE.COLLECTIONTYPES', { type: settings.mediaServer.serverType === 0 ? "Emby" : "Jellyfin" })}
        </Typography>
      </Grid>
      <Loading
        className="m-t-32"
        loading={isLoading}
        label={t('COMMON.LOADING', { item: t('COMMON.LIBRARIES') })}
        allLibraries={libraries}
        libraries={settings.movieLibraries}
        address={getFullMediaServerUrl(settings)}
        saveList={(libraries) => setSelectedLibraries(libraries)}
        Component={LibrarySelector}
      />
    </SettingsCard>
  )
}

export default MovieLibraryCard
