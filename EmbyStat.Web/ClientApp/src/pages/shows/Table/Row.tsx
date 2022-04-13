import React from 'react';
import {useTranslation} from 'react-i18next';

import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import {Collapse, IconButton, TableCell, TableRow} from '@mui/material';

import {EsHyperLinkButton} from '../../../shared/components/buttons';
import {EsChipList} from '../../../shared/components/esChipList';
import {useMediaServerUrls, useServerType} from '../../../shared/hooks';
import {ShowRow} from '../../../shared/models/show';
import {calculateFileSize} from '../../../shared/utils';
import {EpisodeColumn, ShowDetailRow} from './';

type Props = {
  row: ShowRow;
}

export const Row = (props: Props) => {
  const {row} = props;
  const [open, setOpen] = React.useState(false);
  const {t} = useTranslation();
  const {serverType} = useServerType();
  const {getItemDetailLink} = useMediaServerUrls();

  return (
    <>
      <TableRow key={row.id} hover>
        <TableCell>
          <IconButton size="small" onClick={() => setOpen(!open)}>
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell component="th" scope="row">{row.name}</TableCell>
        <TableCell align="left"><EsChipList list={row.genres} /></TableCell>
        <TableCell align="right">{row.status}</TableCell>
        <TableCell align="right">{calculateFileSize(row.sizeInMb)}</TableCell>
        <TableCell align="right">{row.runTime} {t('COMMON.MIN')}</TableCell>
        <TableCell align="right">{row.cumulativeRunTime} {t('COMMON.MIN')}</TableCell>
        <TableCell align="right">{row.seasonCount}</TableCell>
        <TableCell align="right"><EpisodeColumn row={row} /></TableCell>
        <TableCell align="right">{row.officialRating}</TableCell>
        <TableCell align="right">
          <EsHyperLinkButton label={serverType} href={getItemDetailLink(row.id)} />
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{paddingBottom: 0, paddingTop: 0}} colSpan={11}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <ShowDetailRow id={row.id} />
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
};
