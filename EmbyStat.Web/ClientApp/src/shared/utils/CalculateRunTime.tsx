const calculateRunTime = (value: number | null): string => {
  return `${Math.round((value ?? 0) / 600000000)} min`;
};

export default calculateRunTime;
