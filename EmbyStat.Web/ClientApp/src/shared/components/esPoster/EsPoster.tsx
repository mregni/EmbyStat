import {Paper, Box} from '@mui/material';
import React from 'react';

import {useMediaServerUrls} from '../../hooks';

type Props = {
  mediaId: string;
  tag: string;
}

export function EsPoster(props: Props) {
  const {mediaId, tag} = props;
  const {getPrimaryImageLink} = useMediaServerUrls();

  return (
    <Paper
      elevation={5}
      sx={{
        width: 200,
        height: 300,
      }}
    >
      <Box
        component="img"
        src={getPrimaryImageLink(mediaId, tag)}
        width={200}
        height={300}
        sx={{
          backgroundSize: 'cover',
        }}
      />
    </Paper>
  );
}
