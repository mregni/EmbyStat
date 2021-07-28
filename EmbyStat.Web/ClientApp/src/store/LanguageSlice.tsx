import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppDispatch, AppThunk } from '.';
import { Language, LanguageContainer } from '../shared/models/language';
import { getLanguages } from '../shared/services/SettingsService';
import { RootState } from './RootReducer';

const initialState: LanguageContainer = {
  languages: [],
  isLoaded: false,
};

const languageSlice = createSlice({
  name: 'language',
  initialState,
  reducers: {
    receiveLanguages(state, action: PayloadAction<Language[]>) {
      return {
        languages: action.payload,
        isLoaded: true,
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    },
  },
});

export const loadLanguages = (): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  if (!getState().languages.isLoaded) {
    const languages = await getLanguages();
    dispatch(languageSlice.actions.receiveLanguages(languages.data));
  } else {
    dispatch(languageSlice.actions.alreadyLoaded());
  }
};

export default languageSlice.reducer;
