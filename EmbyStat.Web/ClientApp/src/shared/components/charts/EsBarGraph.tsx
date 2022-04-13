import {
  Animation, ArgumentAxis, Chart as BarChart, CommonSeriesSettings, Font, Format, Label, Legend,
  LoadingIndicator, Margin, SeriesTemplate, Title, ValueAxis,
} from 'devextreme-react/chart';
import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Paper} from '@mui/material';

import {theme} from '../../../styles/theme';
import {Chart, SimpleData} from '../../models/common';

type Props = {
  chart: Chart;
}

export const EsBarGraph = (props: Props) => {
  const {chart} = props;
  const {t} = useTranslation();
  const [data, setData] = useState<SimpleData[]>(null!);

  useEffect(() => {
    setData(chart.dataSets);
  }, [chart.dataSets]);

  return (
    <Paper elevation={5}>
      <Box sx={{p: 2}}>
        <BarChart
          palette="Soft"
          dataSource={data}
        >
          <CommonSeriesSettings
            argumentField="label"
            type="bar"
            valueField="value"
            ignoreEmptyPoints={true}
            hoverMode="allArgumentPoints"
            selectionMode="allArgumentPoints"
          >
            <Label visible={true}>
              <Format type="fixedPoint" precision={0} />
            </Label>
          </CommonSeriesSettings>
          <SeriesTemplate nameField="label" />
          <Title text={t(chart.title)} horizontalAlignment="center">
            <Font size={30} color={theme.palette.secondary.main} weight="bold" />
            <Margin top={25} />
          </Title>
          <ValueAxis position="left">
            <Title text={t('COMMON.COUNT')} />
          </ValueAxis>
          <ArgumentAxis>
            {' '}
            <Label
              displayMode="standard"
              rotationAngle={-45}
              overlappingBehavior="rotate"
            />
          </ArgumentAxis>
          <Legend visible={false} />
          <LoadingIndicator enabled={false} />
          <Animation easing="linear" duration={500} maxPointCountSupported={100} />
        </BarChart>
      </Box>
    </Paper>
  );
};
