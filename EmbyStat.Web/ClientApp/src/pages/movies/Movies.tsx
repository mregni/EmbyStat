import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';
import {Outlet} from 'react-router';

import {EsNoMedia} from '../../shared/components/blockers';
import {EsJobRunning} from '../../shared/components/blockers/EsJobRunning';
import {EsLoading} from '../../shared/components/esLoading';
import {MoviesContext, MoviesContextProvider} from '../../shared/context/movies';
import {movieJobId} from '../../shared/utils';

function MoviesContainer() {
  const {loaded, mediaPresent, load} = useContext(MoviesContext);
  const {t} = useTranslation();

  return (
    <EsLoading label={t('MOVIES.LOADER')} loading={!loaded}>
      <EsJobRunning
        jobId={movieJobId}
        title="DIALOGS.MOVIEJOBRUNNING.TITLE"
        body="DIALOGS.MOVIEJOBRUNNING.BODY"
        finishedAction={load}
      >
        <EsNoMedia
          mediaPresent={mediaPresent}
          jobId={movieJobId}
          title="DIALOGS.NOMOVIETYPEFOUND.TITLE"
          body="DIALOGS.NOMOVIETYPEFOUND.BODY"
        >
          <Outlet />
        </EsNoMedia>
      </EsJobRunning>
    </EsLoading>
  );
}

export function Movies() {
  return (
    <MoviesContextProvider>
      <MoviesContainer />
    </MoviesContextProvider>
  );
}
