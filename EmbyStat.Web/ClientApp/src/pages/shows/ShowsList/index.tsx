import React, { useState, useEffect } from 'react';
import DataGrid, {
  Scrolling,
  Paging,
  Column,
  Sorting,
  MasterDetail,
} from 'devextreme-react/data-grid';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
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

import Flag from '../../../shared/components/flag';
import FilterDrawer from '../../../shared/components/filterDrawer';
import { ActiveFilter } from '../../../shared/models/filter';
import { getItemDetailLink } from '../../../shared/utils/MediaServerUrlUtil';
import { RootState } from '../../../store/RootReducer';
import { useServerType } from '../../../shared/hooks';
import calculateFileSize from '../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../shared/utils/CalculateRunTime';

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

export const ShowsList = (props: Props) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [dataSource, setDataSource] = useState<CustomStore>();
  const settings = useSelector((state: RootState) => state.settings);
  const serverType = useServerType();

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

  const getTitleValue = (data) => {
    return data.name;
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

  const getGenresValues = (data) => {
    return data.genres.join(', ');
  };

  const calculateRunTimeValue = (data) => {
    return calculateRunTime(data.runTime);
  };

  const getCumulativeRunTimeTicks = (data) => {
    return calculateRunTime(data.cumulativeRunTimeTicks)
  }

  const RenderEpisodesCount = (data) => {
    return `${data.collectedEpisodeCount} + ${data.missingEpisodesCount} + (${data.specialEpisodeCount})`
  }

  return (
    <Grid container direction="column">
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
          <Column
            dataField="ended"
            caption={t('COMMON.STATUS')}
            dataType="string"
          />
          <Column
            caption={t('COMMON.RUNTIME')}
            calculateCellValue={getCumulativeRunTimeTicks}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.RUNTIME')}
            calculateCellValue={calculateRunTimeValue}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.EPISODES')}
            calculateCellValue={RenderEpisodesCount}
            allowSorting={false}
          />
          <Column
            caption={t('COMMON.OFFICIALRATING')}
            dataField="officialRating"
          />
          <Column
            caption={t('COMMON.LINKS')}
            cellRender={renderLinks}
            allowSorting={false}
            alignment="right"
          />
        </DataGrid>
      </Grid>
    </Grid>
  )
}

export default ShowsList;