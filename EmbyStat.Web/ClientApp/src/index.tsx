import React, {Suspense} from 'react';
import ReactDOM from 'react-dom';
import {BrowserRouter} from 'react-router-dom';

import './index.css';
import './i18n';
import App from './App';
import reportWebVitals from './reportWebVitals';
import {EsPageLoader} from './shared/components/esPageLoader';
import {SettingsContextProvider} from './shared/context/settings';
import {UserContextProvider} from './shared/context/user';
import {JobsContextProvider} from './shared/context/jobs';

ReactDOM.render(
  <React.StrictMode>
    <SettingsContextProvider>
      <JobsContextProvider>
        <UserContextProvider>
          <Suspense fallback={<EsPageLoader />}>
            <BrowserRouter>
              <App />
            </BrowserRouter>
          </Suspense>
        </UserContextProvider>
      </JobsContextProvider>
    </SettingsContextProvider>
  </React.StrictMode>,
  document.getElementById('root'),
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
