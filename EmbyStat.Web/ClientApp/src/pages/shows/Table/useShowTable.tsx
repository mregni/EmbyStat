import {useState} from 'react';

import {TablePage} from '../../../shared/models/common';
import {ShowRow} from '../../../shared/models/show';
import {getPage} from '../../../shared/services/showService';

export const useShowTable = () => {
  const [loading, setLoading] = useState(false);
  const [pageData, setPageData] = useState<TablePage<ShowRow>>({data: [], totalCount: 0});

  const fetchRows = (page: number, rowsPerPage: number,
    order: 'asc'| 'desc', orderedBy: string, filtersJson: string)=> {
    setLoading(true);
    setPageData({data: [], totalCount: 0});
    getPage(page * rowsPerPage, rowsPerPage, orderedBy, order, true, filtersJson)
      .then((data: TablePage<ShowRow>) => {
        setPageData(data);
      })
      .finally(() => setLoading(false))
      .catch(() => setLoading(false));
  };

  return {fetchRows, loading, pageData};
};
