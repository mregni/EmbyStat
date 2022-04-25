import {Button} from '@mui/material';
import React, {ReactElement} from 'react';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';

type Props = {
  label: string;
  href: string;
  startIcon?: ReactElement;
}

export function EsHyperLinkButton(props: Props) {
  const {label, href, startIcon = <OpenInNewIcon />} = props;
  return (
    <Button
      variant="text"
      color="primary"
      size="small"
      href={href}
      target="_blank"
      startIcon={startIcon}
    >
      {label}
    </Button>
  );
}
