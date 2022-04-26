import React, {useContext, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Paper, Table, TableContainer} from '@mui/material';

import {EsLoading} from '../../../shared/components/esLoading';
import {EsTableHeader, EsTablePagination, Header} from '../../../shared/components/table';
import {MoviesContext} from '../../../shared/context/movies';
import {Body} from './Helpers';
import {useMovieTable} from './useMovieTable';

export function MovieTable() {
  const {activeFilters} = useContext(MoviesContext);
  const {t} = useTranslation();
  const {fetchRows, loading, pageData} = useMovieTable();
  const [orderedBy, setOrderedBy] = useState('sortName');
  const [rowsPerPage, setRowsPerPage] = useState<number>(100);
  const [pageNumber, setPageNumber] = useState(0);
  const [order, setOrder] = useState<'asc' | 'desc'>('asc');

  useEffect(() => {
    const filtersJson = JSON.stringify(
      activeFilters?.map((x) => ({
        field: x.field,
        operation: x.operation.operation,
        value: x.value,
      })));
    fetchRows(pageNumber, rowsPerPage, order, orderedBy, filtersJson);
  }, [pageNumber, rowsPerPage, order, orderedBy, activeFilters, fetchRows]);

  const sortHandler = (property: string) => {
    const isAsc = orderedBy === property && order === 'asc';
    setOrder(isAsc ? 'desc' : 'asc');
    setOrderedBy(property);
  };

  const handleChangePage = (newPage: number) => {
    setPageNumber(newPage);
  };

  const handleChangeRowsPerPage = (countPerPage: number) => {
    setPageNumber(0);
    setRowsPerPage(countPerPage);
  };

  const headers: Header[] = [
    {label: '', field: '', sortable: false, width: 30, align: 'left'},
    {label: 'COMMON.TITLE', field: 'sortName', sortable: true, width: 300, align: 'left'},
    {label: 'COMMON.GENRES', field: '', sortable: false, align: 'left'},
    {label: 'COMMON.CONTAINER', field: 'Container', width: 60, sortable: true, align: 'right'},
    {label: 'COMMON.RUNTIME', field: 'runTimeTicks', width: 60, sortable: true, align: 'right'},
    {label: 'COMMON.STREAMS', field: '', sortable: false, align: 'right'},
    {label: 'COMMON.SIZEINMB', field: '', sortable: false, align: 'right'},
    {label: 'COMMON.SUBTITLES', field: '', sortable: false, width: 210, align: 'right'},
    {label: 'COMMON.AUDIO', field: '', sortable: false, width: 150, align: 'right'},
    {label: 'COMMON.LINKS', field: '', sortable: false, align: 'right'},
  ];

  return (
    <Box>
      <EsLoading loading={loading} label={t('MOVIES.LOADINGLIST')}>
        <Paper sx={{p: 1}}>
          <EsTablePagination
            totalCount={pageData.totalCount}
            rowsPerPage={rowsPerPage}
            pageNumber={pageNumber}
            handleChangePage={handleChangePage}
            handleChangeRowsPerPage={handleChangeRowsPerPage}
          />
          <TableContainer>
            <Table stickyHeader={true} size="small">
              <EsTableHeader orderedBy={orderedBy} order={order} sortHandler={sortHandler} headers={headers} />
              <Body page={pageData} />
            </Table>
          </TableContainer>
          <EsTablePagination
            totalCount={pageData.totalCount}
            rowsPerPage={rowsPerPage}
            pageNumber={pageNumber}
            handleChangePage={handleChangePage}
            handleChangeRowsPerPage={handleChangeRowsPerPage}
          />
        </Paper>
      </EsLoading>
    </Box>
  );
}
