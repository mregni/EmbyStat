import React, {useContext} from 'react';

import {Box, Grid} from '@mui/material';

import {EsBasicCard} from '../../../shared/components/cards';
import {ServerContext} from '../../../shared/context/server';
import {Card} from '../../../shared/models/common';

export const EsServerExtraInfo = () => {
  const {serverInfo} = useContext(ServerContext);

  const cards: Card[] = [
    {value: serverInfo.activeUserCount.toString(), title: 'USERS.ACTIVEUSERS', type: 'text', icon: 'PoundRoundedIcon'},
    {value: serverInfo.idleUserCount.toString(), title: 'USERS.IDLEUSERS', type: 'text', icon: 'PoundRoundedIcon'},
    {value: serverInfo.activeDeviceCount.toString(),
      title: 'SERVER.ACTIVEDEVICES',
      type: 'text',
      icon: 'PoundRoundedIcon',
    },
    {value: serverInfo.idleDeviceCount.toString(), title: 'SERVER.IDLEDEVICES', type: 'text', icon: 'PoundRoundedIcon'},
  ];

  return (
    <Box>
      <Grid container spacing={2}>
        {cards.map((card)=> (
          <Grid key={card.title} item xs={12} sm={6} md={4} lg={3} xl={2}>
            <EsBasicCard card={card} />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};
