import Button from '@material-ui/core/Button';
import { createStyles, Theme, withStyles } from '@material-ui/core/styles';

export const FilterDropdownButton = withStyles((theme: Theme) =>
  createStyles({
    root: {
      background: theme.palette.grey[800],
      border: "1px solid rgba(151, 231, 51, 0)",
      color: 'white',
      height: 35,
      margin: '0 15px 15px 0',
      padding: '0 22px',
      '&:hover': {
        color: theme.palette.primary.main,
      }
    },
    label: {
      textTransform: 'capitalize',
    },
  })
)(Button);