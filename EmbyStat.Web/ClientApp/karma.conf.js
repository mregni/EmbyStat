// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine', '@angular-devkit/build-angular'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-coverage'),
      require('karma-junit-reporter'),
      require('@angular-devkit/build-angular/plugins/karma'),
      require('karma-coverage-istanbul-reporter'),
      require('karma-sonarqube-unit-reporter')
    ],
    client: {
      clearContext: false // leave Jasmine Spec Runner output visible in browser
    },
    angularFilesort: {
      whitelist: ['/**/!(*.html|*.spec|*.mock).js']
    },
    reporters: ['progress', 'junit', 'sonarqubeUnit'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ['Chrome'],
    singleRun: false,
    junitReporter: {
      outputDir: '../coverage',
      suite: 'models'
    },
    coverageIstanbulReporter: {
      dir: require('path').join(__dirname, '../coverage'),
      reports: ['lcovonly'],
      fixWebpackSourcePaths: true,
      thresholds: {
        emitWarning: true,
        global: {
          statements: 65,
          lines: 65,
          branches: 65,
          functions: 65
        },
        each: {
          statements: 65,
          lines: 65,
          branches: 65,
          functions: 65
        }
      }
    },
    sonarQubeUnitReporter: {
      sonarQubeVersion: 'LATEST',
      outputFile: '../coverage/ut_report.xml',
      useBrowserName: false
    },
    preprocessors: {
      'app/**/*.js': 'coverage',
      'app/*.js': 'coverage'
    }
  })
}
