import React, { useState, useEffect, useContext } from 'react'
import { useTranslation } from 'react-i18next';
import Typography from '@material-ui/core/Typography';
import Grid from '@material-ui/core/Grid';

import { SettingsCard } from '.';
import { Library } from '../../../shared/models/mediaServer';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { getLibraries } from '../../../shared/services/MediaServerService';
import { Loading } from '../../../shared/components/loading';
import LibrarySelector from '../../../shared/components/librarySelector';
import getFullMediaServerUrl from '../../../shared/utils/MediaServerUrlUtil';
import { SettingsContext } from '../../../shared/context/settings';
import { getMediaServerTypeString } from '../../../shared/utils';
import { LibraryContainer } from '../../../shared/models/settings';

interface Props {
  delay: number;
  list: LibraryContainer[];
  saveList: (selectedLibraries: LibraryContainer[]) => void;
}

export const LibraryCard = (props: Props) => {
  const { delay, list, saveList } = props;
  const { t } = useTranslation();
  const { settings } = useContext(SettingsContext);
  const [libraries, setLibraries] = useState([] as Library[]);
  const [selectedLibraries, setSelectedLibraries] = useState<LibraryContainer[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setSelectedLibraries(list);
  }, [list])

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

  const saveForm = (): void => {
    saveList(selectedLibraries);
  }

  return (
    <SettingsCard
      delay={delay}
      title={t('COMMON.LIBRARIES')}
      saveForm={saveForm}
    >
      <Grid item>
        <Typography>
          {t('SETTINGS.MOVIE.COLLECTIONTYPES', { type: getMediaServerTypeString(settings) })}
        </Typography>
      </Grid>
      <Loading
        className="m-t-32"
        loading={isLoading}
        label={t('COMMON.LOADING', { item: t('COMMON.LIBRARIES') })}
      >
        <LibrarySelector
          allLibraries={libraries}
          libraries={list}
          address={getFullMediaServerUrl(settings)}
          saveList={(libraries) => setSelectedLibraries(libraries)}
        />
      </Loading>
    </SettingsCard>
  )
}
