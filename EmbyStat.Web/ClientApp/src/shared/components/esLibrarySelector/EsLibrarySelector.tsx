import React, {useContext, useEffect, useState} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Avatar, Box, Grid, Theme, Typography} from '@mui/material';

import {LibrariesContext, LibrariesContextProvider} from '../../context/library';
import {useMediaServerUrls} from '../../hooks';
import {Library} from '../../models/library';
import {EsSaveCard} from '../cards';

type LibraryCardProps = {
  library: Library;
  fallbackServerUrl?: string;
}

export const EsLibraryCard = (props: LibraryCardProps) => {
  const {library, fallbackServerUrl = ''} = props;
  const {toggleLibrary, selected} = useContext(LibrariesContext);
  const {getPrimaryImageLink} = useMediaServerUrls();
  const [isSelected, setIsSelected] = useState(false);

  useEffect(() => {
    setIsSelected(selected.indexOf(library.id) !== -1);
  }, [selected]);

  return (
    <Grid
      item
      xs={12}
      md={6}
      lg={4}
      sx={{cursor: 'pointer'}}
      onClick={() => toggleLibrary(library.id)}
    >
      <Box
        sx={{position: 'relative'}}
      >
        <Avatar
          alt={library.name}
          sx={{
            'borderRadius': 1,
            'width': '100%',
            'opacity': isSelected ? 1 : 0.3,
            'minHeight': '135px',
            'boxShadow': !isSelected ? (theme: Theme) => theme.shadows[4] : 'none',
            '&:hover': {
              'opacity': 1,
            },
          }}
          src={getPrimaryImageLink(library.id, library.primary, fallbackServerUrl)}
        />
        <Box sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: '100%',
          // eslint-disable-next-line max-len
          textShadow: '-1px -1px 0 Black,-1px 0px 0 Black,-1px 1px 0 Black,0px -1px 0 Black,0px 0px 0 Black,0px 1px 0 Black,1px -1px 0 Black,1px 0px 0 Black,1px 1px 0 Black',
        }}>
          <Typography variant="h6" align="center">
            {library.name}
          </Typography>
        </Box>
      </Box>
    </Grid>
  );
};

type Props = {
  fetch: () => Promise<Library[]>;
  type: 'COMMON.SHOW' | 'COMMON.MOVIE';
  push: (libraryIds: string[]) => Promise<void>;
}

const EsLibrarySelectorContainer = (props: Props) => {
  const {fetch, type, push} = props;
  const {load, libraries, save} = useContext(LibrariesContext);
  const {handleSubmit} = useForm();
  const {t} = useTranslation();

  useEffect(() => {
    (async () => {
      load(fetch);
    })();
  }, [fetch]);

  const submitForm = async () => {
    await save(push);
  };

  return (
    <EsSaveCard
      title='SETTINGS.LIBRARIES.TITLE'
      saveForm={submitForm}
      handleSubmit={handleSubmit}
    >
      <Typography variant="body1">
        {t('SETTINGS.LIBRARIES.CONTENT', {type: t(type)})}
      </Typography>
      <Box>
        <Grid container spacing={1}>
          {
            libraries.map((lib) => (<EsLibraryCard key={lib.id} library={lib} />))
          }
        </Grid>
      </Box>
    </EsSaveCard>
  );
};

export const EsLibrarySelector = (props: Props) => {
  return (
    <LibrariesContextProvider>
      <EsLibrarySelectorContainer {...props} />
    </LibrariesContextProvider>
  );
};

