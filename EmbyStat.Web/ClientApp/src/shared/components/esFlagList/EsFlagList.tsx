import React from 'react';
import {Grid, Chip} from '@mui/material';
import {EsFlag} from '../esFlag';

type Props = {
  list: string[];
  maxItems: number;
}

export const EsFlagList = (props: Props) => {
  const {list, maxItems} = props;
  if (list && list.length) {
    const uniqueList = Array.from(new Set(list));
    const normalCount = uniqueList.filter((x) => x !== 'und' && x !== null).length;
    const extraCount = uniqueList.filter((x) => x === 'und' || x === null).length +
      (normalCount > maxItems ? normalCount - maxItems : 0);

    return (
      <Grid container justifyContent="flex-end" direction="row">
        {uniqueList?.filter((x) => x !== 'und' && x !== null).slice(0, maxItems).map((x) => (
          <Grid item key={x} sx={{mr: '4px', pt: '6px'}}>
            <EsFlag language={x} />
          </Grid>
        ))}
        {extraCount > 0 ? (
          <Grid item>
            <Chip size='small' label={`+${extraCount}`} sx={{mr: '4px', mt: '4px'}}></Chip>
          </Grid>
        ) : null}
      </Grid>
    );
  }
  return (<div />);
};

export default EsFlagList;
