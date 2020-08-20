import {
  Animation,
  ArgumentAxis,
  Chart as BarChart,
  CommonSeriesSettings,
  Font,
  Format,
  Label,
  Legend,
  LoadingIndicator,
  Margin,
  Series,
  Title,
  ValueAxis,
  Tooltip
} from 'devextreme-react/chart';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { withTheme } from '@material-ui/core/styles';
import { Theme } from '@material-ui/core/styles/createMuiTheme';

import { Chart } from '../../models/common';

interface Props {
  chart: Chart;
  theme: Theme;
}

const BarGraph = (props: Props) => {
  const { chart, theme } = props;
  const { t } = useTranslation();

  return (
    <BarChart
      palette="Soft Blue"
      paletteExtensionMode="blend"
      dataSource={JSON.parse(chart.dataSets)}
    >
      <CommonSeriesSettings
        argumentField="Label"
        type="bar"
        hoverMode="allArgumentPoints"
        selectionMode="allArgumentPoints"
      >
        <Label visible={true}>
          <Format type="fixedPoint" precision={0} />
        </Label>
      </CommonSeriesSettings>
      <Title text={t(chart.title)} horizontalAlignment="center">
        <Font size={30} color={theme.palette.primary.main} />
        <Margin top={25} />
      </Title>
      <Series valueField="Val0" name={t(chart.title)} />
      <ValueAxis position="right">
        <Title text={t("COMMON.COUNT")} />
      </ValueAxis>
      <ArgumentAxis>
        {" "}
        <Label
          displayMode="standard"
          rotationAngle={45}
          overlappingBehavior="rotate"
        />
      </ArgumentAxis>
      <Legend visible={false} />
      <LoadingIndicator enabled={true} />
      <Animation easing="linear" duration={500} maxPointCountSupported={100} />
    </BarChart>
  );
};

export default withTheme(BarGraph);
