import React, { useState, useEffect, ReactElement, useCallback, useContext } from 'react';
import { Scrollbars } from 'react-custom-scrollbars';

import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import { makeStyles, Theme } from '@material-ui/core/styles';
import { useSelector } from 'react-redux';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useTranslation } from 'react-i18next';
import KeyboardArrowUpRoundedIcon from '@material-ui/icons/KeyboardArrowUpRounded';
import KeyboardArrowDownRoundedIcon from '@material-ui/icons/KeyboardArrowDownRounded';
import LinearProgress from '@material-ui/core/LinearProgress';

import { ActiveFilter } from '../../../../shared/models/filter';
import { getItemDetailLink } from '../../../../shared/utils/MediaServerUrlUtil';
import { RootState } from '../../../../store/RootReducer';
import { useServerType } from '../../../../shared/hooks';
import calculateRunTime from '../../../../shared/utils/CalculateRunTime';
import { Paper, TableContainer, Table, TableHead, TableRow, TableCell, TableBody, TablePagination, IconButton, Collapse } from '@material-ui/core';
import TablePaginationActions from '@material-ui/core/TablePagination/TablePaginationActions';
import SortLabel from '../../../../shared/components/tables/SortLabel';
import { DetailShowTemplate } from '../DetailShowTemplate';
import { ShowRow } from '../../../../shared/models/show';
import { TablePage } from '../../../../shared/models/common';
import { getShowPage } from '../../../../shared/services/ShowService';
import { SettingsContext } from '../../../../shared/context/settings';

const useStyles = makeStyles((theme: Theme) => ({
  container: {
    maxHeight: 'calc(100vh - 232px)',
  },
  paper__root: {
    width: '100%',
    height: 'calc(100vh - 155px)',
    padding: 16,
  },
  stickyHeader: {
    backgroundColor: theme.palette.background.paper,
  },
}));

interface Props {
  filters: ActiveFilter[];
}

export const ShowsTable = (props: Props) => {
  const { filters } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [rowsPerPage, setRowsPerPage] = useState<number>(100);
  const [page, setPage] = useState(0);
  const [orderedBy, setOrderedBy] = useState('sortName');
  const [tableData, setTableData] = useState<TablePage<ShowRow>>({ data: [], totalCount: 0 });
  const [loading, setLoading] = useState(false);

  type Order = 'asc' | 'desc';
  const [order, setOrder] = useState<Order>('asc');

  const fetchRows = useCallback(
    (page: number, rowsPerPage: number, order: string, orderedBy: string, filters: string) => {
      setLoading(true);
      setTableData({ data: [], totalCount: 0 });
      getShowPage(page * rowsPerPage, rowsPerPage, orderedBy, order, true, filters)
        .then((data: TablePage<ShowRow>) => {
          setTableData(data);
        })
        .finally(() => {
          setLoading(false);
        });
    }, []);

  useEffect(() => {
    const filtersJson = JSON.stringify(
      filters?.map((x) => ({
        field: x.field,
        operation: x.operation,
        value: x.value,
      })));
    fetchRows(page, rowsPerPage, order, orderedBy, filtersJson);
  }, [page, rowsPerPage, order, orderedBy, fetchRows, filters]);

  const emptyRows = rowsPerPage - Math.min(rowsPerPage, tableData.totalCount - page * rowsPerPage);
  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const createSortHandler = (property: string) => (event: React.MouseEvent<unknown>) => {
    const isAsc = orderedBy === property && order === 'asc';
    setOrder(isAsc ? 'desc' : 'asc');
    setOrderedBy(property);
  };

  type head = {
    label: string,
    field: string,
    sortable: boolean,
    width?: number | undefined,
    align: 'right' | 'left',
  }

  const headers: head[] = [
    { label: "", field: "", sortable: false, width: 30, align: 'left' },
    { label: "COMMON.TITLE", field: "sortName", sortable: true, width: 300, align: 'left' },
    { label: "COMMON.GENRES", field: "", sortable: false, width: 220, align: 'left' },
    { label: "COMMON.STATUS", field: "", sortable: false, width: 220, align: 'right' },
    { label: "COMMON.RUNTIME", field: "runTimeTicks", sortable: true, align: 'right' },
    { label: "COMMON.TOTALRUNTIME", field: "runTimeTicks", sortable: false, align: 'right' },
    { label: "COMMON.EPISODES", field: "", sortable: false, align: 'right' },
    { label: "COMMON.OFFICIALRATING", field: "officielRating", sortable: true, align: 'right' },
    { label: "COMMON.LINKS", field: "", sortable: false, align: 'right' },
  ]

  return (
    <Grid item container>
      <Paper className={classes.paper__root}>
        {
          loading ? <div>Loading</div>
            :
            (
              <>
                <TableContainer className={classes.container}>
                  <Scrollbars
                    autoHide
                    autoHideTimeout={1000}
                    autoHideDuration={200}
                    style={{ width: '100%', height: 'calc(100vh - 232px)' }}
                  >
                    <Table
                      stickyHeader
                      size="small"
                    >
                      <TableHead>
                        <TableRow>
                          {
                            headers.map((column) =>
                              <TableCell
                                width={column.width}
                                key={column.label}
                                classes={{ stickyHeader: classes.stickyHeader }}
                                align={column.align}
                              >
                                {
                                  column.sortable ?
                                    (
                                      <SortLabel
                                        field={column.field}
                                        orderedBy={orderedBy}
                                        order={order}
                                        sortHandler={createSortHandler}
                                        label={column.label}
                                      />
                                    ) : (t(column.label))
                                }
                              </TableCell>
                            )
                          }
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {
                          tableData.data.map((row) => (
                            <Row row={row} key={row.id} />
                          ))}
                        {emptyRows > 0 && (
                          <TableRow style={{ height: 53 * emptyRows }}>
                            <TableCell colSpan={6} />
                          </TableRow>
                        )}
                      </TableBody>
                    </Table>
                  </Scrollbars>
                </TableContainer>

                <TablePagination
                  rowsPerPageOptions={[25, 50, 100, { label: 'All', value: tableData.totalCount }]}
                  component="div"
                  count={tableData.totalCount}
                  rowsPerPage={rowsPerPage}
                  page={page}
                  SelectProps={{
                    native: true,
                  }}
                  onChangePage={handleChangePage}
                  onChangeRowsPerPage={handleChangeRowsPerPage}
                  ActionsComponent={TablePaginationActions}
                  backIconButtonText={t('PAGINATION.PREVIOUSPAGE')}
                  labelRowsPerPage={t('PAGINATION.ROWSPERPAGE')}
                  nextIconButtonText={t('PAGINATION.NEXTPAGE')}
                />
              </>
            )
        }

      </Paper>
    </Grid>
  )
}

interface RowProps {
  row: ShowRow;
}

const useRowStyles = makeStyles(() => ({
  button__padding: {
    paddingTop: 5,
  },
}));

const renderEpisodesCount = (data: ShowRow) => {
  let value = `${data.collectedEpisodeCount} / ${data.collectedEpisodeCount + data.missingEpisodesCount}`;
  if (data.specialEpisodeCount > 0) {
    value += ` (${data.specialEpisodeCount})`
  }

  const percentage = data.collectedEpisodeCount / (data.collectedEpisodeCount + data.missingEpisodesCount) * 100;

  return (
    <Grid container direction="column">
      <Grid item>
        {value}
      </Grid>
      <Grid item>
        <LinearProgress value={percentage} variant="determinate" color={percentage >= 90 ? "primary" : "secondary"} />
      </Grid>
    </Grid>
  )
}

const Row = (props: RowProps): ReactElement => {
  const { row } = props;
  const [open, setOpen] = React.useState(false);
  const { t } = useTranslation();
  const { settings } = useContext(SettingsContext);
  const serverType = useServerType();
  const classes = useRowStyles();

  const renderLinks = (showId: string) => {
    return (
      <Grid container direction="row" justify="flex-end" alignItems="center">
        <Button
          variant="outlined"
          color="secondary"
          size="small"
          href={getItemDetailLink(settings, showId)}
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

  return (
    <>
      <TableRow key={row.id} hover>
        <TableCell>
          <IconButton aria-label="expand row" size="small" onClick={() => setOpen(!open)}>
            {open ? <KeyboardArrowUpRoundedIcon /> : <KeyboardArrowDownRoundedIcon />}
          </IconButton>
        </TableCell>
        <TableCell component="th" scope="row">{row.name}</TableCell>
        <TableCell align="left">{row.genres.join(', ')}</TableCell>
        <TableCell align="right">{row.status}</TableCell>
        <TableCell align="right">{calculateRunTime(row.runTime)}</TableCell>
        <TableCell align="right">{calculateRunTime(row.cumulativeRunTimeTicks)}</TableCell>
        <TableCell align="right">{renderEpisodesCount(row)}</TableCell>
        <TableCell align="right">{row.officialRating}</TableCell>
        <TableCell align="right">{renderLinks(row.id)}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={17}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <DetailShowTemplate data={row} />
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}

export default ShowsTable
