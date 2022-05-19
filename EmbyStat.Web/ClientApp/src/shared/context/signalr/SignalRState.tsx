/* eslint-disable no-console */
import {useContext, useEffect, useState} from 'react';

import {
  HttpTransportType, HubConnection, HubConnectionBuilder, JsonHubProtocol, LogLevel,
} from '@microsoft/signalr';

import {JobLogLine, JobProgress} from '../../models/jobs';
import {JobsContext} from '../jobs';
import {SettingsContext} from '../settings';

export const useSignalR = () => {
  const [hubConnetion, setHubConnetion] = useState<HubConnection |null>(null);
  const {setProgress} = useReportProgressReceived();
  const {addLogLine} = useReportLogReceived();
  const {databaseReset, addResetLogLine} = useResetReceived();

  const connect = () => {
    const connectionHub = '/hub';
    const protocol = new JsonHubProtocol();
    const transport = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
    const options = {
      transport,
      logMessageContent: true,
      logger: LogLevel.Debug,
    };

    const connection = new HubConnectionBuilder()
      .withUrl(connectionHub, options)
      .withHubProtocol(protocol)
      .withAutomaticReconnect()
      .build();

    connection.on('JobReportProgress', setProgress);
    connection.on('JobReportLog', addLogLine);
    connection.on('ResetDatabaseFinished', databaseReset);
    connection.on('ResetDatabaseLogLine', addResetLogLine);
    connection
      .start()
      .then(() => console.info('SignalR Connected'))
      .catch((err) => console.error('SignalR Connection Error: ', err));

    setHubConnetion(connection);
  };

  useEffect(() => {
    if (hubConnetion == null) {
      connect();
    }

    return function cleanup() {
      if (hubConnetion != null) {
        hubConnetion.stop()
          .then(() => console.info('SignalR Connection closed'))
          .catch((err) => console.error('Can\'t close SignalR Connection: ', err)); ;
      }
    };
  }, [hubConnetion]);
};

const useReportProgressReceived = () => {
  const {onProgressReceived} = useContext(JobsContext);
  const [internalProgress, setInernalProgress] = useState<JobProgress>(null!);

  const setProgress = (progress: JobProgress) => {
    setInernalProgress(progress);
  };

  useEffect(() => {
    if (internalProgress !== null) {
      onProgressReceived(internalProgress);
    }
  }, [internalProgress]);

  return {setProgress};
};

const useReportLogReceived = () => {
  const {onLogReceived} = useContext(JobsContext);
  const [logLine, setLogLine] = useState<JobLogLine>(null!);

  const addLogLine = (line: JobLogLine) => {
    setLogLine(line);
  };

  useEffect(() => {
    if (logLine !== null) {
      onLogReceived(logLine);
    }
  }, [logLine]);

  return {addLogLine};
};

const useResetReceived = () => {
  const {setResetFinished, setResetLogLine} = useContext(SettingsContext);

  const databaseReset = () => {
    setResetFinished(true);
    setResetLogLine('');
  };

  const addResetLogLine = (line: {line: string}) => {
    setResetLogLine(line.line);
  };

  return {databaseReset, addResetLogLine};
};
