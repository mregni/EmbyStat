/* eslint-disable max-len */
import React, {ReactNode} from 'react';

import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import PlayCircleOutlineIcon from '@mui/icons-material/PlayCircleOutline';
import QueryBuilderIcon from '@mui/icons-material/QueryBuilder';
import StorageIcon from '@mui/icons-material/Storage';
import TheatersIcon from '@mui/icons-material/Theaters';
import SvgIcon from '@mui/material/SvgIcon';

export const convertToIcon = (value: string): ReactNode => {
  switch (value) {
  case 'TheatersRoundedIcon':
    return <TheatersIcon />;
  case 'StorageRoundedIcon':
    return <StorageIcon />;
  case 'QueryBuilderRoundedIcon':
    return <QueryBuilderIcon />;
  case 'PlayCircleOutline':
    return <PlayCircleOutlineIcon />;
  case 'PoundRoundedIcon':
    return (
      <SvgIcon>
        <path d="M3,5A2,2 0 0,1 5,3H19A2,2 0 0,1 21,5V19A2,2 0 0,1 19,21H5C3.89,21 3,20.1 3,19V5M7,18H9L9.35,16H13.35L13,18H15L15.35,16H17.35L17.71,14H15.71L16.41,10H18.41L18.76,8H16.76L17.12,6H15.12L14.76,8H10.76L11.12,6H9.12L8.76,8H6.76L6.41,10H8.41L7.71,14H5.71L5.35,16H7.35L7,18M10.41,10H14.41L13.71,14H9.71L10.41,10Z" />
      </SvgIcon>
    );
  case 'PeopleAltRoundedIcon':
    return <PeopleAltIcon />;
  }
};
