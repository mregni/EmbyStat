import React, {ReactElement} from 'react';
import {useTranslation} from 'react-i18next';

import {Chip, IconButton, Stack, Tooltip, Typography} from '@mui/material';

type Props = {
  content: string;
  isFirst?: boolean;
  variant?: 'h4' | 'h6';
  onClick?: () => void;
  icon?: ReactElement;
  tooltip?: string;
  chipContent: string;
}

export function EsChipTitle(props: Props) {
  const {content, isFirst = false, variant = 'h4', onClick, icon, tooltip = '', chipContent} = props;
  const {t} = useTranslation();

  return (
    <Stack
      direction="row"
      justifyContent="space-between"
      sx={{borderBottom: 'solid 1px #aaaaaa', width: '100%', pt: isFirst ? 0 : 3}}
    >
      <Typography variant={variant} component="div">
        {t(content)} <Chip sx={{mb: '2px'}} label={chipContent} size="small" />
      </Typography>
      {
        icon && (
          (
            <Tooltip title={tooltip}>
              <IconButton
                color="primary"
                component="span"
                onClick={onClick}
                disableRipple={true}
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
