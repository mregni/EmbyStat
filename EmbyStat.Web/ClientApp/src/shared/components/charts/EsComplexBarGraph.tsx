import {format} from 'date-fns';
import React, {useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {
  Bar, BarChart, CartesianGrid, Cell, LabelList, LabelProps, Legend, ResponsiveContainer, Tooltip,
  XAxis, YAxis,
} from 'recharts';

import {Box, Paper, Stack, Typography} from '@mui/material';

import i18n from '../../../i18n';
import {theme} from '../../../styles/theme';
import {useLocale, usePalette} from '../../hooks';
import {Chart, ComplexChart, SimpleData} from '../../models/common';

type Props = {
  chart: ComplexChart;
  height?: number;
}

const renderCustomizedLabel = (props: LabelProps): React.ReactNode => {
  const {x, y, width, value, fill} = props;

  if (x === undefined ||
    y === undefined ||
    width === undefined
  ) {
    return (null);
  }

  if (typeof x === 'string' ||
    typeof y === 'string' ||
    typeof width === 'string'
  ) {
    return (null);
  }

  const fontColor = theme.palette.getContrastText(fill ?? '#000');

  return (
    <g>
      <rect x={x} y={y - 20} width={width} height={15} fill={fill} />
      <text
        x={x + width / 2}
        y={y - 12}
        fontSize={14}
        fill={fontColor}
        textAnchor="middle"
        dominantBaseline="middle"
      >
        {value}
      </text>
    </g>
  );
};

export function EsComplexBarGraph(props: Props) {
  const {chart, height = 250} = props;
  const generatePalette = usePalette();
  const {t} = useTranslation();
  const {locale, getWeekDays} = useLocale();
  const [data, setData] = useState<[]>(null!);


  useEffect(() => {
    setData(JSON.parse(chart.dataSets));
  }, [chart.dataSets]);

  const colors = useMemo(() => {
    return generatePalette(chart.series.length);
  }, [chart.series.length]);

  const formatX = (tickItem: string): string => {
    if (chart.formatString === 'week') {
      const weekDays = getWeekDays();
      const index = weekDays.findIndex((x) => x.isoIndex === tickItem);
      if (index !== -1) {
        return weekDays[index].value;
      }
      return '';
    } else if (['p'].includes(chart.formatString)) {
      return format(new Date(tickItem), chart.formatString, {locale});
    }

    return '';
  };

  const fontColor = theme.palette.getContrastText(theme.palette.background.paper);
  return (
    <Paper elevation={5} sx={{pt: 2}}>
      <Stack>
        <Typography variant="h5" color="secondary" align="center">
          {t(chart.title)}
        </Typography>
        <ResponsiveContainer width="100%" height={height}>
          <BarChart
            height={height}
            data={data}
            margin={{
              top: 20,
              right: 25,
              left: 0,
              bottom: 5,
            }}
          >
            <CartesianGrid
              vertical={false}
              strokeDasharray="4" />
            <XAxis
              dataKey="label"
              angle={-45}
              stroke={fontColor}
              dy={5}
              height={50}
              fontSize={10}
              interval={0}
              tickFormatter={(tickItem) => formatX(tickItem)}
            />
            <Legend />
            <Tooltip />
            <YAxis stroke={fontColor} />
            {
              chart.series.map((id, i) => {
                return (
                  <Bar
                    stackId="a"
                    key={i}
                    dataKey={id}
                    isAnimationActive={false}
                    fill={colors[i]}
                  />
                );
              },
              )
            }
          </BarChart>
        </ResponsiveContainer>
      </Stack>
    </Paper>
  );
}
