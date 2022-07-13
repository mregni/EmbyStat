import React, {useEffect, useState} from 'react';

import {Avatar} from '@mui/material';

import {useMediaServerUrls} from '../../hooks';

type Props = {
  name: string
  id: string,
  borderRight?: boolean
}

export function EsAvatar(props: Props) {
  const {name, id, borderRight = true} = props;
  const {getPrimaryUserImageLink} = useMediaServerUrls();
  const [backgroundColor, setBackgroundColor] = useState<string>('#000000');

  useEffect(() => {
    setBackgroundColor('#' + id.substring(9, 6));
  }, [id]);

  return (
    <Avatar
      alt={name.charAt(0).toUpperCase()}
      sx={{
        'borderRadius': 1,
        'borderTopRightRadius': borderRight ? 4 : 0,
        'borderBottomRightRadius': borderRight ? 4 : 0,
        backgroundColor,
        'color': (theme) => theme.palette.getContrastText(backgroundColor),
      }}
      src={getPrimaryUserImageLink(id)}
    />
  );
}
