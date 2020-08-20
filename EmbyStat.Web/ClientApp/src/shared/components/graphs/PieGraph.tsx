import PieChart, {
  Legend,
  Series,
  Tooltip,
  Format,
  Label,
  Connector,
  Title,
  Font,
  Margin,
  Animation,
  LoadingIndicator
} from 'devextreme-react/pie-chart';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { withTheme } from '@material-ui/core/styles';
import { Theme } from '@material-ui/core/styles/createMuiTheme';

import { Chart } from '../../models/common';

interface Props {
  chart: Chart;
  theme: Theme;
}

const PieGraph = (props: Props) => {
  const { chart, theme } = props;
  const { t } = useTranslation();

  const customizeTooltip = (arg) => {
    return {
      text: `${arg.valueText} - ${(arg.percentText)}`
    };
  }

  return (
    <PieChart
      type="doughnut"
      palette="Soft Blue"
      dataSource={JSON.parse(chart.dataSets)}
    >
      <Series argumentField="Label" valueField="Val0">
        <Label visible={true}>
          <Connector visible={true} />
        </Label>
      </Series>
      <Legend
        horizontalAlignment="right"
        verticalAlignment="top"
      />
      <Tooltip enabled={true} customizeTooltip={customizeTooltip} />
      <Title text={t(chart.title)} horizontalAlignment="center">
        <Font size={30} color={theme.palette.primary.main} />
        <Margin top={25} />
      </Title>
      <Animation easing="linear" duration={500} maxPointCountSupported={100} />
      <LoadingIndicator enabled={true} />
    </PieChart>
  );
};

export default withTheme(PieGraph);
