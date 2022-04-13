import React, {ReactElement} from 'react';
import Scrollbars from 'react-custom-scrollbars-2';

import {useTheme} from '@mui/material';

type Props = {
  children : ReactElement | ReactElement[];
}

export const EsScrollbar = (props: Props) => {
  const {children} = props;
  const theme = useTheme();

  return (
    <Scrollbars
      style={{height: 180, minWidth: 450}}
      renderThumbVertical={({style, ...props}) =>
        <div {...props}
          style={{
            ...style,
            backgroundColor: theme.palette.primary.main,
            width: '6px',
            marginLeft: '6px',
            opacity: '1'}}
        />
      }>
      {children}
    </Scrollbars>
  );
};
