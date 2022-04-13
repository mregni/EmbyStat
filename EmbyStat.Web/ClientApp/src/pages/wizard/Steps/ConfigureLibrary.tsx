import React, {forwardRef, useContext, useEffect, useImperativeHandle} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Grid, Stack, Typography} from '@mui/material';

import {EsLibraryCard} from '../../../shared/components/esLibrarySelector';
import {LibrariesContext, LibrariesContextProvider} from '../../../shared/context/library';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {Library} from '../../../shared/models/library';
import {fetchLibraries as fetchMovieLibraries} from '../../../shared/services/movieService';
import {fetchLibraries as fetchShowLibraries} from '../../../shared/services/showService';
import {LibraryStepProps, ValidationHandle} from '../Interfaces';

export const ConfigureLibrary =
forwardRef<ValidationHandle, LibraryStepProps>(function ConfigureLibrary(props, ref) {
  // eslint-disable-next-line react/prop-types
  const {type} = props;
  const {setMovieLibraryIds, setShowLibraryIds} = useContext(WizardContext);
  if (type === 'movie') {
    return (
      <LibrariesContextProvider>
        <ConfigureLibraryContainer
          fetch={fetchMovieLibraries}
          type="COMMON.MOVIE"
          push={setMovieLibraryIds}
          ref={ref}
        />
      </LibrariesContextProvider>
    );
  }

  return (
    <LibrariesContextProvider>
      <ConfigureLibraryContainer
        fetch={fetchShowLibraries}
        type="COMMON.SHOW"
        push={setShowLibraryIds}
        ref={ref} />
    </LibrariesContextProvider>
  );
});

type Props = {
  fetch: () => Promise<Library[]>;
  type: 'COMMON.SHOW' | 'COMMON.MOVIE';
  push: (libraryIds: string[]) => Promise<void>;
}

export const ConfigureLibraryContainer =
forwardRef<ValidationHandle, Props>(function ConfigureLibraryContainer(props, ref) {
  // eslint-disable-next-line react/prop-types
  const {fetch, type, push} = props;
  const {t} = useTranslation();
  const {load, libraries, save} = useContext(LibrariesContext);
  const {wizard} = useContext(WizardContext);

  useEffect(() => {
    (async () => {
      load(fetch);
    })();
  }, []);

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await save(push);
      return Promise.resolve(true);
    },
  }));

  return (
    <Stack spacing={2}>
      <Typography variant="h4" color="primary">
        {t('WIZARD.LIBRARIES.TITLE', {type: t(type)})}
      </Typography>
      <Typography variant="body1">
        {t('SETTINGS.LIBRARIES.CONTENT', {type: t(type)})}
      </Typography>
      <Box>
        <Grid container spacing={1}>
          {
            libraries.map((lib) => (
              <EsLibraryCard
                key={lib.id}
                library={lib}
                fallbackServerUrl={wizard.address}
              />))
          }
        </Grid>
      </Box>
    </Stack>
  );
});
