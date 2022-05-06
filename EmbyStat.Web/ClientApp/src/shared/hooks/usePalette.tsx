import {Color} from '../models/common';

export const usePalette = () => {
  const getColor = (index: number, count: number) => {
    const palette = ['#cbc87b', '#9ab57e', '#e55253', '#7e4452', '#e8c267', '#565077', '#6babac', '#ad6082'];
    const paletteCount = palette.length;

    const cycles = Math.floor((count - 1) / paletteCount + 1);
    const color = palette[index % paletteCount];
    if (cycles > 1) {
      return (function(color, cycleIndex, cycleCount) {
        const colorObj = new Color(color);
        let l = colorObj.hsl.l / 100;
        const diapason = cycleCount - 1 / cycleCount;
        let minL = l - 0.5 * diapason;
        let maxL = l + 0.5 * diapason;
        const cycleMiddle = (cycleCount - 1) / 2;
        const cycleDiff = cycleIndex - cycleMiddle;
        if (minL < Math.min(0.5, 0.9 * l)) {
          minL = Math.min(0.5, 0.9 * l);
        }
        if (maxL > Math.max(0.8, l + 0.15 * (1 - l))) {
          maxL = Math.max(0.8, l + 0.15 * (1 - l));
        }
        if (cycleDiff < 0) {
          l -= (minL - l) * cycleDiff / cycleMiddle;
        } else {
          l += cycleDiff / cycleMiddle * (maxL - l);
        }
        colorObj.hsl.l = 100 * l;
        return colorObj.toHexFromHsl();
      }(color, Math.floor(index / paletteCount), cycles));
    }
    return color;
  };

  const generatePalette = (count: number): string[] => {
    const colors = [];
    for (let i = 0; i < count; i++) {
      colors.push(getColor(i, count));
    }
    return colors;
  };

  return generatePalette;
};
