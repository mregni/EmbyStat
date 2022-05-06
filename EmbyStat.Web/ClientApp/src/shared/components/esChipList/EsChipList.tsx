import {Chip} from '@mui/material';
import React from 'react';

type Props = {
  list: string[];
  maxItems?: number;
}

export function EsChipList(props: Props) {
  const {list, maxItems = 2} = props;
  return (
    <>
      {
        list
          .slice(0, maxItems)
          .map((item) => <Chip size='small' key={item} label={item} sx={{mr: '4px'}} />)
      }
      {
        list.length > maxItems ?
          <Chip size='small' label={`+${list.length - maxItems}`} sx={{mr: '4px'}} /> :
          null
      }
    </>
  );
}
