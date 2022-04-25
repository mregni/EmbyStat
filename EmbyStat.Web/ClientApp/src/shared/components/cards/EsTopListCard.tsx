import {format} from 'date-fns';
import React, {useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Grid, Paper, Stack, Typography} from '@mui/material';

import NoPoster from '../../assets/images/no-poster.png';
import {useMediaServerUrls} from '../../hooks';
import {TopCard, TopCardItem} from '../../models/common';
import {calculateFileSize} from '../../utils';

interface Props {
  data: TopCard;
  enableBackground: boolean;
}

export function EsTopListCard(props: Props) {
  const {data, enableBackground} = props;
  const {t} = useTranslation();
  const [hoveredItem, setHoveredItem] = useState<TopCardItem>(data.values[0]);
  const {getPrimaryImageLink, getItemDetailLink, getBackdropImageLink} = useMediaServerUrls();

  const calculateTime = (date: string): string => {
    return format(new Date(date), 'P');
  };

  const calculateMinutes = (ticks: string): number => {
    return Math.round(parseInt(ticks) / 600000000);
  };

  const addDefaultSrc = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
    e.currentTarget.src = NoPoster;
  };

  return (
    <Paper
      elevation={5}
      sx={{
        width: 355,
        ...(
          enableBackground && {
            backgroundImage: `linear-gradient(90deg,
              rgba(0,0,0,0) 0%, 
              rgba(0,0,0,0.8) 85%, 
              rgba(0,0,0,0.95) 100%), 
              url(${getBackdropImageLink(hoveredItem?.mediaId ?? data.values[0]?.mediaId ?? '')})`,
            backgroundPosition: 'top',
            backgroundSize: 'cover',
            backgroundRepeat: 'no-repeat',
          }
        ),
      }}
    >
      <Stack direction="row">
        <Box
          sx={{
            'width': '140px',
            'height': '161px',
            '& img': {
              borderTopLeftRadius: 4,
              borderBottomLeftRadius: 4,
            },
          }}>
          <img
            src={getPrimaryImageLink(hoveredItem.mediaId, hoveredItem.image)}
            alt="poster"
            style={{
              height: '100%',
              width: '100%',
              objectFit: 'cover',
              borderRight: 'solid #333333 1px',
            }}
            onError={addDefaultSrc} />
        </Box>
        <Grid
          container={true}
          direction="column"
          justifyContent="flex-start"
          sx={{m: 1}}
        >
          <Grid
            item={true}
            container={true}
            direction="row"
            justifyContent="space-between"
            sx={{
              textTransform: 'uppercase',
              fontWeight: 700,
              borderBottom: 'solid #333333 1px',
              mb: 1,
            }}
          >
            <Grid item={true}>{t(data.title)}</Grid>
            <Grid item={true}>
              <Typography variant="body1" color="secondary" fontWeight="700">
                {data.unitNeedsTranslation ? t(data.unit) : data.unit}
              </Typography>
            </Grid>
          </Grid>


          {data.values.map((pair) => (
            <Grid
              item={true}
              container={true}
              justifyContent="space-between"
              key={pair.mediaId}
              sx={{
                fontSize: '0.8rem',
                fontStyle: 'italic',
              }}
              onMouseOver={() => setHoveredItem(pair)}
            >
              <Grid
                item={true}
                sx={{
                  'cursor': 'pointer',
                  '&:hover': {
                    color: (theme) => theme.palette.secondary.main,
                  },
                  '& a': {
                    color: 'inherit',
                    textDecoration: 'none',
                  },
                }}>
                <a
                  href={`${getItemDetailLink(pair.mediaId)}`}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {pair.label.length > 30 ? t(pair.label).substr(0, 30) + '...' : t(pair.label)}
                </a>
              </Grid>
              <Grid item={true}>
                <Typography variant="body1" color="secondary" fontWeight="700">
                  {data.valueType === 0 ? pair.value : null}
                  {data.valueType === 1 ? calculateMinutes(pair.value) : null}
                  {data.valueType === 2 ? calculateTime(pair.value) : null}
                  {data.valueType === 3 ? calculateFileSize(parseInt(pair.value)) : null}
                </Typography>
              </Grid>
            </Grid>
          ))}
        </Grid>
      </Stack>
    </Paper>
  );
}
