import React, {useEffect, useMemo, useState} from 'react';
import {Cell, Label, Pie, PieChart, ResponsiveContainer, Sector} from 'recharts';

import {darken, Paper, Stack, Typography} from '@mui/material';

import i18n from '../../../i18n';
import {theme} from '../../../styles/theme';
import {usePalette} from '../../hooks';
import {Chart, SimpleData} from '../../models/common';

const color = theme.palette.getContrastText(theme.palette.background.paper);

interface ActiveShapeProps {
  cx: number;
  cy: number;
  midAngle: number;
  innerRadius: number;
  outerRadius: number;
  startAngle: number;
  endAngle: number;
  fill: string;
  payload: {label: string};
  percent: number;
  value: number;
}

const renderActiveShape = (props: ActiveShapeProps) => {
  const RADIAN = Math.PI / 180;
  const {cx, cy, midAngle, innerRadius, outerRadius, startAngle, endAngle, fill, payload, percent, value} = props;
  const sin = Math.sin(-RADIAN * midAngle);
  const cos = Math.cos(-RADIAN * midAngle);
  const sx = cx + (outerRadius + 10) * cos;
  const sy = cy + (outerRadius + 10) * sin;
  const mx = cx + (outerRadius + 30) * cos;
  const my = cy + (outerRadius + 30) * sin;
  const ex = mx + (cos >= 0 ? 1 : -1) * 22;
  const ey = my;
  const textAnchor = cos >= 0 ? 'start' : 'end';

  const colorDark = darken(color, 0.3);

  return (
    <g>
      <Sector
        cx={cx}
        cy={cy}
        innerRadius={innerRadius}
        outerRadius={outerRadius}
        startAngle={startAngle}
        endAngle={endAngle}
        fill={fill}
      />
      <Sector
        cx={cx}
        cy={cy}
        startAngle={startAngle}
        endAngle={endAngle}
        innerRadius={outerRadius + 6}
        outerRadius={outerRadius + 10}
        fill={fill}
      />
      <path d={`M${sx},${sy}L${mx},${my}L${ex},${ey}`} stroke={fill} fill="none" />
      <circle cx={ex} cy={ey} r={2} fill={fill} stroke="none" />
      <text x={ex + (cos >= 0 ? 1 : -1) * 12} y={ey} textAnchor={textAnchor} fill={color}>
        {`${value} ${payload.label}`}
      </text>
      <text x={ex + (cos >= 0 ? 1 : -1) * 12} y={ey} dy={15} fontSize={12} textAnchor={textAnchor} fill={colorDark}>
        {`${(percent * 100).toFixed(2)}%`}
      </text>
    </g>
  );
};

interface Props {
  chart: Chart;
  height?: number;
}

export function EsPieGraph(props: Props) {
  const {chart, height = 250} = props;
  const [data, setData] = useState<SimpleData[]>(null!);
  const [activeIndex, setActiveIndex] = useState(0);
  const generatePalette = usePalette();

  useEffect(() => {
    setData(chart.dataSets);
  }, [chart.dataSets]);

  const onPieMouseOverEnter = (_: any, index: number) => {
    setActiveIndex(index);
  };

  const colors = useMemo(() => {
    return generatePalette(chart.dataSets.length);
  }, [chart.dataSets.length]);

  return (
    <Paper elevation={5} sx={{pt: 2}}>
      <Stack>
        <Typography variant="h5" color="secondary" align="center">
          {i18n.t(chart.title)}
        </Typography>
        <ResponsiveContainer width="100%" height={height}>
          <PieChart width={400} height={height}>
            <Pie
              activeIndex={activeIndex}
              activeShape={renderActiveShape}
              nameKey='label'
              dataKey='value'
              data={data}
              cx="50%"
              cy="50%"
              innerRadius={40}
              paddingAngle={5}
              stroke="none"
              outerRadius={60}
              onMouseEnter={onPieMouseOverEnter}
            >

              {data != null && data.map((e, i) => (
                // eslint-disable-next-line react/no-array-index-key
                <Cell key={`cell-${i}`} fill={colors[i % colors.length] ?? '#fff'} />
              ))}

              <Label
                value={data !== null ? data.reduce((pv, cv) => pv + cv.value, 0) : 0}
                position='center'
                fill={color}
                fontSize={22}
                fontWeight="bold"
              />
            </Pie>
          </PieChart>
        </ResponsiveContainer>
      </Stack>
    </Paper>
  );
}

