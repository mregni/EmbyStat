import React, {useContext} from 'react';

import {Box, Grid, Stack} from '@mui/material';

import {EsBasicCard, EsTopListCard} from '../../shared/components/cards';
import {EsBarGraph, EsPieGraph} from '../../shared/components/charts';
import {EsTitle} from '../../shared/components/esTitle';
import {ShowsContext} from '../../shared/context/shows';
import {Card, TopCard} from '../../shared/models/common';

type Props = {}

export function General(props: Props) {
  const {statistics} = useContext(ShowsContext);

  return (
    <Stack direction="column" spacing={2}>
      <EsTitle content="Numbers" isFirst />
      <Box>
        <Grid container spacing={2}>
          {statistics?.cards != null && statistics.cards.length > 0 ?
            statistics.cards.map((card: Card) => (
              <Grid item key={card.title} xs={12} sm={6} md={4} lg={3} xl={2}>
                <EsBasicCard card={card} />
              </Grid>
            )) : null}
        </Grid>
      </Box>
      <Box>
        <Grid container spacing={2}>
          {statistics.topCards != null && statistics.topCards.length > 0 ?
            statistics.topCards.map((card: TopCard) => (
              <Grid item key={card.title}>
                <EsTopListCard data={card} enableBackground />
              </Grid>
            )) : null}
        </Grid>
      </Box>
      <EsTitle content="COMMON.GRAPHS" />
      <Box>
        <Grid container spacing={2}>
          {statistics.barCharts != null && statistics.barCharts.length > 0 ?
            statistics.barCharts.map((chart) => (
              <Grid item xs={12} xl={6} key={chart.title}>
                <EsBarGraph chart={chart} />
              </Grid>
            )) : null}
          {statistics.pieCharts != null && statistics.pieCharts.length > 0 ?
            statistics.pieCharts.map((chart) => (
              <Grid item xs={6} xl={3} key={chart.title}>
                <EsPieGraph chart={chart} height={250} />
              </Grid>
            )) : null}
        </Grid>
      </Box>
    </Stack>
  );
}
