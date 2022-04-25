import PieChart, {
  Animation, Connector, Font, Label, Legend, LoadingIndicator, Margin, Series, Title, Tooltip,
} from 'devextreme-react/pie-chart';
import React, {ReactElement, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Paper, useTheme} from '@mui/material';

import i18n from '../../../i18n';
import {theme} from '../../../styles/theme';
import {Chart, SimpleData} from '../../models/common';

const formatNumber = new Intl.NumberFormat('en-US', {
  minimumFractionDigits: 0,
}).format;

const calculateTotal = (pieChart: any): string => {
  return formatNumber(pieChart
    .getAllSeries()[0]
    .getVisiblePoints()
    .reduce((s: number, p: any) => s + p.data.value, 0));
};

function CenterTemplate(pieChart: any): ReactElement {
  return (
    <svg>
      <circle cx="100" cy="100" r={pieChart.getInnerRadius() - 6} fill={theme.palette.background.paper} />
      <text textAnchor="middle" x="100" y="100" style={{fontSize: 18, fill: theme.palette.text.primary}}>
        <tspan x="100">{i18n.t('COMMON.TOTAL')}</tspan>
        <tspan x="100" dy="20px" style={{fontWeight: 600}}>{
          calculateTotal(pieChart)
        }</tspan>
      </text>
    </svg>
  );
}

interface Props {
  chart: Chart;
}

export function EsPieGraph(props: Props) {
  const {chart} = props;
  const theme = useTheme();
  const {t} = useTranslation();
  const [data, setData] = useState<SimpleData[]>(null!);

  useEffect(() => {
    setData(chart.dataSets);
  }, [chart.dataSets]);

  const formatText = (arg: { argumentText: string; percentText: string; }) => {
    return `${arg.argumentText} (${arg.percentText})`;
  };

  return (
    <Paper elevation={5}>
      <Box sx={{p: 2}}>
        <PieChart
          type="doughnut"
          palette="Soft"
          dataSource={data}
          resolveLabelOverlapping="shift"
          centerRender={CenterTemplate}
        >
          <Series argumentField="label" valueField="value">
            <Label visible={true} customizeText={formatText}>
              <Connector visible={true} />
            </Label>
          </Series>
          <Legend visible={false}/>
          <Tooltip enabled={false}/>
          <Title text={t(chart.title)} horizontalAlignment="center">
            <Font size={30} color={theme.palette.secondary.main} weight="bold" />
            <Margin top={25} />
          </Title>
          <Animation easing="linear" duration={500} maxPointCountSupported={100} />
          <LoadingIndicator enabled={true} />
        </PieChart>
      </Box>
    </Paper>
  );
}

