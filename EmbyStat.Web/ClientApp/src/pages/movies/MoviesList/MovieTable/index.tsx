import React, { useState, useEffect, useCallback, ReactElement } from 'react';
import { Scrollbars } from 'react-custom-scrollbars';
import Grid from '@material-ui/core/Grid';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import TablePagination from '@material-ui/core/TablePagination';
import Paper from '@material-ui/core/Paper';
import Button from '@material-ui/core/Button';
import TablePaginationActions from '@material-ui/core/TablePagination/TablePaginationActions';
import { makeStyles, Theme } from '@material-ui/core/styles';
import uuid from 'react-uuid';
import OpenInNewIcon from '@material-ui/icons/OpenInNew';
import { useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';
import KeyboardArrowUpRoundedIcon from '@material-ui/icons/KeyboardArrowUpRounded';
import KeyboardArrowDownRoundedIcon from '@material-ui/icons/KeyboardArrowDownRounded';

import calculateFileSize from '../../../../shared/utils/CalculateFileSize';
import DetailMovieTemplate from '../DetailMovieTemplate';
import { getItemDetailLink } from '../../../../shared/utils/MediaServerUrlUtil';
import Flag from '../../../../shared/components/flag';
import { RootState } from '../../../../store/RootReducer';
import { useServerType } from '../../../../shared/hooks';
import { MovieRow } from '../../../../shared/models/movie';
import SortLabel from '../../../../shared/components/tables/SortLabel';

import { getMoviePage } from '../../../../shared/services/MovieService';
import { TablePage } from '../../../../shared/models/common';
import { ActiveFilter } from '../../../../shared/models/filter';
import Collapse from '@material-ui/core/Collapse';
import Box from '@material-ui/core/Box';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';

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

const MovieTable = (props: Props) => {
  const { filters } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [rowsPerPage, setRowsPerPage] = useState<number>(100);
  const [page, setPage] = useState(0);
  const [orderedBy, setOrderedBy] = useState('sortName');
  const [tableData, setTableData] = useState<TablePage<MovieRow>>({ data: [], totalCount: 0 });
  const [loading, setLoading] = useState(false);

  type Order = 'asc' | 'desc';
  const [order, setOrder] = useState<Order>('asc');

  const fetchRows = useCallback(
    (page: number, rowsPerPage: number, order: string, orderedBy: string, filters: string) => {
      setLoading(true);
      setTableData({ data: [], totalCount: 0 });
      getMoviePage(page * rowsPerPage, rowsPerPage, orderedBy, order, true, filters)
        .then((data: TablePage<MovieRow>) => {
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
    { label: "COMMON.CONTAINER", field: "Container", sortable: true, align: 'right' },
    { label: "COMMON.RUNTIME", field: "runTimeTicks", sortable: true, align: 'right' },
    { label: "COMMON.OFFICIALRATING", field: "officielRating", sortable: true, align: 'right' },
    { label: "COMMON.HEIGHT", field: "", sortable: false, align: 'right' },
    { label: "COMMON.WIDTH", field: "", sortable: false, align: 'right' },
    { label: "COMMON.BITRATE", field: "", sortable: false, align: 'right' },
    { label: "COMMON.SIZEINMB", field: "", sortable: false, align: 'right' },
    { label: "COMMON.BITDEPTH", field: "", sortable: false, align: 'right' },
    { label: "COMMON.CODEC", field: "", sortable: false, align: 'right' },
    { label: "COMMON.VIDEORANGE", field: "", sortable: false, align: 'right' },
    { label: "COMMON.RATING", field: "communityRating", sortable: true, align: 'right' },
    { label: "COMMON.SUBTITLES", field: "", sortable: false, width: 180, align: 'right' },
    { label: "COMMON.AUDIO", field: "", sortable: false, width: 100, align: 'right' },
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
  row: MovieRow;
}

const useRowStyles = makeStyles((theme: Theme) => ({
  button__padding: {
    paddingTop: 5,
  },
}));

const Row = (props: RowProps): ReactElement => {
  const { row } = props;
  const [open, setOpen] = React.useState(false);
  const { t } = useTranslation();
  const settings = useSelector((state: RootState) => state.settings);
  const serverType = useServerType();
  const classes = useRowStyles();

  const getSubtitleValues = (subtitles: string[]) => {
    if (subtitles && subtitles.length) {
      return (
        <Grid container justify="flex-end" direction="row">
          {subtitles?.slice(0, 5).map((x) => (
            <Grid item key={uuid()} className="m-r-4">
              <Flag language={x} />
            </Grid>
          ))}
          {subtitles?.length > 5 ? (
            <Grid item>+ {subtitles.length - 5}</Grid>
          ) : null}
        </Grid>
      );
    }
    return (<div />);
  };

  const getAudioValues = (audioLanguages: string[]) => {
    if (audioLanguages && audioLanguages.length) {
      return (
        <Grid container justify="flex-end">
          {audioLanguages.slice(0, 5).map((x) => (
            <Grid item key={uuid()} className="m-r-4">
              <Flag language={x} />
            </Grid>
          ))}
          {audioLanguages.length > 5 ? (
            <Grid item>+ {audioLanguages.length - 5}</Grid>
          ) : null}
        </Grid>
      );
    }
    return (<div />);
  };

  const renderLinks = (movieId: string) => {
    return (
      <Grid container direction="row" justify="flex-end" alignItems="center">
        <Button
          variant="outlined"
          color="secondary"
          size="small"
          href={getItemDetailLink(settings, movieId)}
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
        <TableCell align="right">{row.container}</TableCell>
        <TableCell align="right">{row.runTime} {t('COMMON.MIN')}</TableCell>
        <TableCell align="right">{row.officialRating}</TableCell>
        <TableCell align="right">{row.height}</TableCell>
        <TableCell align="right">{row.width}</TableCell>
        <TableCell align="right">{row.bitRate}</TableCell>
        <TableCell align="right">{calculateFileSize(row.sizeInMb)}</TableCell>
        <TableCell align="right">{row.bitDepth}</TableCell>
        <TableCell align="right">{row.codec}</TableCell>
        <TableCell align="right">{row.videoRange}</TableCell>
        <TableCell align="right">{row.communityRating}</TableCell>
        <TableCell align="right">{getSubtitleValues(row.subtitles)}</TableCell>
        <TableCell align="right">{getAudioValues(row.audioLanguages)}</TableCell>
        <TableCell align="right">{renderLinks(row.id)}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={17}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <DetailMovieTemplate data={row} />
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}

export default MovieTable
