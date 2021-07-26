import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { Trans, useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';

import { RootState } from '../../../../store/RootReducer';
import { Wizard } from '../../../../shared/models/wizard';
import {
  updateSelectedLibraries,
  setMovieLibraryStepLoaded,
  setShowLibraryStepLoaded,
} from '../../../../store/WizardSlice';
import LibrarySelector from '../../../../shared/components/librarySelector';

interface Props {
  type: string;
}

const getFullAddress = (wizard: Wizard): string => {
  const protocol = wizard.serverProtocol === 0 ? 'https://' : 'http://';
  const baseUrl = wizard.serverBaseUrlNeeded ? wizard.serverBaseurl : '';
  return `${protocol}${wizard.serverAddress}:${wizard.serverPort}${baseUrl}`;
};

const ConfigureLibraries = (props: Props) => {
  const { type } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [selectedLibraries, setSelectedLibraries] = useState([] as string[]);
  const [listLoaded, setListLoaded] = useState(false);
  const wizard = useSelector((state: RootState) => state.wizard);

  useEffect(() => {
    const typeNumber = type === 'movie' ? 1 : 2;
    if (!listLoaded) {
      let list: string[] = [];
      if (typeNumber === 1) {
        list = false
          ? wizard.movieLibraries
          : wizard.allLibraries
            .filter((x) => x.type === typeNumber)
            .map((x) => x.id);
        dispatch(setMovieLibraryStepLoaded(true));
      } else if (typeNumber === 2) {
        list = false
          ? wizard.showLibraries
          : wizard.allLibraries
            .filter((x) => x.type === typeNumber)
            .map((x) => x.id);
        dispatch(setShowLibraryStepLoaded(true));
      }

      setSelectedLibraries(list);
      dispatch(updateSelectedLibraries(list, type));
    }
    setListLoaded(true);
  }, [wizard, type, listLoaded, dispatch]);

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        <Trans i18nKey="WIZARD.CONFIGURELIBRARIES" values={{ type }} />
      </Typography>
      <Typography variant="body1" className="m-t-16 m-b-16">
        <Trans
          i18nKey="WIZARD.SELECTLIBRARIES"
          values={
            type === 'movie'
              ? { type: t('COMMON.MOVIE') }
              : { type: t('COMMON.SHOW') }
          }
        />
      </Typography>
      <LibrarySelector
        allLibraries={wizard.allLibraries}
        libraries={selectedLibraries}
        address={getFullAddress(wizard)}
        saveList={(libraries) => dispatch(updateSelectedLibraries(libraries, type))}
      />
    </Grid>
  );
};

export default ConfigureLibraries;
