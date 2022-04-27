import {format} from 'date-fns';
import {saveAs} from 'file-saver';
import React from 'react';
import {useTranslation} from 'react-i18next';
import {usePapaParse} from 'react-papaparse';

import FileDownloadIcon from '@mui/icons-material/FileDownload';
import {Stack, Table, TableBody, TableCell, TableRow, Typography} from '@mui/material';

import {EsScrollbar} from '../../../shared/components/esScrollbar';
import {EsTitle} from '../../../shared/components/esTitle';
import {EsTableHeader, Header} from '../../../shared/components/table';
import {useLocale} from '../../../shared/hooks';
import {Show} from '../../../shared/models/common';

type Props = {
  show: Show
;}

export function MissingEpisodeList(props: Props) {
  const {show} = props;
  const {jsonToCSV} = usePapaParse();
  const {t} = useTranslation();
  const {locale} = useLocale();

  const generateCsv = (): void => {
    const result = show
      .missingSeasons
      .map((season) => (
        season.episodes.map((episode) => ({
          Season: season.indexNumber,
          Episode: episode.indexNumber,
          Title: episode.name,
          PremiereDate: episode.premiereDate,
        }))
      ))
      .reduce((accumulator, value) => accumulator.concat(value), []);

    const csvString = jsonToCSV(result, {delimiter: ';', skipEmptyLines: true});
    const blob = new Blob([csvString], {type: 'text/plain;charset=utf-8'} );
    saveAs(blob, `${show.sortName}.csv`);
  };

  const headers: Header[] = [
    {label: 'COMMON.SEASON'},
    {label: 'COMMON.EPISODE'},
    {label: 'COMMON.TITLE'},
    {label: 'COMMON.PREMIEREDATE', align: 'right'},
  ];

  return (
    <Stack>
      <EsTitle
        content={t('SHOWS.MISSINGEPISODES')}
        variant="h6"
        isFirst
        icon={<FileDownloadIcon/>}
        tooltip={t('COMMON.DOWNLOADCSV')}
        onClick={generateCsv}
      />
      {
        show.missingSeasons.length === 0 && (
          <Typography variant="subtitle2">
            {t('SHOWS.NOMISSINGEPS')}
          </Typography>
        )
      }
      {
        show.missingSeasons.length !== 0 && (
          <EsScrollbar>
            <Table>
              <EsTableHeader headers={headers} />
              <TableBody>
                {
                  show.missingSeasons.map((season) => (
                    season.episodes.map((episode) => (
                      <TableRow key={episode.id} hover>
                        <TableCell>{season.indexNumber}</TableCell>
                        <TableCell>{episode.indexNumber}</TableCell>
                        <TableCell>{episode.name}</TableCell>
                        <TableCell align="right">
                          {format(new Date(episode.premiereDate), 'P', {locale})}
                        </TableCell>
                      </TableRow>
                    ))
                  ))
                }
              </TableBody>
            </Table>
          </EsScrollbar>
        )
      }
    </Stack>
  );
}
