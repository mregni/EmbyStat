import i18n from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import HttpApi from 'i18next-http-backend';
import {initReactI18next} from 'react-i18next';

let translationFilePath = '/locales/{{lng}}.json';
if (process.env.NODE_ENV === 'development' && module.hot) {
  translationFilePath = '/locales/base.json';
}

i18n
  .use(HttpApi)
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    detection: {
      order: [
        'querystring',
        'cookie',
        'localStorage',
        'navigator',
        'htmlTag',
        'path',
        'subdomain',
      ],
    },
    backend: {
      loadPath: translationFilePath,
    },
    supportedLngs: [
      'cs',
      'da',
      'de',
      'el',
      'en',
      'es',
      'fi',
      'fr',
      'hu',
      'it',
      'nl',
      'no',
      'pl',
      'pt',
      'pt',
      'ro',
      'sv',
    ],
    fallbackLng: 'en',
    debug: false,
    interpolation: {
      escapeValue: false,
    },
    react: {
      useSuspense: true,
    },
  });

export const getLanguage = () => i18n.language || window.localStorage.i18nextLng;

export default i18n;
