import React from 'react';

import {Card, CardContent} from '@mui/material';

import {MediaServerUser} from '../../../shared/models/mediaServer';

type Props = {
  user: MediaServerUser;
}

export const UserOverviewCard = (props: Props) => {
  const {user} = props;
  return (
    <Card>
      <CardContent>
        {user.id} - {user.totalPlayCount}
      </CardContent>
    </Card>
  );
};
