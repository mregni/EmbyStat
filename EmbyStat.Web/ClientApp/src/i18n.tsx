import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import HttpApi from 'i18next-http-backend';
import LanguageDetector from 'i18next-browser-languagedetector';

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
      checkWhitelist: true,
    },
    backend: {
      loadPath: translationFilePath,
    },
    whitelist: [
      'cs-CZ',
      'da-DK',
      'de-DE',
      'el-GR',
      'en-US',
      'es-ES',
      'fi-FI',
      'fr-FR',
      'hu-HU',
      'it-IT',
      'nl-NL',
      'no-NO',
      'pl-PL',
      'pt-BR',
      'pt-PT',
      'ro-RO',
      'sv-SE',
    ],
    fallbackLng: 'en-US',
    debug: false,
    interpolation: {
      escapeValue: false,
    },
    react: {
      wait: true,
    },
  });

export const getLanguage = () => i18n.language || window.localStorage.i18nextLng;

export default i18n;
