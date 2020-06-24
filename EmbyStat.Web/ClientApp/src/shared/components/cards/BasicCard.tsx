import React from 'react'
import { Paper, makeStyles, Zoom } from '@material-ui/core';
import { useTranslation } from 'react-i18next';
import { Card } from '../../models/common';
import convertToIcon from '../../utils/StringToIconUtil';

import calculateFileSize from '../../utils/CalculateFileSize';

const useStyles = makeStyles((theme) => ({
  card: {
    display: 'flex',
    flexDirection: 'row',
    height: 60,
    width: 220,
  },
  card__icon: {
    width: 60,
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'row',
    backgroundColor: theme.palette.secondary.dark,
    borderTopLeftRadius: 4,
    borderBottomLeftRadius: 4,
    '& > svg': {
      color: theme.palette.primary.dark,
      height: 30,
      width: 30,
    }
  },
  card__text: {
    display: 'flex',
    justifyContent: 'center',
    flexDirection: 'column',
    marginLeft: 8,
    color: theme.palette.getContrastText(theme.palette.background.paper),
  },
  card__value: {
    fontWeight: 700,
    fontSize: '1.3rem'
  },
  card__title: {
    textTransform: 'uppercase',
    fontWeight: 300,
    fontSize: '0.75rem'
  }
}));

interface Props {
  card: Card,
}

const BasicCard = (props: Props) => {
  const { card } = props;
  const { t } = useTranslation();
  const classes = useStyles();

  const calculateSize = (value: string): string => {
    let newValue = parseInt(value, 10);
    return calculateFileSize(newValue);
  }

  const calculateTime = (value: string): string => {
    const newValues = String(value).split('|');
    let returnValue = '';
    if (newValues[0].length > 0) {
      returnValue += `${newValues[0]}${t('COMMON.DAYSABBR')} `;
    }
    if (newValues[1].length > 0) {
      returnValue += `${newValues[1]}${t('COMMON.HOURSABBR')} `;
    }
    if (newValues[2].length > 0) {
      returnValue += `${newValues[2]}${t('COMMON.MINUTESABBR')}`;
    }

    return returnValue;
  }

  return (
    <Zoom in={true}>
      <Paper elevation={5}>
        <div className={classes.card}>
          <div className={classes.card__icon}>
            {convertToIcon(card.icon)}
          </div>
          <div className={classes.card__text}>
            <div className={classes.card__value}>
              {card.type === 'text' ? card.value : null}
              {card.type === 'size' ? calculateSize(card.value) : null}
              {card.type === 'time' ? calculateTime(card.value) : null}
            </div>
            <div className={classes.card__title}>{t(card.title)}</div>
          </div>
        </div>
      </Paper>
    </Zoom>
  )
}

export default BasicCard
