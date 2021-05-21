import React from 'react';
import { Story, Meta } from '@storybook/react';

import EsButton, { Props } from '../shared/components/buttons/EsButton';

export default {
  title: 'Example/Button',
  component: EsButton,
  argTypes: {
    backgroundColor: { control: 'color' },
  },
} as Meta;

const Template: Story<Props> = (args) => <EsButton {...args} />;

export const Primary = Template.bind({});
Primary.args = {
  disable: false,
  isSaving: false
};

export const Saving = Template.bind({});
Saving.args = {
  disable: false,
  isSaving: true
};

export const Disabled = Template.bind({});
Disabled.args = {
  disable: true,
  isSaving: false
};
