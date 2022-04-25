import React from 'react';
import {useTranslation} from 'react-i18next';

import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import {Collapse, IconButton, TableCell, TableRow} from '@mui/material';

import {MovieDetailRow} from '..';
import {EsHyperLinkButton} from '../../../../shared/components/buttons';
import {EsChipList} from '../../../../shared/components/esChipList';
import {EsFlagList} from '../../../../shared/components/esFlagList';
import {useMediaServerUrls, useServerType} from '../../../../shared/hooks';
import {MovieRow} from '../../../../shared/models/movie';
import {calculateFileSize} from '../../../../shared/utils';
import {SmallStreamList} from '.';

type RowProps = {
  row: MovieRow;
}

export function Row(props: RowProps) {
  const {row} = props;
  const [open, setOpen] = React.useState(false);
  const {t} = useTranslation();
  const {serverType} = useServerType();
  const {getItemDetailLink} = useMediaServerUrls();

  return (
    <>
      <TableRow key={row.id} hover={true}>
        <TableCell>
          <IconButton size="small" onClick={() => setOpen(!open)}>
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell component="th" scope="row">{row.name}</TableCell>
        <TableCell align="left"><EsChipList list={row.genres} /></TableCell>
        <TableCell align="right">{row.container}</TableCell>
        <TableCell align="right">{row.runTime} {t('COMMON.MIN')}</TableCell>
        <TableCell align="right">
          <SmallStreamList streams={row.videoStreams} />
        </TableCell>
        <TableCell align="right">{calculateFileSize(row.sizeInMb)}</TableCell>
        <TableCell align="right">
          <EsFlagList list={row.subtitles} maxItems={5} />
        </TableCell>
        <TableCell align="right">
          <EsFlagList list={row.audioLanguages} maxItems={3} />
        </TableCell>
        <TableCell align="right">
          <EsHyperLinkButton label={serverType} href={getItemDetailLink(row.id)} />
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{paddingBottom: 0, paddingTop: 0}} colSpan={10}>
          <Collapse in={open} timeout="auto" unmountOnExit={true}>
            <MovieDetailRow id={row.id} />
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}
