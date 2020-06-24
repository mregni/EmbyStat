import { configureStore, Action, getDefaultMiddleware } from '@reduxjs/toolkit'
import { ThunkAction } from 'redux-thunk'

import rootReducer, { RootState } from './RootReducer'

const store = configureStore({
  reducer: rootReducer,
})

if (process.env.NODE_ENV === 'development' && module.hot) {
  module.hot.accept('./RootReducer', () => {
    const newRootReducer = require('./RootReducer').default
    store.replaceReducer(newRootReducer)
  })
}

export type AppDispatch = typeof store.dispatch
export type AppThunk = ThunkAction<void, RootState, null, Action<string>>

export default store
