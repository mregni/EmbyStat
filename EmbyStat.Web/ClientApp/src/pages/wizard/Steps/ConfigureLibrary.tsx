import Grid from '@material-ui/core/Grid'
import Typography from '@material-ui/core/Typography'
import React, { forwardRef, useContext, useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'

import { LibraryStepProps, ValidationHandleWithSave } from '.'
import LibrarySelector from '../../../shared/components/librarySelector'
import { WizardContext } from '../Context/WizardState'

export const ConfigureLibrary = forwardRef<ValidationHandleWithSave, LibraryStepProps>((props, ref) => {
  const { type } = props;
  const [typeNumber, setTypeNumber] = useState(0);
  const [selectedLibraries, setSelectedLibraries] = useState<string[]>([]);
  const { wizard, fullServerUrl, setMovieLibraries, setShowLibraries } = useContext(WizardContext);
  const { t } = useTranslation();

  useEffect(() => {
    if (type === 'movie') {
      setTypeNumber(1);
      setSelectedLibraries(wizard.movieLibraries);
      return;
    }

    if (type === 'show') {
      setTypeNumber(2);
      setSelectedLibraries(wizard.showLibraries);
    }
  }, [type, typeNumber, wizard.allLibraries, wizard.movieLibraries, wizard.showLibraries]);

  const saveLibrariesToWizard = (libraries: string[]): void => {
    if (type === 'movie') {
      setMovieLibraries(libraries);
      return;
    }

    if (type === 'show') {
      setShowLibraries(libraries);
      return;
    }
  }

  return (
    <Grid container direction="column" spacing={2}>
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('WIZARD.CONFIGURELIBRARIES', { type: type })}
        </Typography>
      </Grid>
      <Grid item>
        <Typography variant="body1">
          {t('WIZARD.SELECTLIBRARIES', { type: t(`COMMON.${type.toUpperCase()}`) })}
        </Typography>
      </Grid>
      <Grid item>
        <LibrarySelector
          allLibraries={wizard.allLibraries}
          libraries={selectedLibraries}
          address={fullServerUrl}
          saveList={(libraries) => saveLibrariesToWizard(libraries)}
        />
      </Grid>
    </Grid>
  )
})
