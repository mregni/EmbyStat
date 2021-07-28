import React from 'react';
import { Story, Meta } from '@storybook/react';

import { EsSaveButton, Props } from '../shared/components/buttons/EsSaveButton';

export default {
  title: 'Example/SaveButton',
  component: EsSaveButton,
  argTypes: {
    backgroundColor: { control: 'color' },
  },
} as Meta;

const Template: Story<Props> = (args) => <EsSaveButton {...args} />;

export const Primary = Template.bind({});
Primary.args = {
  disabled: false,
  isSaving: false
};

export const Saving = Template.bind({});
Saving.args = {
  disabled: false,
  isSaving: true
};

export const Disabled = Template.bind({});
Disabled.args = {
  disabled: true,
  isSaving: false
};
