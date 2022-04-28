import {useState} from 'react';

import {TablePage} from '../../../shared/models/common';
import {MediaServerUserRow} from '../../../shared/models/mediaServer';
import {getUserPage} from '../../../shared/services';

export const useUserTable = () => {
  const [loading, setLoading] = useState(false);
  const [pageData, setPageData] = useState<TablePage<MediaServerUserRow>>({data: [], totalCount: 0});

  const fetchRows = (page: number, rowsPerPage: number,
    order: 'asc'| 'desc', orderedBy: string)=> {
    setLoading(true);
    setPageData({data: [], totalCount: 0});
    getUserPage(page * rowsPerPage, rowsPerPage, orderedBy, order, true)
      .then((data: TablePage<MediaServerUserRow>) => {
        setPageData(data);
      })
      .finally(() => {
        setLoading(false);
      })
      .catch(() => setLoading(false));
  };

  return {fetchRows, loading, pageData};
};
