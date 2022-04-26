import React, {forwardRef, useContext, useEffect, useImperativeHandle} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {
  Box, Card, CardContent, FormControl, Grid, MenuItem, Select, Typography,
} from '@mui/material';

import {EsTextInput} from '../../../../shared/components/esTextInput';
import {WizardContext} from '../../../../shared/context/wizard/WizardState';
import {ValidationHandleWithSave} from '../../Interfaces';

export const TestSuccessFul = forwardRef<ValidationHandleWithSave>(function TestSuccessFul(props, ref) {
  const {wizard, setMediaServerNetworkInfo, setAdministrator} = useContext(WizardContext);
  const {t} = useTranslation();

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      const {address, selectedAddress} = getValues();

      if (selectedAddress !== 'other') {
        return Promise.resolve(true);
      }

      await trigger();
      const addressArray = (address).split('://');
      if (isValid) {
        if (!['http', 'https'].includes(addressArray[0])) {
          setError('address', {type: 'protocol'});
          return Promise.resolve(false);
        }

        if (addressArray[1].split(':')[0].length === 0) {
          setError('address', {type: 'url'});
          return Promise.resolve(false);
        }

        if (addressArray[1].split(':')[1].split('/')[0] === undefined ||
          isNaN(parseInt(addressArray[1].split(':')[1].split('/')[0], 10))) {
          setError('address', {type: 'port'});
          return Promise.resolve(false);
        }
      }

      return Promise.resolve(true);
    },
    saveChanges(): void {
      const {address, selectedAdmin} = getValues();

      setMediaServerNetworkInfo({
        address: selectedAddress !== 'other' ? wizard.address : address,
        apiKey: wizard.apiKey,
        id: wizard.serverId,
        name: wizard.serverName,
        type: wizard.serverType,
      });

      const admin = wizard.administrators.find((x) => x.id === selectedAdmin);
      if (admin !== undefined) {
        setAdministrator(admin.id);
      }
    },
  }));

  const {register, getValues, trigger, watch, control, setError,
    clearErrors, formState: {isValid, errors}} = useForm({
    mode: 'onBlur',
    defaultValues: {
      address: '',
      selectedAddress: wizard.address,
      selectedAdmin: wizard.administrators[0].id,
    },
  });

  const selectedAddress = watch('selectedAddress');
  const addressRegister = register('address', {
    validate:
      (value: string) => selectedAddress !== 'other' || value.length > 0,
  });

  useEffect(() => {
    clearErrors('address');
  }, [selectedAddress, clearErrors]);

  return (
    <Grid item={true} container={true} xs={12} direction="column">
      <Grid item={true}>
        <Card
          elevation={7}
          square={true}
          sx={{
            mt: 4,
            display: 'flex',
            padding: '8px',
            position: 'relative',
          }}
        >
          <CardContent>
            <Grid container={true} direction="column" spacing={2}>
              <Grid item={true}>
                {t('WIZARD.LANWANCHOISE', {
                  type: wizard.serverType === 0 ?
                    'Emby' :
                    'Jellyfin',
                },
                )}
              </Grid>
              <Grid item={true}>
                <FormControl style={{width: '100%'}}>
                  <Controller
                    name="selectedAddress"
                    control={control}
                    render={({field}) => (
                      <Select
                        sx={{
                          width: '100%',
                          marginTop: [2, 2, 2, 0, 0],
                        }}
                        variant="outlined"
                        {...field}
                      >
                        {
                          wizard.address !== wizard.mediaServerInfo.wanAddress &&
                          wizard.address !== wizard.mediaServerInfo.localAddress &&
                          <MenuItem value={wizard.address}>
                            {t('COMMON.CURRENT')}&nbsp;
                            <Box component="span" sx={{fontStyle: 'italic'}}>
                              ({wizard.address})
                            </Box>
                          </MenuItem>
                        }
                        {
                          wizard.mediaServerInfo.wanAddress !== null &&
                          <MenuItem value={wizard.mediaServerInfo.wanAddress}>
                            WAN&nbsp;
                            <Box component="span" sx={{fontStyle: 'italic'}}>
                              ({wizard.mediaServerInfo.wanAddress})
                            </Box>
                          </MenuItem>
                        }
                        <MenuItem value={wizard.mediaServerInfo.localAddress}>
                          LAN&nbsp;
                          <Box component="span" sx={{fontStyle: 'italic'}}>
                            ({wizard.mediaServerInfo.localAddress})
                          </Box>
                        </MenuItem>
                        <MenuItem value="other">{t('COMMON.OTHER')}</MenuItem>
                      </Select>
                    )}
                  />
                </FormControl>
              </Grid>
              <Grid item={true}>
                <EsTextInput
                  readonly={selectedAddress !== 'other'}
                  inputRef={addressRegister}
                  defaultValue={getValues('address')}
                  error={errors.address}
                  errorText={{
                    validate: t('SETTINGS.MEDIASERVER.NOADDRESS'),
                    protocol: 'Protocol should be http or https',
                    url: 'URL be longer then 0 characters',
                    port: 'Port should be a number between 1 and 65535',
                  }}
                  label={t('SETTINGS.MEDIASERVER.ADDRESS')}
                />
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      </Grid>
      {wizard.serverType === 1 ? (
        <Grid item={true}>
          <Card
            elevation={7}
            square={true}
            sx={{
              mt: 4,
              display: 'flex',
              padding: '8px',
              position: 'relative',
            }}
          >
            <CardContent>
              <Grid container={true} direction="column" spacing={2}>
                <Grid item={true}>
                  <Typography gutterBottom={true}>
                    {t('WIZARD.JELLYFIN.ADMINTEXT')}
                  </Typography>
                </Grid>
                <Grid item={true}>
                  <Controller
                    name="selectedAdmin"
                    control={control}
                    render={({field}) => (
                      <Select
                        variant="outlined"
                        {...field}
                      >
                        {
                          wizard.administrators.map((admin) => (
                            <MenuItem value={admin.id} key={admin.id}>{t(admin.name)}</MenuItem>
                          ))
                        }
                      </Select>)}
                  />
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
      ) : null}
    </Grid>
  );
});
