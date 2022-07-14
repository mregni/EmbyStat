import React, {useEffect} from 'react';
import {useParams} from 'react-router';

import {Grid, Stack} from '@mui/material';

import {EsLoading} from '../../../shared/components/esLoading';
import {useUserDetails} from '../hooks/useUserDetails';
import {User} from './components';
import {Header} from './components/Header';

type Props = {}

export function UserDetails(props: Props) {
  const {id} = useParams();
  const {loading, userDetails, fetchUserDetails} = useUserDetails();

  useEffect(() => {
    (async () => {
      await fetchUserDetails(id);
    })();
  }, [id]);

  return (
    <EsLoading loading={loading} label="Loading user data">
      <Stack spacing={1}>
        <Header details={userDetails} />
        <Grid direction="row" container>
          <Grid item xs={12} md={4} xl={3}>
            <User details={userDetails} />
          </Grid>
        </Grid>
      </Stack>
    </EsLoading>
  );
}
