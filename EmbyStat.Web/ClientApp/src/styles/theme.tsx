import {createTheme, ThemeOptions} from '@mui/material/styles';

const themeOptions: ThemeOptions = {
  palette: {
    mode: 'dark',
    primary: {
      main: '#85cb2d',
    },
    secondary: {
      main: '#cb572d',
    },
    background: {
      default: '#333333',
      paper: '#424242',
    },
  },
  typography: {
    fontFamily: 'Source Sans Pro',
  },
  components: {
    MuiList: {
      defaultProps: {
        dense: true,
      },
    },
    MuiMenuItem: {
      defaultProps: {
        dense: true,
      },
    },
    MuiTextField: {
      defaultProps: {
        size: 'small',
      },
    },
    MuiOutlinedInput: {
      defaultProps: {
        size: 'small',
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          padding: '3px 6px',
        },
      },
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          '&.Mui-selected': {
            borderLeftStyle: 'solid',
            borderLeftWidth: '4px',
            borderLeftColor: 'pink',
            color: 'pink',
          },
        },
      },
    },
  },
};

// eslint-disable-next-line import/no-mutable-exports
let theme = createTheme(themeOptions);
theme = createTheme(theme, {
  components: {
    MuiListItemButton: {
      styleOverrides: {
        root: {
          '&.Mui-selected': {
            borderLeftStyle: 'solid',
            borderLeftWidth: '8px',
            borderLeftColor: theme.palette.primary.main,
            color: theme.palette.primary.main,
          },
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          '&.MuiTableCell-stickyHeader': {
            backgroundColor: 'inherit',
          },
        },
      },
    },
  },
});

export {theme};
