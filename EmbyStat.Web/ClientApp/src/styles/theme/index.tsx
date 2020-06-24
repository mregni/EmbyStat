import { createMuiTheme, ThemeOptions } from "@material-ui/core/styles";

const primaryColor = "#2557af";
const secondaryColor = "#FFD421";

const overrides: ThemeOptions = {
  palette: {
    primary: { main: primaryColor },
    secondary: { main: secondaryColor },
    type: 'dark',
    contrastThreshold: 3,
    tonalOffset: 0.2,
  },
  overrides: {
    MuiSelect: {
      select: {
        '&:focus': {
          backgroundColor: 'transparant'
        }
      }
    },
    MuiButtonBase: {
      root: {
        '& .MuiSvgIcon-root': {
          marginTop: '-1px'
        }
      }
    },
    MuiTextField: {
      root: {
        width: '100%'
      }
    }
  }
};

export default createMuiTheme(overrides);
