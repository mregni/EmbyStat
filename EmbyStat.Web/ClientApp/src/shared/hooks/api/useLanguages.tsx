import { useEffect, useState } from "react";
import { FetchState } from "..";
import { Language } from "../../models/language";
import { getLanguages } from "../../services/SettingsService";

export const useLanguages = () => {
  const [languageList, setLanguageList] = useState<Language[]>([]);
  const [state, setState] = useState<FetchState>(FetchState.notStarted);

  useEffect(() => {
    let didCancel = false;

    const fetchData = async () => {
      try {
        setState(FetchState.initiated);
        const result = await getLanguages();

        if (!didCancel) {
          setLanguageList(result.data);
        }
      } catch (error) {
        if (!didCancel) {
          setState(FetchState.failed);
        }
      }
      finally {
        setState(FetchState.success);
      }
    };

    fetchData();

    return () => {
      didCancel = true;
    };
  }, []);

  return {
    languageList, state
  }
}