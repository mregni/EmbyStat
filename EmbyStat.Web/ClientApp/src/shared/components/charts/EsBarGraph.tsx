import React, {useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {
  Bar, BarChart, CartesianGrid, Cell, Label, LabelList, LabelProps, ResponsiveContainer, XAxis,
  YAxis,
} from 'recharts';

import {Box, Paper, Stack, Typography} from '@mui/material';

import i18n from '../../../i18n';
import {theme} from '../../../styles/theme';
import {usePalette} from '../../hooks';
import {Chart, SimpleData} from '../../models/common';

type Props = {
  chart: Chart;
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
        x={x + width/2}
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

export function EsBarGraph(props: Props) {
  const {chart, height = 250} = props;
  const generatePalette = usePalette();
  const {t} = useTranslation();
  const [data, setData] = useState<SimpleData[]>(null!);
  const [toLongLabels, setToLongLabels] = useState(false);
  const [isAnimation, setIsAnimation] = useState(true);

  useEffect(() => {
    if (data != null) {
      setToLongLabels(data.some((x) => x.label.length >= 7));
    }
  }, [data]);

  useEffect(() => {
    setData(chart.dataSets);
  }, [chart.dataSets]);

  const colors = useMemo(() => {
    return generatePalette(chart.dataSets.length);
  }, [chart.dataSets.length]);

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
              angle={toLongLabels ? -45 : 0}
              stroke={fontColor}
              dy={toLongLabels ? 12 : 5}
              height={50}
              fontSize={10}
              interval={0}
            />
            <YAxis stroke={fontColor}/>
            <Bar dataKey='value'
              isAnimationActive={false}
            >
              <LabelList content={renderCustomizedLabel} />
              {data != null && data.map((e, index) => (
                <Cell
                  // eslint-disable-next-line react/no-array-index-key
                  key={`cell-${index}`}
                  fill={colors[index % colors.length] ?? '#fff'}
                />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </Stack>
    </Paper>
  );
}
