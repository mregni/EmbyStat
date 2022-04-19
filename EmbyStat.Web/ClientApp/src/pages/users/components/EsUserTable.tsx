import {format} from 'date-fns';
import {t} from 'i18next';
import React, {useEffect, useState} from 'react';

import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import {
  Avatar, Box, Paper, Table, TableBody, TableCell, TableContainer, TableRow,
} from '@mui/material';

import {EsHyperLinkButton} from '../../../shared/components/buttons';
import {EsLoading} from '../../../shared/components/esLoading';
import {EsTableHeader, EsTablePagination, Header} from '../../../shared/components/table';
import {useLocale, useMediaServerUrls, useServerType} from '../../../shared/hooks';
import {MediaServerUserRow} from '../../../shared/models/mediaServer';
import {useUserTable} from '../hooks';

type EsUserRowProps = {
  user: MediaServerUserRow;
}

const EsUserRow = (props: EsUserRowProps) => {
  const {user} = props;
  const {getPrimaryUserImageLink} = useMediaServerUrls();
  const {locale} = useLocale();
  const {serverType} = useServerType();
  const {getUserDetailLink} = useMediaServerUrls();
  const [backgroundColor, setBackgroundColor] = useState<string>('#000000');

  useEffect(() => {
    setBackgroundColor('#' + user.id.substring(9, 6));
  }, [user.id]);

  return (
    <TableRow>
      <TableCell sx={{width: 50}}>
        <Avatar
          alt={user.name.charAt(0).toUpperCase()}
          sx={{
            'borderRadius': 1,
            'backgroundColor': backgroundColor,
            'color': (theme) => theme.palette.getContrastText(backgroundColor),
          }}
          src={getPrimaryUserImageLink(user.id)}
        />
      </TableCell>
      <TableCell>{user.name}</TableCell>
      <TableCell>{format(new Date(user.lastActivityDate ?? ''), 'p P', {locale})}</TableCell>
      <TableCell align="right">{user.movieViewCount}</TableCell>
      <TableCell align="right">{user.episodeViewCount}</TableCell>
      <TableCell align="right">{user.totalViewCount}</TableCell>
      <TableCell sx={{width: 100}} align="right">
        {user.isAdministrator && (<CheckCircleOutlineIcon color="success" />)}
        {!user.isAdministrator && (<HighlightOffIcon color="error" />)}
      </TableCell>
      <TableCell sx={{width: 100}} align="right">
        {user.isHidden && (<CheckCircleOutlineIcon color="success" />)}
        {!user.isHidden && (<HighlightOffIcon color="error" />)}
      </TableCell>
      <TableCell sx={{width: 100}} align="right">
        {user.isDisabled && (<CheckCircleOutlineIcon color="success" />)}
        {!user.isDisabled && (<HighlightOffIcon color="error" />)}
      </TableCell>
      <TableCell align="right">
        <EsHyperLinkButton label={serverType} href={getUserDetailLink(user.id)} />
      </TableCell>
    </TableRow>
  );
};

export const EsUserTable = () => {
  const {fetchRows, loading, pageData} = useUserTable();
  const [orderedBy, setOrderedBy] = useState('Name');
  const [rowsPerPage, setRowsPerPage] = useState<number>(100);
  const [pageNumber, setPageNumber] = useState(0);
  const [order, setOrder] = useState<'asc' | 'desc'>('asc');

  useEffect(() => {
    fetchRows(pageNumber, rowsPerPage, order, orderedBy);
  }, [pageNumber, rowsPerPage, order, orderedBy]);

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
    {label: '', field: '', sortable: false, align: 'left'},
    {label: 'COMMON.NAME', field: 'UPPER(Name)', sortable: true, align: 'left'},
    {label: 'USERS.LASTACTIVE', field: 'LastActivityDate', sortable: true, align: 'left'},
    {label: 'USERS.WATCHED.MOVIES', field: 'MovieViewCount', sortable: true, align: 'right'},
    {label: 'USERS.WATCHED.EPISODES', field: 'EpisodeViewCount', sortable: true, align: 'right'},
    {label: 'USERS.WATCHED.TOTAL', field: 'TotalViewCount', sortable: true, align: 'right'},
    {label: 'USERS.ADMIN', field: '', sortable: false, align: 'right'},
    {label: 'USERS.HIDDEN', field: '', sortable: false, align: 'right'},
    {label: 'USERS.DISABLED', field: '', sortable: false, align: 'right'},
    {label: 'COMMON.LINKS', field: '', sortable: false, align: 'right'},
  ];

  return (
    <Box>
      <EsLoading loading={loading} label={t('USERS.LOADINGLIST')}>
        <Paper sx={{p: 1}}>
          <EsTablePagination
            totalCount={pageData.totalCount}
            rowsPerPage={rowsPerPage}
            pageNumber={pageNumber}
            handleChangePage={handleChangePage}
            handleChangeRowsPerPage={handleChangeRowsPerPage}
            pageSizeSteps={[10, 20, 50, 100]}
          />
          <TableContainer>
            <Table stickyHeader size="small">
              <EsTableHeader orderedBy={orderedBy} order={order} sortHandler={sortHandler} headers={headers} />
              <TableBody>
                { pageData.data.map((user) => <EsUserRow key={user.id} user={user} /> )}
              </TableBody>
            </Table>
          </TableContainer>
          <EsTablePagination
            totalCount={pageData.totalCount}
            rowsPerPage={rowsPerPage}
            pageNumber={pageNumber}
            handleChangePage={handleChangePage}
            handleChangeRowsPerPage={handleChangeRowsPerPage}
            pageSizeSteps={[10, 20, 50, 100]}
          />
        </Paper>
      </EsLoading>
    </Box>
  );
};
