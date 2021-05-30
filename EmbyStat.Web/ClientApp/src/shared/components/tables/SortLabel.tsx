import React from 'react'
import { makeStyles } from '@material-ui/core/styles';
import TableSortLabel from '@material-ui/core/TableSortLabel';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles(() => ({
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

interface Props {
  field: string,
  orderedBy: string,
  order: 'asc' | 'desc' | undefined,
  sortHandler: (sortField: string) => void,
  label: string,
}

const SortLabel = (props: Props) => {
  const { field, orderedBy, order, sortHandler, label } = props;
  const classes = useStyles();
  const { t } = useTranslation();

  return (
    <TableSortLabel
      active={orderedBy === field}
      direction={orderedBy === field ? order : 'asc'}
      onClick={() => sortHandler(field)}
    >
      {t(label)}
      {orderedBy === field ? (
        <span className={classes.visuallyHidden}>
          {order === 'desc' ? 'sorted descending' : 'sorted ascending'}
        </span>
      ) : null}
    </TableSortLabel>
  )
}

export default SortLabel;