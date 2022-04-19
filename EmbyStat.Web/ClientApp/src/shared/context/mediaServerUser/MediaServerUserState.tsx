import {createContext} from 'react';

export interface MediaServerUserProps {
};

export const MediaServerUserContext = createContext<MediaServerUserProps>(null!);

export const useMediaServerUserContext = (): MediaServerUserProps => {
  return { };
};
