module.exports = {
  root: true,
  env: {
    "browser": true
  },
  parser: '@typescript-eslint/parser',
  plugins: [
    '@typescript-eslint',
  ],
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/eslint-recommended',
    'plugin:@typescript-eslint/recommended',
  ],
  rules: {
	'@typescript-eslint/no-unused-vars': 0,
	'@typescript-eslint/no-explicit-any': 0
  },
  overrides: [
    {
      files: [ "**/*.reducer.ts", "**/app.component.ts", "**/app.module.ts", "**/app.actions.ts" ],
      rules: {
        "@typescript-eslint/explicit-function-return-type": 0
      }
    }
  ]
};