import React, { useState, useEffect } from 'react';
import DataGrid, {
  Scrolling,
  Paging,
  Column,
  Sorting,
  MasterDetail,
} from 'devextreme-react/data-grid';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';

import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import Zoom from '@material-ui/core/Zoom';
import Chip from '@material-ui/core/Chip';
import { makeStyles } from '@material-ui/core/styles';
import uuid from 'react-uuid';
import { useSelector } from 'react-redux';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useTranslation } from 'react-i18next';

import CustomStore from 'devextreme/data/custom_store';
import DetailMovieTemplate from './DetailMovieTemplate';
import Flag from '../../../shared/components/flag';
import FilterDrawer from '../../../shared/components/filterDrawer';
import movieFilters from '../../../shared/filters/movieFilters';
import { ActiveFilter } from '../../../shared/models/filter';
import { getItemDetailLink } from '../../../shared/utils/MediaServerUrlUtil';
import { RootState } from '../../../store/RootReducer';
import { useServerType } from '../../../shared/hooks';
import calculateFileSize from '../../../shared/utils/CalculateFileSize';

const useStyles = makeStyles((theme) => ({
  container: {
    height: 'calc(100vh - 180px)',
  },
  title: {
    marginRight: 16,
  },
  button__padding: {
    paddingTop: 5,
  },
}));

interface Props { }

const MovieList = (props: Props) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [dataSource, setDataSource] = useState<CustomStore>();
  const settings = useSelector((state: RootState) => state.settings);
  const serverType = useServerType();

  const handleFilterHide = (id) => {
    const currentFilterIndex = activeFilters.findIndex((x) => x.id === id);
    if (currentFilterIndex !== -1) {
      const newFilters = [...activeFilters];
      newFilters[currentFilterIndex].visible = false;
      setActiveFilters(newFilters);
    }
  };

  useEffect(() => {
    setDataSource(
      AspNetData.createStore({
        key: 'id',
        loadUrl: '/api/movie/list',
        loadParams: {
          filter: JSON.stringify(
            activeFilters.map((x) => ({
              field: x.field,
              operation: x.operation,
              value: x.value,
            }))
          ),
        },
      })
    );
  }, [activeFilters]);

  const handleFilterDelete = (id) => {
    setActiveFilters(activeFilters.filter((x) => x.id !== id));
  };

  const addFilter = (filter: ActiveFilter) => {
    setActiveFilters((state) => [...state, filter]);
  };

  const calculateRunTimeValue = (data) => {
    return `${data.runTime} min`;
  };

  const getTitleValue = (data) => {
    return data.originalTitle;
  };

  const getGenresValues = (data) => {
    return data.genres.join(', ');
  };

  const getSizeInMbValue = (data) => {
    return calculateFileSize(data.sizeInMb)
  }

  const getSubtitleValues = (row) => {
    return (
      <Grid container>
        {row.data.subtitles.slice(0, 5).map((x) => (
          <Grid item key={uuid()} className="m-r-4">
            <Flag language={x} />
          </Grid>
        ))}
        {row.data.subtitles.length > 5 ? (
          <Grid item>+ {row.data.subtitles.length - 5}</Grid>
        ) : null}
      </Grid>
    );
  };

  const getAudioValues = (row) => {
    return (
      <Grid container>
        {row.data.audioLanguages.slice(0, 5).map((x) => (
          <Grid item key={uuid()} className="m-r-4">
            <Flag language={x} />
          </Grid>
        ))}
        {row.data.audioLanguages.length > 5 ? (
          <Grid item>+ {row.data.audioLanguages.length - 5}</Grid>
        ) : null}
      </Grid>
    );
  };

  const renderLinks = (row) => {
    return (
      <Grid container direction="row" justify="flex-end" alignItems="center">
        <Button
          variant="outlined"
          color="secondary"
          size="small"
          href={getItemDetailLink(settings, row.data.id)}
          target="_blank"
          startIcon={<OpenInNewIcon />}
          classes={{
            outlinedSizeSmall: classes.button__padding
          }}
        >
          {serverType}
        </Button>
      </Grid>
    );
  };

  const generateLabel = (filter: ActiveFilter): string => {
    return filter.fieldLabel
      .replace(/\{0\}/g, t(filter.operationLabel))
      .replace(/\{1\}/g, filter.valueLabel);
  };

  return (
    <Grid container direction="column">
      <FilterDrawer
        filterCount={activeFilters.length}
        addFilter={addFilter}
        filterDefinitions={movieFilters}
        clearFilters={() => setActiveFilters([])}
      />
      <Grid item container direction="row" className="p-b-16">
        <Grid item>
          <Typography variant="h5" className={classes.title}>
            {t('COMMON.MOVIES')}
          </Typography>
        </Grid>
        {activeFilters.map((filter: any) => (
          <Grid item key={filter.id}>
            <Zoom
              in={filter.visible}
              onExited={() => handleFilterDelete(filter.id)}
            >
              <Chip
                className="m-r-16"
                label={generateLabel(filter)}
                onDelete={() => handleFilterHide(filter.id)}
              />
            </Zoom>
          </Grid>
        ))}
      </Grid>
      <Grid item>
        <DataGrid
          elementAttr={{
            class: classes.container,
          }}
          dataSource={{ store: dataSource }}
          showBorders={true}
          remoteOperations={true}
          wordWrapEnabled={true}
          rowAlternationEnabled={true}
          allowColumnResizing={true}
          columnAutoWidth={true}
          columnResizingMode="nextColumn"
        >
          <Scrolling mode="virtual" rowRenderingMode="virtual" />
          <Paging pageSize="100" />
          <Sorting mode="single" />

          <Column
            dataField="sortName"
            caption={t('COMMON.TITLE')}
            width="300"
            calculateCellValue={getTitleValue}
            defaultSortIndex={0}
            defaultSortOrder="asc"
          />
          <Column
            caption={t('COMMON.GENRES')}
            calculateCellValue={getGenresValues}
            allowSorting={false}
          />
          <Column dataField="container" caption="Container" />
          <Column
            dataField="runTimeTicks"
            caption={t('COMMON.RUNTIME')}
            dataType="number"
            calculateCellValue={calculateRunTimeValue}
          />
          <Column
            caption={t('COMMON.OFFICIALRATING')}
            dataField="officialRating"
          />
          <Column
            caption={t('COMMON.HEIGHT')}
            dataField="height"
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.WIDTH')}
            dataField="width"
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.BITRATE') + " (Mbps)"}
            dataField="bitRate"
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.SIZEINMB')}
            calculateCellValue={getSizeInMbValue}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.BITDEPTH')}
            dataField="bitDepth"
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.CODEC')}
            dataField="codec"
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.VIDEORANGE')}
            dataField="videoRange"
            allowSorting={false}
          />
          <Column caption={t('COMMON.RATING')} dataField="communityRating" />
          <Column
            caption={t('COMMON.SUBTITLES')}
            width={180}
            cellRender={getSubtitleValues}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.AUDIO')}
            width={100}
            cellRender={getAudioValues}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.LINKS')}
            cellRender={renderLinks}
            allowSorting={false}
            alignment="right"
          />
          <MasterDetail enabled={true} component={DetailMovieTemplate} />
        </DataGrid>
      </Grid>
    </Grid>
  );
};

export default MovieList;
