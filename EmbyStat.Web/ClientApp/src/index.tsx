import './i18n';
import 'moment/locale/cs';
import 'moment/locale/da';
import 'moment/locale/de';
import 'moment/locale/el';
import 'moment/locale/es';
import 'moment/locale/fi';
import 'moment/locale/fr';
import 'moment/locale/hu';
import 'moment/locale/it';
import 'moment/locale/nl';
import 'moment/locale/nb';
import 'moment/locale/pl';
import 'moment/locale/pt-br';
import 'moment/locale/pt';
import 'moment/locale/ro';
import 'moment/locale/sv';

import './index.scss';

import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';

import App from './App';
import * as serviceWorker from './serviceWorker';
import store from './store';
import PageLoader from './shared/components/pageLoader';

function render() {
  ReactDOM.render(
    // <React.StrictMode>
    <Provider store={store}>
      <Suspense fallback={PageLoader}>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </Suspense>
    </Provider>,
    // </React.StrictMode>,
    document.getElementById('root')
  );
}

render();

if (process.env.NODE_ENV === 'development' && module.hot) {
  module.hot.accept('./App', render);
}

serviceWorker.unregister();
