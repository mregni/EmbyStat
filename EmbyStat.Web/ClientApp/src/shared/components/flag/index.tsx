import React from 'react';
import classNames from 'classnames';

import Ara from '../../assets/icons/flags/ara.svg';
import Bas from '../../assets/icons/flags/bas.svg';
import Bul from '../../assets/icons/flags/bul.svg';
import Cat from '../../assets/icons/flags/cat.svg';
import Chi from '../../assets/icons/flags/chi.svg';
import Cze from '../../assets/icons/flags/cze.svg';
import Dan from '../../assets/icons/flags/dan.svg';
import Dut from '../../assets/icons/flags/dut.svg';
import Eng from '../../assets/icons/flags/eng.svg';
import Est from '../../assets/icons/flags/est.svg';
import Fin from '../../assets/icons/flags/fin.svg';
import Fre from '../../assets/icons/flags/fre.svg';
import Ger from '../../assets/icons/flags/ger.svg';
import Gre from '../../assets/icons/flags/gre.svg';
import Heb from '../../assets/icons/flags/heb.svg';
import Hin from '../../assets/icons/flags/hin.svg';
import Hrv from '../../assets/icons/flags/hrv.svg';
import Hun from '../../assets/icons/flags/hun.svg';
import Ice from '../../assets/icons/flags/ice.svg';
import Ita from '../../assets/icons/flags/ita.svg';
import Ind from '../../assets/icons/flags/ind.svg';
import Jpn from '../../assets/icons/flags/jpn.svg';
import Kor from '../../assets/icons/flags/kor.svg';
import Lav from '../../assets/icons/flags/lav.svg';
import Lit from '../../assets/icons/flags/lit.svg';
import Mac from '../../assets/icons/flags/mac.svg';
import May from '../../assets/icons/flags/may.svg';
import Nor from '../../assets/icons/flags/nor.svg';
import Per from '../../assets/icons/flags/per.svg';
import Pol from '../../assets/icons/flags/pol.svg';
import Por from '../../assets/icons/flags/por.svg';
import PorBra from '../../assets/icons/flags/por-bra.svg';
import rum from '../../assets/icons/flags/rum.svg';
import Rus from '../../assets/icons/flags/rus.svg';
import Scc from '../../assets/icons/flags/scc.svg';
import Slo from '../../assets/icons/flags/slo.svg';
import Slv from '../../assets/icons/flags/slv.svg';
import Spa from '../../assets/icons/flags/spa.svg';
import Srp from '../../assets/icons/flags/srp.svg';
import Swa from '../../assets/icons/flags/swa.svg';
import Swe from '../../assets/icons/flags/swe.svg';
import Tha from '../../assets/icons/flags/tha.svg';
import Tur from '../../assets/icons/flags/tur.svg';
import Ukr from '../../assets/icons/flags/ukr.svg';
import Vie from '../../assets/icons/flags/vie.svg';
import { makeStyles } from '@material-ui/core';

const convertToIcon = (language: string): string => {
  switch (language) {
    case 'ara': return Ara;
    case 'baq': return Bas;
    case 'bul': return Bul;
    case 'cat': return Cat;
    case 'chi': return Chi;
    case 'cze': return Cze;
    case 'dan': return Dan;
    case 'nld':
    case 'dut': return Dut;
    case 'eng': return Eng;
    case 'est': return Est;
    case 'fin': return Fin;
    case 'fra':
    case 'frf':
    case 'fre': return Fre;
    case 'deu':
    case 'ger': return Ger;
    case 'gre': return Gre;
    case 'heb': return Heb;
    case 'mal':
    case 'hin': return Hin;
    case 'hrv': return Hrv;
    case 'hun': return Hun;
    case 'ice': return Ice;
    case 'ita': return Ita;
    case 'ind': return Ind;
    case 'jpn': return Jpn;
    case 'kor': return Kor;
    case 'lav': return Lav;
    case 'lit': return Lit;
    case 'mac': return Mac;
    case 'may': return May;
    case 'nob': return Nor;
    case 'nor': return Nor;
    case 'per': return Per;
    case 'pol': return Pol;
    case 'por': return Por;
    case 'Portuguese-Bra': return PorBra;
    case 'rum': return rum;
    case 'rus': return Rus;
    case 'scc': return Scc;
    case 'slo': return Slo;
    case 'slv': return Slv;
    case 'spa': return Spa;
    case 'srp': return Srp;
    case 'swa': return Swa;
    case 'swe': return Swe;
    case 'tha': return Tha;
    case 'tur': return Tur;
    case 'ukr': return Ukr;
    case 'vie': return Vie;
    case 'scr':
    default: return 'none';
  }
}

const useStyles = makeStyles((theme) => ({
  border: {
    border: 2,
    borderColor: 'white',
  },
}));


interface Props {
  language: string;
  className?: string;
  height: number | 'auto';
  width: number | 'auto';
  isDefault: boolean;
}

const Flag = (props: Props) => {
  const { language, className, height, width, isDefault } = props;
  const flag = convertToIcon(language);
  const classes = useStyles();

  return (
    flag !== 'none'
      ? <img
        src={flag}
        className={classNames(className, { [classes.border]: isDefault })}
        alt="flag"
        width={width}
        height={height} />
      : <div>{language != null ? language : 'und'}</div>
  )
}

Flag.defaultProps = {
  className: '',
  width: 'auto',
  height: 20,
  isDefault: false,
}

export default Flag
