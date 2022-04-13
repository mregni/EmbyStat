/* eslint-disable max-len */
import {Box} from '@mui/material';
import React, {ReactElement, useContext} from 'react';

import construction from '../../shared/assets/images/under-construction.webp';
import {SettingsContext} from '../../shared/context/settings';

export const Home = (): ReactElement => {
  const settings = useContext(SettingsContext);

  console.log(settings.settings);
  return (
    <Box>
      <h1>Welcome to EmbyStat</h1>
      <p>This is a static welcome page (not translated). Currently this is still a work in progress and a lot of stuff is not yet implemented or broken. If you find a bug feel free to log it on <a href="https://github.com/mregni/EmbyStat" rel="noopener noreferrer" target="_blank">https://github.com/mregni/EmbyStat</a></p>
      <p>I did a full remake of the frontend in React (instead of Angular). It has taken me a lot of time but I think that the UI looks better and less buggy. Currently the biggest thing that is missing are shows. They will be re-added in the next beta release soon.</p>
      <p>Any page with the following image is in need of review by me, so it can be buggy.</p>
      <img src={construction} alt="work in progress" width={100} className="m-t-16" />
    </Box>
  );
};

