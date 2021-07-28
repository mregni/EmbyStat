import i18n from 'i18next';
import { AxiosResponse } from 'axios';

import { axiosInstance } from './axiosInstance';
import { Settings } from '../models/settings';
import { Language } from '../models/language';
import SnackbarUtils from '../utils/SnackbarUtilsConfigurator';

const domain = 'settings/';

export const getSettings = async (): Promise<Settings> => {
  const response = await axiosInstance.get<Settings>(`${domain}`);
  return response.data;
};

export const updateSettings = async (userSettings: Settings): Promise<Settings | void> => {
  const config = { headers: { 'Content-Type': 'application/json' } };
  return await axiosInstance.put<Settings>(`${domain}`, userSettings, config)
    .then((response) => {
      if (response.status === 200) {
        SnackbarUtils.success(i18n.t('SETTINGS.SAVED'))
      }
      return response.data;
    })
    .catch((x) => {
      SnackbarUtils.error(i18n.t('SETTINGS.SAVEFAILED'));
    });
};

export const getLanguages = async (): Promise<AxiosResponse<Language[]>> => {
  return await axiosInstance.get<Language[]>(`${domain}languages`);
};

// export const useGetlanguage = () => {
//   const [languageList, setLanguageList] = useState<Language[]>([]);
//   const [isLoading, setIsLoading] = useState(false);
//   const [inError, setInError] = useState(false);

//   useEffect(() => {
//     let didCancel = false;

//     const fetchData = async () => {
//       try {
//         setIsLoading(true);
//         const result = await axiosInstance.get<Language[]>(`${domain}languages`);

//         if (!didCancel) {
//           setLanguageList(result.data);
//         }
//       } catch (error) {
//         if (!didCancel) {
//           setInError(true);
//         }
//       }
//       finally {
//         setIsLoading(false);
//       }
//     };

//     fetchData();

//     return () => {
//       didCancel = true;
//     };
//   }, []);

//   return {
//     languageList, isLoading, inError
//   }
// }

