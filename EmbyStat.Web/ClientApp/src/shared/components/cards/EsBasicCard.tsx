import {Box, Paper} from '@mui/material';
import React from 'react';
import {useTranslation} from 'react-i18next';
import {Card} from '../../models/common';
import {convertToIcon, calculateFileSize} from '../../utils';

interface Props {
  card: Card;
}

export function EsBasicCard(props: Props) {
  const {card} = props;
  const {t} = useTranslation();

  const calculateSize = (value: string): string => {
    const newValue = parseInt(value, 10);
    return calculateFileSize(newValue);
  };

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
  };

  return (
    <Paper elevation={5}>
      <Box sx={{
        display: 'flex',
        flexDirection: 'row',
        height: 60,
      }}>
        <Box sx={{
          'width': 60,
          'display': 'flex',
          'justifyContent': 'center',
          'alignItems': 'center',
          'flexDirection': 'row',
          'backgroundColor': (theme) => theme.palette.secondary.dark,
          'borderTopLeftRadius': 4,
          'borderBottomLeftRadius': 4,
          'borderRight': 'solid #333333 1px',
          '& > svg': {
            color: (theme) => theme.palette.grey[400],
            height: 30,
            width: 30,
          },
        }}>{convertToIcon(card.icon)}</Box>
        <Box sx={{
          display: 'flex',
          justifyContent: 'center',
          flexDirection: 'column',
          marginLeft: 1,
          color: (theme) => theme.palette.getContrastText(theme.palette.background.paper),
        }}>
          <Box sx={{
            fontWeight: 700,
            fontSize: '1.3rem',
          }}>
            {card.type === 'text' ? card.value : null}
            {card.type === 'size' ? calculateSize(card.value) : null}
            {card.type === 'time' ? calculateTime(card.value) : null}
          </Box>
          <Box sx={{
            textTransform: 'uppercase',
            fontWeight: 300,
            fontSize: '0.75rem',
          }}>{t(card.title)}</Box>
        </Box>
      </Box>
    </Paper>
  );
}
