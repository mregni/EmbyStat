import React, { useContext } from 'react'
import { useTranslation } from 'react-i18next';

import { LibraryCard } from '../Helpers';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { SettingsContext } from '../../../shared/context/settings';

interface Props {
  delay: number;
}

export const ShowLibraryCard = (props: Props) => {
  const { delay } = props;
  const { t } = useTranslation();
  const { settings, save } = useContext(SettingsContext);

  const saveList = (selectedLibraries: string[]): void => {
    settings.showLibraries = selectedLibraries;
    save(settings)
    SnackbarUtils.info(t('SETTINGS.SHOWS.LIBRARY.SAVING'));
  }

  return (
    <LibraryCard
      delay={delay}
      list={settings.showLibraries}
      saveList={saveList}
    />
  )
}
