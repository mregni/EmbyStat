import React from 'react';
import {useTranslation} from 'react-i18next';

import {TablePagination} from '@mui/material';

type EsTablePaginationProps = {
  totalCount: number;
  rowsPerPage: number;
  pageNumber: number;
  handleChangePage: (newPage: number) => void;
  handleChangeRowsPerPage: (countPerPage: number) => void;
  pageSizeSteps?: number[];
}

export function EsTablePagination(props: EsTablePaginationProps) {
  const {totalCount,
    rowsPerPage,
    pageNumber,
    handleChangePage,
    handleChangeRowsPerPage,
    pageSizeSteps = [25, 50, 100, 200]} = props;
  const {t} = useTranslation();

  return (
    <TablePagination
      sx={{width: '100%'}}
      component="div"
      rowsPerPageOptions={pageSizeSteps}
      count={totalCount}
      rowsPerPage={rowsPerPage}
      page={pageNumber}
      backIconButtonProps={{
        title: t('PAGINATION.PREVIOUSPAGE'),
      }}
      labelRowsPerPage={t('PAGINATION.ROWSPERPAGE')}
      nextIconButtonProps={{
        title: t('PAGINATION.NEXTPAGE'),
      }}
      onPageChange={(event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => handleChangePage(newPage)}
      onRowsPerPageChange={(event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
        handleChangeRowsPerPage(parseInt(event.target.value, 10))}
    />
  );
}
