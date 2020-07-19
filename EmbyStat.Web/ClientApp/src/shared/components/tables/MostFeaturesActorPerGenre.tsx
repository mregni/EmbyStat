import React, { useState } from 'react';
import TableContainer from '@material-ui/core/TableContainer';
import Zoom from '@material-ui/core/Zoom';
import Typography from '@material-ui/core/Typography';
import CardContent from '@material-ui/core/CardContent';
import Card from '@material-ui/core/Card';
import Table from '@material-ui/core/Table';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import TableCell from '@material-ui/core/TableCell';
import TableBody from '@material-ui/core/TableBody';
import TableSortLabel from '@material-ui/core/TableSortLabel';
import { makeStyles } from '@material-ui/core/styles';
import uuid from 'react-uuid';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

import { PersonPoster } from '../../models/person';

interface Props {
  people: PersonPoster[];
}

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
  },
  'row--hover': {
    '&&:hover': {
      backgroundColor: theme.palette.secondary.dark,
    },
  },
  visuallyHidden: {
    border: 0,
    clip: 'rect(0 0 0 0)',
    height: 1,
    margin: -1,
    overflow: 'hidden',
    padding: 0,
    position: 'absolute',
    top: 20,
    width: 1,
  },
}));

function descendingComparator(a, b, orderBy) {
  if (b[orderBy] < a[orderBy]) {
    return -1;
  }
  if (b[orderBy] > a[orderBy]) {
    return 1;
  }
  return 0;
}

function descendingDateComparator(a, b) {
  const aUnix = moment(a.birthDate).format();
  const bUnix = moment(b.birthDate).format();
  if (bUnix < aUnix) {
    return -1;
  }
  if (bUnix > aUnix) {
    return 1;
  }
  return 0;
}

function getComparator(order, orderBy) {
  if (orderBy === 'birthDate') {
    return order === 'desc'
      ? (a, b) => descendingDateComparator(a, b)
      : (a, b) => -descendingDateComparator(a, b);
  }

  return order === 'desc'
    ? (a, b) => descendingComparator(a, b, orderBy)
    : (a, b) => -descendingComparator(a, b, orderBy);
}

function stableSort(array, comparator) {
  const stabilizedThis = array.map((el, index) => [el, index]);
  stabilizedThis.sort((a, b) => {
    const order = comparator(a[0], b[0]);
    if (order !== 0) return order;
    return a[1] - b[1];
  });
  return stabilizedThis.map((el) => el[0]);
}

const MostFeaturesActorPerGenre = (props: Props) => {
  const { people } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const [order, setOrder] = useState<'desc' | 'asc' | undefined>('asc');
  const [orderBy, setOrderBy] = useState('genre');

  const handleRequestSort = (property) => {
    const isAsc = orderBy === property && order === 'asc';
    setOrder(isAsc ? 'desc' : 'asc');
    setOrderBy(property);
  };

  const headers = [
    { id: 'title', alignLeft: true, label: 'Genre' },
    { id: 'name', alignLeft: false, label: 'Name' },
    { id: 'birthDate', alignLeft: false, label: 'Birthday' },
    { id: 'movieCount', alignLeft: false, label: 'Movies' },
    { id: 'showCount', alignLeft: false, label: 'Shows' },
  ];

  return (
    <Zoom in={true}>
      <Card elevation={5} classes={{ root: classes.root }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="h2">
            {t('COMMON.ACTORSPERGENRE')}
          </Typography>
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  {headers.map((headCell) => (
                    <TableCell
                      key={headCell.id}
                      align={headCell.alignLeft ? 'left' : 'right'}
                      padding="default"
                      sortDirection={orderBy === headCell.id ? order : false}
                    >
                      <TableSortLabel
                        active={orderBy === headCell.id}
                        direction={orderBy === headCell.id ? order : 'asc'}
                        onClick={() => handleRequestSort(headCell.id)}
                      >
                        {headCell.label}
                        {orderBy === headCell.id ? (
                          <span className={classes.visuallyHidden}>
                            {order === 'desc'
                              ? 'sorted descending'
                              : 'sorted ascending'}
                          </span>
                        ) : null}
                      </TableSortLabel>
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                {stableSort(people, getComparator(order, orderBy)).map(
                  (row) => (
                    <TableRow
                      key={uuid()}
                      hover
                      classes={{
                        hover: classes['row--hover'],
                      }}
                    >
                      <TableCell component="th" scope="row">
                        {row.title}
                      </TableCell>
                      <TableCell align="right">{row.name}</TableCell>
                      <TableCell align="right">
                        {row.birthDate != null
                          ? moment(row.birthDate).format('ll')
                          : null}
                      </TableCell>
                      <TableCell align="right">{row.movieCount}</TableCell>
                      <TableCell align="right">{row.showCount}</TableCell>
                    </TableRow>
                  )
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </CardContent>
      </Card>
    </Zoom>
  );
};

export default MostFeaturesActorPerGenre;
