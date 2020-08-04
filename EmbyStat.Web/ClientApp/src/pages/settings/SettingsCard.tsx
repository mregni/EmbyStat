import React, { ReactNode } from 'react'
import Grid from '@material-ui/core/Grid';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import Zoom from '@material-ui/core/Zoom';
import Button from '@material-ui/core/Button';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
  title: {
    textTransform: 'capitalize'
  },
}));

interface Props {
  delay: number;
  title: string;
  children: ReactNode | ReactNode[];
  saveForm: () => void;
}

const SettingsCard = (props: Props) => {
  const classes = useStyles();
  const { delay, title, children, saveForm } = props;
  const { t } = useTranslation();

  return (
    <Zoom in={true} style={{ transitionDelay: delay + `ms` }}>
      <Card>
        <CardContent>
          <Typography variant="h5" color="primary" gutterBottom className={classes.title}>
            {title}
          </Typography>
          <form autoComplete="off">
            <Grid container direction="column" spacing={1}>
              {children}
              <Grid item container direction="row" justify="flex-end" className="m-t-16">
                <Button variant="contained" color="primary" onClick={saveForm}>
                  {t('COMMON.SAVE')}
                </Button>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
    </Zoom>
  )
}

export default SettingsCard
