import React, { useState } from 'react'
import { makeStyles } from '@material-ui/core/styles';
import Paper from '@material-ui/core/Paper';
import Zoom from '@material-ui/core/Zoom';
import Grid from '@material-ui/core/Grid';
import CheckCircleOutlineRoundedIcon from '@material-ui/icons/CheckCircleOutlineRounded';
import RadioButtonUncheckedRoundedIcon from '@material-ui/icons/RadioButtonUncheckedRounded';
import classNames from 'classnames';

import NoImage from '../../assets/images/no-image.png';
import { Library } from '../../models/mediaServer';

const useStyles = makeStyles((theme) => ({
  library: {
    padding: 0,
    position: 'relative',
    '&:hover img': {
      opacity: 1,
    },
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
    background:
      'linear-gradient(0deg, rgba(0, 0, 0, 1) 67%, rgba(255, 255, 255, 0) 100%)',

    '&:hover': {
      cursor: 'pointer',
    },
  },
}));

interface Props {
  allLibraries: Library[];
  libraries: string[];
  address: string;
  saveList: (libraries: string[]) => void;
}

const LibrarySelector = (props: Props) => {
  const classes = useStyles();
  const { allLibraries, libraries, address, saveList } = props;
  const [selectedLibraries, setSelectedLibraries] = useState(libraries);

  const librarySelected = (libraryId: string): boolean => {
    return selectedLibraries.indexOf(libraryId, 0) > -1;
  };

  const selectLibrary = (libraryId: string): void => {
    const index = selectedLibraries.indexOf(libraryId, 0);
    let newList = [...selectedLibraries];
    if (index > -1) {
      newList = selectedLibraries.filter((x) => x !== libraryId);
    } else {
      newList.push(libraryId);
    }

    setSelectedLibraries(newList);
    saveList(newList)
  };

  return (
    <Grid item container direction="row" justify="flex-start">
      {allLibraries.map((lib: Library, i: number) => (
        <Zoom
          in={true}
          key={lib.id}
          style={{ transitionDelay: `${25 * i + 100}ms` }}
        >
          <Grid
            item
            xs={6}
            sm={4}
            md={3}
            lg={3}
            xl={2}
            className={classes.library}
            onClick={() => selectLibrary(lib.id)}
          >
            <Paper elevation={5} className={classes.library__paper}>
              {lib.primaryImage !== null ? (
                <img
                  className={classNames(classes.library__image, {
                    [classes['library__image--selected']]: librarySelected(
                      lib.id
                    ),
                  })}
                  alt="library"
                  src={`${address}/emby/Items/${
                    lib.id
                    }/Images/Primary?maxHeight=212&maxWidth=377&tag=${
                    lib.primaryImage
                    }&quality=90`}
                />
              ) : (
                  <img
                    className={classNames(classes.library__image, {
                      [classes['library__image--selected']]: librarySelected(
                        lib.id
                      ),
                    })}
                    alt="no tag found"
                    src={NoImage}
                  />
                )}
              <div className={classes.library__buttons}>
                <span>{lib.name}</span>
                {librarySelected(lib.id) ? (
                  <CheckCircleOutlineRoundedIcon />
                ) : (
                    <RadioButtonUncheckedRoundedIcon />
                  )}
              </div>
            </Paper>
          </Grid>
        </Zoom>
      ))}
    </Grid>
  )
}

export default LibrarySelector
