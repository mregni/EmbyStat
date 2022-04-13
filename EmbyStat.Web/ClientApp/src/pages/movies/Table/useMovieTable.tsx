import {useState} from 'react';

import {TablePage} from '../../../shared/models/common';
import {MovieRow} from '../../../shared/models/movie';
import {getPage} from '../../../shared/services/movieService';

export const useMovieTable = () => {
  const [loading, setLoading] = useState(false);
  const [pageData, setPageData] = useState<TablePage<MovieRow>>({data: [], totalCount: 0});

  const fetchRows = (page: number, rowsPerPage: number,
    order: 'asc'| 'desc', orderedBy: string, filtersJson: string)=> {
    setLoading(true);
    setPageData({data: [], totalCount: 0});
    getPage(page * rowsPerPage, rowsPerPage, orderedBy, order, true, filtersJson)
      .then((data: TablePage<MovieRow>) => {
        setPageData(data);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return {fetchRows, loading, pageData};
};
