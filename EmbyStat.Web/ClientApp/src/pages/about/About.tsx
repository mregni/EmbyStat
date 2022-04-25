import React, {useContext} from 'react';
import {useTranslation} from 'react-i18next';

import {Chip, Link, Stack, Typography} from '@mui/material';

import {EsTitle} from '../../shared/components/esTitle';
import {SettingsContext} from '../../shared/context/settings';
import {useAbout} from './hooks';

export function About() {
  const {settings} = useContext(SettingsContext);
  const {about, loaded} = useAbout();
  const {t} = useTranslation();

  if (!loaded) {
    return (<></>);
  }

  const links = [
    {url: window.runConfig.featureupvoteUrl, label: 'ABOUT.REQUESTFEATURE'},
    {url: window.runConfig.crowdinUrl, label: 'ABOUT.HELPTRANSLATE'},
    {url: window.runConfig.githubUrl, label: 'ABOUT.LOGBUG'},
    {url: window.runConfig.githubReleaseUrl, label: 'ABOUT.GITHUBRELEASE'},
  ];

  return (
    <Stack direction="column" spacing={2}>
      <EsTitle content="ABOUT.SERVERINFO" isFirst={true} />
      <Stack spacing={1}>
        <Typography>
        EmbyStat {t('COMMON.VERSION')}: {settings.version}
        </Typography>
        <Typography>
          {t('ABOUT.OS')}: { about.operatingSystem } {about.architecture} { about.operatingSystemVersion }
        </Typography>
      </Stack>
      <EsTitle content="ABOUT.THANKS.TITLE" />
      <Stack spacing={2}>
        <Typography>
          {t('ABOUT.THANKS.TEXT')}
        </Typography>
        <Stack direction="row" spacing={1}>
          <Chip label="alz41" />
          <Chip label="ben-nl" />
          <Chip label="Floflobel" />
          <Chip label="jeankadang" />
          <Chip label="jm199905" />
          <Chip label="Nickbert7" />
          <Chip label="Sebastián Fenoglio" />
        </Stack>
        <Typography variant="subtitle2">
          {t('ABOUT.THANKS.MISSINGNAME')}
        </Typography>
      </Stack>
      <EsTitle content="ABOUT.ACTIONS" />
      <Stack direction="row" spacing={2}>
        {links.map((link) => (
          <Link
            key={link.url}
            href={link.url}
            target="_blank"
            color="primary"
          >
            {t(link.label)}
          </Link>),
        )}
      </Stack>
    </Stack>
  );
}

{/* <mat-list-item>
  <a href="{{ environment.urls.featureupvote }}" mat-raised-button color="primary"
    target="_blank">{{ 'ABOUT.REQUESTFEATURE' | translate }}</a>
</mat-list-item>
<mat-list-item>
  <a href="{{ environment.urls.github }}" mat-raised-button color="primary"
    target="_black">{{ 'ABOUT.LOGBUG' | translate }}</a>
</mat-list-item>
<mat-list-item>
  <a href="{{ environment.urls.crowdin }}" mat-raised-button color="primary"
      target="_blank">{{ 'ABOUT.HELPTRANSLATE' | translate }}</a>
</mat-list-item>
<mat-list-item>
  <a href="{{ environment.urls.githubRelease }}" mat-raised-button color="primary"
      target="_blank">{{ 'ABOUT.GITHUBRELEASE' | translate }}</a>
</mat-list-item> */}
