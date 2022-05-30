export const calculateFileSize = (value: number): string => {
  let suffix = 'MB';
  if (value > 1073741824) {
    value /= 1073741824;
    suffix = 'PB';
  } else if (value > 1048576) {
    value /= 1048576;
    suffix = 'TB';
  } else if (value > 1024) {
    value /= 1024;
    suffix = 'GB';
  } else {
    return `${value} ${suffix}`;
  }

  return `${value.toFixed(2)} ${suffix}`;
};
