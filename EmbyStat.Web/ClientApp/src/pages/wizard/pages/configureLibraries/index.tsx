import React, { useState, useEffect } from 'react'
import { Grid, Typography, Paper, Zoom, makeStyles } from '@material-ui/core';
import { Trans, useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import classNames from 'classnames';

import { RootState } from '../../../../store/RootReducer';
import { Library } from '../../../../shared/models/mediaServer';
import { Wizard } from '../../../../shared/models/wizard';
import { updateSelectedLibraries, setMovieLibraryStepLoaded, setShowLibraryStepLoaded } from '../../../../store/WizardSlice';
import CheckCircleOutlineRoundedIcon from '@material-ui/icons/CheckCircleOutlineRounded';
import RadioButtonUncheckedRoundedIcon from '@material-ui/icons/RadioButtonUncheckedRounded';
import NoImage from '../../../../shared/assets/images/no-image.png';

const useStyles = makeStyles((theme) => ({
  library: {
    padding: 0,
    position: 'relative',
    '&:hover img': {
      opacity: 1,
    }
  },
  library__paper: {
    marginRight: 16,
    marginTop: 16,
  },
  library__image: {
    cursor: 'pointer',
    width: '100%',
    borderRadius: 4,
    opacity: 0.25,
  },
  'library__image--selected': {
    opacity: '1 !important',
  },
  library__buttons: {
    position: 'absolute',
    right: 0,
    borderRadius: 4,
    bottom: 0,
    paddingTop: 5,
    paddingRight: 5,
    paddingLeft: 5,
    paddingBottom: 2,
    marginRight: 16,
    width: 'calc(100% - 16px)',
    zIndex: 10,
    display: 'flex',
    flex: 1,
    justifyContent: 'space-between',
    background: 'linear-gradient(0deg, rgba(0, 0, 0, 1) 67%, rgba(255, 255, 255, 0) 100%)',

    '&:hover': {
      cursor: 'pointer',
    }
  }
}));

interface Props {
  type: string,
}

const getFullAddress = (wizard: Wizard): string => {
  const protocol = wizard.serverProtocol === 0 ? 'https://' : 'http://';
  const baseUrl = wizard.serverBaseUrlNeeded ? wizard.serverBaseurl : '';
  return `${protocol}${wizard.serverAddress}:${wizard.serverPort}${baseUrl}`;
}

const ConfigureLibraries = (props: Props) => {
  const classes = useStyles();
  const { type } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [libraries, setLibraries] = useState([] as Library[]);
  const [selectedLibraries, setSelectedLibraries] = useState([] as string[]);
  const [listLoaded, setListLoaded] = useState(false);
  const wizard = useSelector((state: RootState) => state.wizard);

  useEffect(() => {
    setLibraries(wizard.allLibraries);
    const typeNumber = type === 'movie' ? 1 : 2;
    if (!listLoaded) {
      let list: string[] = [];
      if (typeNumber === 1) {
        list = wizard.loadedMovieLibraryStep ?
          wizard.movieLibraries :
          wizard.allLibraries.filter(x => x.type === typeNumber).map(x => x.id);
        dispatch(setMovieLibraryStepLoaded(true));
      } else if (typeNumber === 2) {
        list = wizard.loadedShowLibraryStep ?
          wizard.showLibraries :
          wizard.allLibraries.filter(x => x.type === typeNumber).map(x => x.id);
        dispatch(setShowLibraryStepLoaded(true));
      }
      setSelectedLibraries(list);
      dispatch(updateSelectedLibraries(list, type));
    }
    setListLoaded(true);
  }, [wizard, type, listLoaded, dispatch]);

  const librarySelected = (libraryId: string): boolean => {
    return selectedLibraries.indexOf(libraryId, 0) > -1;
  }

  const selectLibrary = (libraryId: string): void => {
    const index = selectedLibraries.indexOf(libraryId, 0);
    let newList = [...selectedLibraries];
    if (index > -1) {
      newList = selectedLibraries.filter(x => x !== libraryId);
    } else {
      newList.push(libraryId);
    }

    setSelectedLibraries(newList);
    dispatch(updateSelectedLibraries(newList, type));
  }

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.CONFIGURELIBRARIES" values={{ type: type }} />
      </Typography>
      <Typography variant="body1" className="m-t-16 m-b-16">
        <Trans i18nKey="WIZARD.SELECTLIBRARIES" values={type === 'movie' ? { type: t('COMMON.MOVIE') } : { type: t('COMMON.SHOW') }} />
      </Typography>
      <Grid item container direction="row" justify="flex-start">
        {libraries.map((lib: Library, i: number) =>
          <Zoom in={true} key={lib.id} style={{ transitionDelay: (25 * i) + 100 + 'ms' }}>
            <Grid
              item
              sm={4}
              xs={6}
              md={3}
              xl={2}
              className={classes.library}
              onClick={() => selectLibrary(lib.id)}>
              <Paper elevation={5} className={classes.library__paper}>
                {lib.primaryImage !== null ?
                  <img
                    className={classNames(classes.library__image, { [classes['library__image--selected']]: librarySelected(lib.id) })}
                    alt="library"
                    src={`${getFullAddress(wizard)}/emby/Items/${lib.id}/Images/Primary?maxHeight=212&maxWidth=377&tag=${lib.primaryImage}&quality=90`} />
                  : <img
                    className={classNames(classes.library__image, { [classes['library__image--selected']]: librarySelected(lib.id) })}
                    alt="no tag found"
                    src={NoImage} />}
                <div className={classes.library__buttons}>
                  <span>{lib.name}</span>
                  {librarySelected(lib.id) ? <CheckCircleOutlineRoundedIcon /> : <RadioButtonUncheckedRoundedIcon />}
                </div>
              </Paper>
            </Grid>
          </Zoom>
        )}
      </Grid>
    </Grid >
  )
}

export default ConfigureLibraries
