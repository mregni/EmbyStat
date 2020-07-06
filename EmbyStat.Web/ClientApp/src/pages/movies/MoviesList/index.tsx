import React, { useState, useRef, useEffect } from 'react'
import DataGrid, { Scrolling, Paging, Column, Sorting, MasterDetail } from 'devextreme-react/data-grid';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';
import { makeStyles, Grid, Typography, Zoom, Chip } from '@material-ui/core';
import uuid from 'react-uuid';

import imdb from '../../../shared/assets/icons/imdb.svg';
import tmdb from '../../../shared/assets/icons/tmdb.svg';
import DetailMovieTemplate from './DetailMovieTemplate';
import Flag from '../../../shared/components/flag';
import FilterDrawer from '../../../shared/components/filterDrawer';
import { useTranslation } from 'react-i18next';

import movieFilters from '../../../shared/filters/movieFilters';
import { ActiveFilter } from '../../../shared/models/filter';

const useStyles = makeStyles((theme) => ({
  container: {
    height: 'calc(100vh - 180px)',
  }
}));

interface Props {

}

const MovieList = (props: Props) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const dataGridRef = useRef(null);
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [dataSource, setDataSource] = useState(AspNetData.createStore({
    key: 'id',
    loadUrl: `/api/movie/list?libraryids=f137a2dd21bbc1b99aa5c0f6bf02a805`
  }));

  const handleFilterHide = (id) => {
    const currentFilterIndex = activeFilters.findIndex(x => x.id === id);
    if (currentFilterIndex !== -1) {
      const newFilters = [...activeFilters];
      newFilters[currentFilterIndex].visible = false;
      setActiveFilters(newFilters);
    }
  }

  useEffect(() => {
    const datagrid = dataGridRef != null ? dataGridRef.current as unknown as DataGrid : {} as DataGrid;
    datagrid.instance.refresh();

    let filterParameters = '';
    if (activeFilters.length > 0) {
      filterParameters = `&filters=${JSON.stringify(activeFilters.map(x => ({ field: x.field, operation: x.operation, value: x.value })))}`;
    }

    setDataSource(AspNetData.createStore({
      key: 'id',
      loadUrl: `/api/movie/list?libraryids=f137a2dd21bbc1b99aa5c0f6bf02a805${filterParameters}`
    }));
  }, [activeFilters])

  const handleFilterDelete = (id) => {
    setActiveFilters(activeFilters.filter(x => x.id !== id));
  }

  const addFilter = (filter: ActiveFilter) => {
    setActiveFilters((state) => ([...state, filter]));
  }

  const calculateRunTimeValue = (data) => {
    return `${data.runTime} min`;
  }

  const getTitleValue = (data) => {
    return data.originalTitle;
  }

  const getGenresValues = (data) => {
    return data.genres.join(', ');
  }

  const getSubtitleValues = (row) => {
    return <Grid container>
      {row.data.subtitles.slice(0, 5).map(x =>
        <Grid item key={uuid()} className="m-r-4">
          <Flag language={x} />
        </Grid>
      )}
      {
        row.data.subtitles.length > 5 ?
          <Grid item>
            + {row.data.subtitles.length - 5}
          </Grid> : null
      }
    </Grid>
  }

  const getAudioValues = (row) => {
    return <Grid container>
      {row.data.audioLanguages
        .slice(0, 5).map(x =>
          <Grid item key={uuid()} className="m-r-4">
            <Flag language={x} />
          </Grid>
        )}
      {
        row.data.audioLanguages.length > 5 ?
          <Grid item>
            + {row.data.audioLanguages.length - 5}
          </Grid> : null
      }
    </Grid>
  }

  const getResolutionValues = (data) => {
    return data.resolutions.join(', ');
  }

  const renderLinks = (row) => {
    return <Grid container direction="row" justify="flex-start" alignItems="center" spacing={2}>
      {
        row.data.imdb != null
          ? <Grid item>
            <a href={`https://www.imdb.com/title/${row.data.imdb}`} target="_blank" rel="noopener noreferrer">
              <img src={imdb} alt="Imdb icon" height="20" />
            </a>
          </Grid> : null
      }
      {
        row.data.tmdb != null
          ? <Grid item>
            <a href={`https://www.themoviedb.org/movie/${row.data.tmdb}`} target="_blank" rel="noopener noreferrer">
              <img src={tmdb} alt="Tmdb icon" height="15" />
            </a>
          </Grid> : null
      }
    </Grid>
  }

  const generateLabel = (filter: ActiveFilter): string => {
    return filter.fieldLabel
      .replace(/\{0\}/g, t(filter.operationLabel))
      .replace(/\{1\}/g, filter.valueLabel);
  }

  return (
    <Grid container direction="column" spacing={1}>
      <FilterDrawer
        filterCount={activeFilters.length}
        addFilter={addFilter}
        filterDefinitions={movieFilters}
        clearFilters={() => setActiveFilters([])} />
      <Grid item container direction="row" spacing={1}>
        <Grid item>
          <Typography variant='h5'>{t('COMMON.FILTERS')}: {activeFilters.length === 0 ? 'none' : null}</Typography>
        </Grid>
        {activeFilters.map((filter: any) => <Grid item key={filter.id}>
          <Zoom in={filter.visible} onExited={() => handleFilterDelete(filter.id)}>
            <Chip
              label={generateLabel(filter)}
              onDelete={() => handleFilterHide(filter.id)}
            />
          </Zoom>
        </Grid>)}
      </Grid>
      <Grid item>
        <DataGrid
          elementAttr={{
            class: classes.container
          }}
          ref={dataGridRef}
          dataSource={{ store: dataSource }}
          showBorders={true}
          remoteOperations={true}
          wordWrapEnabled={true}
          rowAlternationEnabled={true}
          allowColumnResizing={true}
          columnResizingMode={'nextColumn'}
        >
          <Scrolling mode="virtual" rowRenderingMode="virtual" />
          <Paging pageSize="100" />
          <Sorting mode="single" />

          <Column dataField="id" width="75" />
          <Column
            dataField="sortName"
            caption="Title"
            width="300"
            calculateCellValue={getTitleValue}
          />
          <Column dataField="container" caption="Container" width="100" />
          <Column
            dataField="runTimeTicks"
            caption="Run time"
            width="120"
            dataType="number"
            calculateCellValue={calculateRunTimeValue}
          />
          <Column caption="Genres" width="250" calculateCellValue={getGenresValues} allowSorting={false} />
          <Column dataField="officialRating" caption="Official rating" width="120" />
          <Column caption="Resolution" width="200" calculateCellValue={getResolutionValues} allowSorting={false} />
          <Column dataField="communityRating" caption="Rating" width="100" />
          <Column caption="Subtitles" width="200" cellRender={getSubtitleValues} allowSorting={false} />
          <Column caption="Audio" width="120" cellRender={getAudioValues} allowSorting={false} />
          <Column caption="Links" cellRender={renderLinks} allowSorting={false} />
          <MasterDetail
            enabled={true}
            component={DetailMovieTemplate}
          />
        </DataGrid>
      </Grid>
    </Grid>
  )
}

export default MovieList
