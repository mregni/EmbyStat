import React from 'react';

import {calculateFileSize} from '../../shared/utils';

const cases = [
  [1, '1 MB'],
  [1024, '1024 MB'],
  [1025, '1.00 GB'],
  [4000, '3.91 GB'],
  [1048576, '1024.00 GB'],
  [1048577, '1.00 TB'],
  [4048577, '3.86 TB'],
  [1073741824, '1024.00 TB'],
  [1073741825, '1.00 PB'],
  [5073741825, '4.73 PB'],
];

describe('calculateFileSize utility', () => {
  it.each(cases)(
    'given %p as argument, returns %p',
    (firstArg, expectedResult) => {
      const result = calculateFileSize(firstArg as number);
      expect(result).toEqual(expectedResult);
    },
  );
});
