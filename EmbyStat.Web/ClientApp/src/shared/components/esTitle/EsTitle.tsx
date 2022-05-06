import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

import {IconButton, Stack, Tooltip, Typography} from '@mui/material';

type Props = {
  content: string;
  isFirst?: boolean;
  variant?: 'h4' | 'h6';
  onClick?: () => void;
  icon?: ReactElement;
  tooltip?: string;
}

export function EsTitle(props: Props) {
  const {content, isFirst = false, variant = 'h4', onClick, icon, tooltip = ''} = props;
  const {t} = useTranslation();

  return (
    <Stack
      direction="row"
      justifyContent="space-between"
      sx={{borderBottom: 'solid 1px #aaaaaa', width: '100%', pt: isFirst ? 0 : 3}}
    >
      <Typography variant={variant}>
        {t(content)}
      </Typography>
      {
        icon && (
          (
            <Tooltip title={tooltip}>
              <IconButton
                color="primary"
                component="span"
                onClick={onClick}
                disableRipple
              >
                {(icon)}
              </IconButton>
            </Tooltip>
          )
        )
      }
    </Stack>
  );
}
