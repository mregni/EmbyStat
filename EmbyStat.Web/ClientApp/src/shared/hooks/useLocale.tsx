import {eachDayOfInterval, endOfWeek, format, Locale, startOfWeek} from 'date-fns';
import enUs from 'date-fns/locale/en-US';
import nl from 'date-fns/locale/nl';
import {useContext, useEffect, useState} from 'react';

import {SettingsContext} from '../context/settings';

export const useLocale = () => {
  const {userConfig} = useContext(SettingsContext);
  const [locale, setLocale] = useState<Locale>(null!);

  useEffect(() => {
    if (userConfig !== null) {
      setLocale(getLocale(userConfig.dateTimeLanguage));
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

  const getWeekDays = (): {isoIndex: string, value: string}[] => {
    const now = new Date();
    const weekDays: {isoIndex: string, value: string}[] = [];
    const start = startOfWeek(now, {locale});
    const end = endOfWeek(now, {locale});
    eachDayOfInterval({start, end}).forEach((day) => {
      let iso = format(day, 'i');
      if (iso === '7') {
        iso = '0';
      };
      weekDays.push({isoIndex: iso, value: format(day, 'EEEEEE', {locale})});
    });
    return weekDays;
  };

  return {locale, getLocale, getWeekDays};
};
