import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import moment from 'moment';
import { JobLogLine, EnhancedJobLogLine } from '../shared/models/jobs';

const initialState: EnhancedJobLogLine[] = [];

const jobLogsSlice = createSlice({
  name: "jobLogs",
  initialState,
  reducers: {
    receiveLog(state, action: PayloadAction<JobLogLine>) {
      const time = moment().format('LTS');
      const date = moment().format('L');
      const textArray = action.payload.value.split('=>');
      const left = `${time} ${date} - ${textArray[0]}`;

      let lines = [...state];
      const newLine = {
        left: left,
        right: textArray.length > 1 ? textArray[1] : '',
        type: action.payload.type,
      }

      lines.push(newLine);

      if (lines.length >= 20) {
        lines.shift();
      }

      return lines;
    }
  }
});

export default jobLogsSlice;
