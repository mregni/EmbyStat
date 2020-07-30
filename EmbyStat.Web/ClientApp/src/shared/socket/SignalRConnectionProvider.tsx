import React, { Component } from 'react';
import {
  JsonHubProtocol,
  HttpTransportType,
  HubConnectionBuilder,
  LogLevel,
  HubConnection,
} from '@aspnet/signalr';

import { Store } from 'redux';
import jobSlice from '../../store/JobSlice';
import { Job } from '../models/jobs';
import jobLogsSlice from '../../store/JobLogsSlice';
import ServerStatusSlice from '../../store/ServerStatusSlice';

interface Props {
  store: Store;
}

interface State {
  hubConnection: HubConnection;
}

class SignalRConnectionProvider extends Component<Props, State> {
  componentDidMount() {
    if (this.state === null) {
      console.log('*********CONNECTING********');
      const connectionHub = `${window.location.origin}/jobs-socket`;
      console.log(connectionHub);
      const protocol = new JsonHubProtocol();
      const transport =
        HttpTransportType.WebSockets | HttpTransportType.LongPolling;
      const options = {
        transport,
        logMessageContent: true,
        logger: LogLevel.Error,
      };

      const connection = new HubConnectionBuilder()
        .withUrl(connectionHub, options)
        .withHubProtocol(protocol)
        .build();

      this.setState({ hubConnection: connection }, () => {
        this.startSignalRConnection(this.state.hubConnection);

        this.state.hubConnection.on(
          'JobReportProgress',
          this.JobReportProgressReceived
        );
        this.state.hubConnection.on(
          'JobReportLog',
          this.onJobReportLogReceived
        );
        this.state.hubConnection.on(
          'MediaServerConnectionState',
          this.onMissedPingStatusReceived
        );
        this.state.hubConnection.on('UpdateState', this.onNotifReceived);
      });
    }
  }

  startSignalRConnection = (connection) =>
    connection
      .start()
      .then(() => console.info('SignalR Connected'))
      .catch((err) => console.error('SignalR Connection Error: ', err));

  componentWillUnmount() {
    this.state.hubConnection
      .stop()
      .then(() => console.info('SignalR Connection closed'))
      .catch((err) => console.error("Can't close SignalR Connection: ", err));
  }

  onNotifReceived = (res) => {
    console.log('****** NOTIFICATION ******', res);
  };

  onMissedPingStatusReceived = (res: number) => {
    console.log(res);
    const { store } = this.props;
    store.dispatch(ServerStatusSlice.actions.receivePingResult(res));
  }

  onJobReportLogReceived = (res) => {
    const { store } = this.props;
    store.dispatch(jobLogsSlice.actions.receiveLog(res));
  };

  JobReportProgressReceived = (res: Job) => {
    const { store } = this.props;
    store.dispatch(jobSlice.actions.updateJob(res));
  };

  render() {
    const { children } = this.props;
    return <>{children}</>;
  }
}

export default SignalRConnectionProvider;
