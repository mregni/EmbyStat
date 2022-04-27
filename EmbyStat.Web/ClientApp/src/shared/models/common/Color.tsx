export class Color {
  r: number;
  g: number;
  b: number;
  a: number;
  baseColor: string;
  hsl: {h:number, s:number, l:number};
  standardColorType = {
    re: /^#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})$/,
    process(colorString: string[]): number[] {
      return [parseInt(colorString[1], 16), parseInt(colorString[2], 16), parseInt(colorString[3], 16)];
    },
  };

  constructor(value: string) {
    this.baseColor = value;
    let color;
    if (value) {
      color = String(value).toLowerCase().replace(/ /g, '');
      color = this.parseColor(color);
    }

    color = color || {};
    this.r = this.normalize(color[0]);
    this.g = this.normalize(color[1]);
    this.b = this.normalize(color[2]);
    this.a = this.normalize(color[3], 1, 1);
    this.hsl = this.toHslFromRgb(this.r, this.g, this.b);
  }

  toHexFromHsl(): string {
    let r;
    let g;
    let b;
    const h = this.convertTo01Bounds(this.hsl.h, 360);
    const s = this.convertTo01Bounds(this.hsl.s, 100);
    const l = this.convertTo01Bounds(this.hsl.l, 100);
    if (s === 0) {
      r = g = b = l;
    } else {
      const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
      const p = 2 * l - q;
      r = this.hueToRgb(p, q, this.makeColorTint('r', h));
      g = this.hueToRgb(p, q, this.makeColorTint('g', h));
      b = this.hueToRgb(p, q, this.makeColorTint('b', h));
    }
    return '#' + (16777216 |
      Math.round(255 * r) << 16 |
      Math.round(255 * g) << 8 |
      Math.round(255 * b))
      .toString(16).slice(1);
  }

  makeColorTint(colorPart: string, h: number): number {
    let colorTint = h;
    if (colorPart === 'r') {
      colorTint = h + 1 / 3;
    }
    if (colorPart === 'b') {
      colorTint = h - 1 / 3;
    }
    return colorTint;
  }

  hueToRgb(p: number, q: number, colorTint: number): number {
    colorTint = this.modifyColorTint(colorTint);
    if (colorTint < 1 / 6) {
      return p + 6 * (q - p) * colorTint;
    }
    if (colorTint < 0.5) {
      return q;
    }
    if (colorTint < 2 / 3) {
      return p + (q - p) * (2 / 3 - colorTint) * 6;
    }
    return p;
  }

  modifyColorTint(colorTint: number): number {
    if (colorTint < 0) {
      colorTint += 1;
    }
    if (colorTint > 1) {
      colorTint -= 1;
    }
    return colorTint;
  }

  convertTo01Bounds(n: number, max: number) {
    n = Math.min(max, Math.max(0, n));
    if (Math.abs(n - max) < 1e-6) {
      return 1;
    }
    return n % max / max;
  }

  calculateHue(r: number, g: number, b: number, delta: number) {
    const max = Math.max(r, g, b);
    switch (max) {
    case r:
      return (g - b) / delta + (g < b ? 6 : 0);
    case g:
      return (b - r) / delta + 2;
    case b:
    default:
      return (r - g) / delta + 4;
    }
  }

  toHslFromRgb(r: number, g: number, b: number) {
    r = this.convertTo01Bounds(r, 255);
    g = this.convertTo01Bounds(g, 255);
    b = this.convertTo01Bounds(b, 255);
    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    const maxMinSum = max + min;
    let h;
    let s;
    const l = maxMinSum / 2;
    if (max === min) {
      h = s = 0;
    } else {
      const delta = max - min;
      if (l > 0.5) {
        s = delta / (2 - maxMinSum);
      } else {
        s = delta / maxMinSum;
      }
      h = this.calculateHue(r, g, b, delta);
      h /= 6;
    }
    return {
      h: Math.round(360 * h),
      s: Math.round(100 * s),
      l: Math.round(100 * l),
    };
  }

  normalize(colorComponent: number, def?: number, max?: number) {
    def = def || 0;
    max = max || 255;
    return colorComponent < 0 || isNaN(colorComponent) ?
      def :
      colorComponent > max ? max : colorComponent;
  }

  parseColor(color: string): number[] | null {
    const str = this.standardColorType.re.exec(color);
    if (str) {
      return this.standardColorType.process(str);
    }

    return null;
  }
}
