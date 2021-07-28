
export const parameters = {
  actions: { argTypesRegex: "^on[A-Z].*" },
  controls: {
    matchers: {
      color: /(background|color)$/i,
      date: /Date$/,
    },
  },
}

// import { muiTheme } from 'storybook-addon-material-ui';
// import { addDecorator } from '@storybook/react';

// import theme from '../src/styles/theme';
// addDecorator(muiTheme([theme]));


import { ThemeProvider } from '@material-ui/core';
import { createMuiTheme } from '@material-ui/core/styles';
import { addDecorator } from '@storybook/react';
import { withThemes } from '@react-theming/storybook-addon';
import theme from '../src/styles/theme';

const providerFn = ({ theme, children }) => {
  return (<div className="container-app">
    <ThemeProvider theme={theme}>
      {children}
    </ThemeProvider>
  </div>);
};

// pass ThemeProvider and array of your themes to decorator
addDecorator(withThemes(null, [theme], { providerFn }));