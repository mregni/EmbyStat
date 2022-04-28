import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';
import {Outlet} from 'react-router';

import {EsNoMedia} from '../../shared/components/blockers';
import {EsJobRunning} from '../../shared/components/blockers/EsJobRunning';
import {EsLoading} from '../../shared/components/esLoading';
import {ShowsContext, ShowsContextProvider} from '../../shared/context/shows';
import {showJobId} from '../../shared/utils';

function ShowsContainer() {
  const {loaded, mediaPresent, load} = useContext(ShowsContext);
  const {t} = useTranslation();

  return (
    <EsLoading label={t('SHOWS.LOADER')} loading={!loaded}>
      <EsJobRunning
        jobId={showJobId}
        title="DIALOGS.SHOWJOBRUNNING.TITLE"
        body="DIALOGS.SHOWJOBRUNNING.BODY"
        finishedAction={load}
      >
        <EsNoMedia
          mediaPresent={mediaPresent}
          jobId={showJobId}
          title="DIALOGS.NOSHOWTYPEFOUND.TITLE"
          body="DIALOGS.NOSHOWTYPEFOUND.BODY"
        >
          <Outlet />
        </EsNoMedia>
      </EsJobRunning>
    </EsLoading>
  );
}

export function Shows() {
  return (
    <ShowsContextProvider>
      <ShowsContainer />
    </ShowsContextProvider>
  );
}
