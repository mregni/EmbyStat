import {Locale} from 'date-fns';
import enUs from 'date-fns/locale/en-US';
import nl from 'date-fns/locale/nl';
import {useContext, useEffect, useState} from 'react';

import {SettingsContext} from '../context/settings';

export const useLocale = () => {
  const {userConfig} = useContext(SettingsContext);
  const [locale, setLocale] = useState<Locale>(null!);

  useEffect(() => {
    if (userConfig !== null) {
      setLocale(getLocale(userConfig.language));
    }
  }, [userConfig]);

  const getLocale = (language: string): Locale => {
    switch (language) {
    case 'nl':
    case 'nl-NL':
    case 'nl-BE': return nl;
    case 'en-US':
    default: return enUs;
    }
  };

  return {locale, getLocale};
};
