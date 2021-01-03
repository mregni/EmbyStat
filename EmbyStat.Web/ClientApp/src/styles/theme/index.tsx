import { createMuiTheme, ThemeOptions } from "@material-ui/core/styles";

declare module "@material-ui/core/styles/createBreakpoints" {
  interface BreakpointOverrides {
    xs: true;
    sm: true;
    md: true;
    lg: true;
    xl: true;
    xxl: true;
  }
}

const overrides: ThemeOptions = {
  palette: {
    primary: {
      light: "#ccff6a",
      main: "#97e733",
      dark: "#62b400",
      contrastText: "#000000",
    },
    secondary: {
      light: "#eb7a7e",
      main: "#b54b52",
      dark: "#811a2a",
      contrastText: "#ffffff",
    },
    type: "dark",
    contrastThreshold: 3,
    tonalOffset: 0.2,
    background: {
      default: "#333333",
      paper: "#424242",
    },
  },
  typography: {
    fontFamily: ["Poppins", "sans-serif"].join(","),
  },
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 960,
      lg: 1280,
      xl: 1920,
      xxl: 2500,
    },
  },
  overrides: {
    MuiSelect: {
      select: {
        "&:focus": {
          backgroundColor: "transparant",
        },
      },
    },
    MuiButtonBase: {
      root: {
        "& .MuiSvgIcon-root": {
          marginTop: "-1px",
        },
      },
    },
    MuiTextField: {
      root: {
        width: "100%",
      },
    },
    MuiPaper: {
      rounded: {
        borderRadius: 3,
      },
    },
  },
  props: {
    MuiButton: {
      size: "small",
    },
    MuiFilledInput: {
      margin: "dense",
    },
    MuiFormControl: {
      margin: "dense",
    },
    MuiFormHelperText: {
      margin: "dense",
    },
    MuiIconButton: {
      size: "small",
    },
    MuiInputBase: {
      margin: "dense",
    },
    MuiInputLabel: {
      margin: "dense",
    },
    MuiListItem: {
      dense: true,
    },
    MuiOutlinedInput: {
      margin: "dense",
    },
    MuiFab: {
      size: "small",
    },
    MuiTable: {
      size: "small",
    },
    MuiTextField: {
      margin: "dense",
    },
    MuiToolbar: {
      variant: "dense",
    },
  },
};

export default createMuiTheme(overrides);
