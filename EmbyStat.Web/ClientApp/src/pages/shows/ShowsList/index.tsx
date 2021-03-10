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
import DetailShowTemplate from './DetailShowTemplate';
import Flag from '../../../shared/components/flag';
import FilterDrawer from '../../../shared/components/filterDrawer';
import showFilters from '../../../shared/filters/showFilters';
import { ActiveFilter } from '../../../shared/models/filter';
import { getItemDetailLink } from '../../../shared/utils/MediaServerUrlUtil';
import { RootState } from '../../../store/RootReducer';
import { useServerType } from '../../../shared/hooks';
import calculateFileSize from '../../../shared/utils/CalculateFileSize';
import { Show } from '../../../shared/models/common';

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

const ShowList = (props: Props) => {
  const show = useState<Show>({} as Show);
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
        loadUrl: '/api/show/list',
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

  const getTitleValue = (data) => {
    return data.name;
  };

  const getSeasonsValue = (data) => {
    return data.seasons;
  };

  const getEpisodesValue = (data) => {
    return data.episodes;
  };

  const getMissingEpisodes = (data) => {
    return data.missingEpisodes;
  }

  const getSizeInMbValue = (data) => {
    return calculateFileSize(data.sizeInMb)
  }

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
        filterDefinitions={showFilters}
        clearFilters={() => setActiveFilters([])}
      />
      <Grid item container direction="row" className="p-b-16">
        <Grid item>
          <Typography variant="h5" className={classes.title}>
            {t('COMMON.SHOWS')}
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
          <Column caption={t('COMMON.RATING')} dataField="communityRating" />
          <Column
            caption={t('SHOWS.SEASONS')}
            dataField="seasons"
            calculateCellValue={getSeasonsValue}
            allowSorting={true}
          />
          <Column
            caption={t('COMMON.EPISODES')}
            dataField="episodes"
            calculateCellValue={getEpisodesValue}
            allowSorting={true}
          />
          <Column
            caption={t('SHOWS.MISSINGEPISODES')}
            dataField="missingepisodes"
            calculateCellValue={getMissingEpisodes}
            allowSorting={true}
          />
          <Column
            caption={t('COMMON.SIZEINMB')}
            dataField="sizeInMb"
            calculateCellValue={getSizeInMbValue}
            allowSorting={true}
          />
          <MasterDetail enabled={true} component={DetailShowTemplate} />
        </DataGrid>
      </Grid>
    </Grid>
  );
};

export default ShowList;
