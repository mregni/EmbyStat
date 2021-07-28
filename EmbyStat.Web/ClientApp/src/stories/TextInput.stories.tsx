import React from 'react';
import { Story, Meta } from '@storybook/react';

import { Props, EsTextInput } from '../shared/components/esTextInput/esTextInput';

export default {
  title: 'Example/TextInput',
  component: EsTextInput,
  argTypes: {
    backgroundColor: { control: 'color' },
  },
} as Meta;

const Template: Story<Props> = (args) => <EsTextInput {...args} />;

export const Primary = Template.bind({});
Primary.args = {
  defaultValue: '',
  inputRef: {},
};
