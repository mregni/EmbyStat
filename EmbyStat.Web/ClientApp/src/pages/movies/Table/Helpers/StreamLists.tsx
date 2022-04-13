import {Grid, Stack, Chip, Tooltip} from '@mui/material';
import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';
import {generateStreamChipLabel} from '../../../../shared/utils';
import {Stream, VideoStream} from '../../../../shared/models/common';
import {EsFlag} from '../../../../shared/components/esFlag';

type StreamListProps = {
  list: Stream[];
  icon: ReactElement;
  tooltip: string;
}

export const StreamList = (props: StreamListProps) => {
  const {list, icon, tooltip} = props;
  const {t} = useTranslation();

  return (
    <Grid container item alignItems="flex-start" spacing={1}>
      <Grid item>
        <Tooltip title={t(tooltip) ?? ''}>
          {icon}
        </Tooltip>
      </Grid>
      <Grid item>
        <Stack direction="row">
          {list
            .filter((x) => ![null, 'und', 'src'].includes(x.language))
            .map((x) => x.language != null ?
              (
                <EsFlag language={x.language} isDefault={x.isDefault} key={x.id} height={18} />
              ) : null,
            )}
          {list.some((x) => [null, 'und', 'src'].includes(x.language)) ?
            (
              <Chip size="small" label={`+${list.filter((x) => [null, 'und', 'src'].includes(x.language)).length}`} />
            ) :
            null}
        </Stack>
      </Grid>
    </Grid>
  );
};

type SmallStreamListProps = {
  streams: VideoStream[];
}

export const SmallStreamList = (props: SmallStreamListProps) => {
  const {streams} = props;
  if (streams && streams.length) {
    const count = streams.length;
    return (
      <Grid container justifyContent="flex-end">
        {streams.slice(0, 1).map((stream) =>
          <Chip key={stream.id} size='small' label={generateStreamChipLabel(stream)} sx={{mr: '4px'}} />)}
        {count > 1 ? (
          <Grid item>
            <Chip size='small' label={`+${count - 1}`} sx={{mr: '4px'}} />
          </Grid>
        ) : null}
      </Grid>
    );
  }

  return <div />;
};
