export const calculateFileSize = (value: number): string => {
  let suffix = 'MB';
  if (value < 1000000) {
    value /= 1024;
    suffix = 'GB';
  } else if (value < 1000000000) {
    value /= 1048576;
    suffix = 'TB';
  } else if (value < 1000000000000) {
    value /= 1073741824;
    suffix = 'PB';
  }

  return `${value.toFixed(2)} ${suffix}`;
};
