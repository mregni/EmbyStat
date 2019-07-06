import { Injectable } from '@angular/core';

const colors: string[] = [
    '#2fab11',
    '#5ecbdd',
    '#753328',
    '#d8a34b',
    '#165af7',
    '#26cb95',
    '#9350c2',
    '#3987cb',
    '#732816',
    '#51a4dd',
    '#c35e8f',
    '#5f8b5f',
    '#772574',
    '#0cbdf4',
    '#9bd053',
    '#790796',
    '#fd9211',
    '#22b09e',
    '#08af65',
    '#967bb7',
    '#bbfc7a',
    '#66e402',
    '#ff6d45'
];

const barOptions: Options = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
        yAxes: [{
            gridLines: {
                display: true
            },
            ticks: {
                beginAtZero: true,
                fontColor: '#CCC',
                autoSkip: false
            }
        }],
        xAxes: [{
            gridLines: {
                display: false
            },
            ticks: {
                beginAtZero: true,
                autoSkip: false,
                fontColor: '#CCC'
            }
        }]
    },
    legend: { display: false },
    title: {
        display: true,
        fontColor: '#FFF',
        text: ''
    },
    tooltips: {
        display: true,
        callbacks: {
            label: function (tooltipItem, data) {
                return ' ' + Math.round(tooltipItem.yLabel * 100) / 100;
            }
        }
    }
};

@Injectable()
export class OptionsService {
    getPieOptions(): any {
        return {
            responsive: true,
            maintainAspectRatio: false,
            cutoutPercentage: 70, legend: { labels: { fontColor: '#FFF' } }
        };
    }

    getBarOptions(): Options {
        return barOptions;
    }

    getColors(length: number): string[] {
        const list = colors;
        while (list.length < length) {
            list.push(...colors);
        }
        return list.slice(0, length);
    }
}

export interface Options {
    responsive: boolean;
    maintainAspectRatio: boolean;
    scales: Scales;
    legend: Legend;
    title: Title;
    tooltips: Tooltips;
}

export interface Scales {
    yAxes: Axes[];
    xAxes: Axes[];
}

export interface Axes {
    ticks: Ticks;
    gridLines: GridLines;
}

export interface Ticks {
    beginAtZero: boolean;
    fontColor: string;
    autoSkip: boolean;
}

export interface GridLines {
    display: boolean;
}

export interface Legend {
    display: boolean;
}

export class Title {
    display: boolean;
    text: string;
    fontColor: string;
}

export interface Tooltips {
    callbacks: Callbacks;
    display: boolean;
}

export interface Callbacks {
    label: Function;
}
