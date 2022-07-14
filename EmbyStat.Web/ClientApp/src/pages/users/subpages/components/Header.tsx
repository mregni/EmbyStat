import React from 'react';

import {Box, Card, Stack} from '@mui/material';

import {EsAvatar} from '../../../../shared/components/esAvatar';
import {EsTitle} from '../../../../shared/components/esTitle';
import {MediaServerUserDetails} from '../../../../shared/models/mediaServer';

type Props = {
  details: MediaServerUserDetails
}

export function Header(props: Props) {
  const {details} = props;
  return (
    <Card>
      <Stack direction="row" spacing={1}>
        <Box>
          <EsAvatar name={details.name} id={details.id} borderRight={false} />
        </Box>
        <Box>
          <EsTitle content={details.name} isFirst variant="h6" />
        </Box>
      </Stack>
    </Card>
  );
}
