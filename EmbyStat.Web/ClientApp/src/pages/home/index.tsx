import React, { ReactElement } from 'react'
import { Button } from '@material-ui/core'
import { useLocation } from 'react-router-dom';

import { fireJob } from '../../shared/services/JobService';


const Home = (): ReactElement => {
  const location = useLocation();




  return (
    <div>
      Hello from Home<br />
      <p>{location.pathname}</p>
      <Button color="secondary" variant="contained" onClick={() => fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f')}>Testing buttons</Button>
    </div>
  )
}

export default Home
