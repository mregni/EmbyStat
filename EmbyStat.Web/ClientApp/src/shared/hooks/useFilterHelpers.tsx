import {useTranslation} from 'react-i18next';

import {ActiveFilter, FilterOperation} from '../models/filter';

export const useFilterHelpers = () => {
  const {t} = useTranslation();

  const calculateValue = (value: string, filterField: string): string => {
    switch (filterField) {
    case 'RunTimeTicks':
      if (value.includes('|')) {
        return `${parseInt(value.split('|')[0], 10) * 600000000}|${parseInt(value.split('|')[1], 10) * 600000000}`;
      }
      return `${parseInt(value, 10) * 600000000}`;
    case 'Images':
    case 'Genres':
    case 'Container':
    case 'Subtitles':
    case 'Codec':
    case 'VideoRange':
      return `${value.split('|')[0]}`;
    default:
      return encodeURIComponent(value);
    }
  };

  const generateLabel = (filter: ActiveFilter): string => {
    return filter.fieldLabel
      .replace(/\{0\}/g, t(filter.operation.label))
      .replace(/\{1\}/g, filter.fieldValue);
  };

  const calculateValueLabel = (value: string, operation: FilterOperation): string => {
    switch (operation.type) {
    case 'dropdown':
      return `${value.split('|')[1]}`;
    default:
      break;
    }

    switch (operation.operation) {
    case 'between':
      const unit = operation.unit != null ? t(operation.unit) : '';
      return `
      ${value.split('|')[0]}${unit} ${t('COMMON.AND')} ${value.split('|')[1]}${unit}`;
    case 'null':
      return '';
    default:
      return `${value}${operation.unit != null ? t(operation.unit) : ''}`;
    }
  };

  return {calculateValue, generateLabel, calculateValueLabel};
};
