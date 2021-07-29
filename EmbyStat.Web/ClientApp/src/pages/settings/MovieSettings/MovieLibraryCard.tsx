import React, { useContext } from 'react'
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';

import { LibraryCard } from '../Helpers';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { SettingsContext } from '../../../shared/context/settings';
interface Props {
  delay: number;
}

export const MovieLibraryCard = (props: Props) => {
  const { delay } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const { settings } = useContext(SettingsContext);

  const saveList = (selectedLibraries: string[]): void => {
    const newSettings = { ...settings };
    newSettings.movieLibraries = selectedLibraries;
    dispatch(saveSettings(newSettings));
    SnackbarUtils.info(t('SETTINGS.MOVIE.LIBRARY.SAVING'));
  }

  return (
    <LibraryCard
      delay={delay}
      list={settings.movieLibraries}
      saveList={saveList}
    />
  )
}
