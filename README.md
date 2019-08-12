![# Logo!](https://github.com/mregni/EmbyStat/blob/master/EmbyStat.Web/ClientApp/src/assets/images/logo_color.png?raw=true)

[![Gitter](https://img.shields.io/gitter/room/embystat/EmbyStat.js.svg)](https://gitter.im/EmbyStat/Lobby)
![GitHub (pre-)release](https://img.shields.io/github/release/mregni/embystat/all.svg)
![Github All Releases](https://img.shields.io/github/downloads/mregni/embystat/total.svg)
[![Docker Pulls](https://img.shields.io/docker/pulls/uping/embystat.svg)](https://hub.docker.com/r/uping/embystat/)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/mregni/embystat.svg)
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/embystat/localized.svg)](https://crowdin.com/project/embystat)
![Codacy](https://api.codacy.com/project/badge/Grade/92431e9931574cf2a663242fde86c47f)
[![All Contributors](https://img.shields.io/badge/all_contributors-10-orange.svg?style=flat-square)](#contributors)

# Intro

EmbyStat is a personal web server that can calculate all kinds of statistics from your (local) Emby server. Just install this on your server and let him calculate all kinds of fun stuff.

This project is still in Alpha fase, but feel free to pull in on your computer and test it out yourself. When the time is right I will host a full informational website/release for common platforms and Wiki pages.

## Build state

| Environment  | Build  | Deploys |
|---|---|---|
| Nightly  | [![Build status](https://dev.azure.com/uping-be/EmbyStat/_apis/build/status/DEV)](https://dev.azure.com/uping-be/EmbyStat/_build/latest?definitionId=2)  | DOCKER win10-x64 ![Deploy status](https://vsrm.dev.azure.com/uping-be/_apis/public/Release/badge/60c279c7-80b3-445f-8d2e-45e07778a9e6/1/1) |
| | |DOCKER ubuntu-x64 ![Deploy status](https://vsrm.dev.azure.com/uping-be/_apis/public/Release/badge/60c279c7-80b3-445f-8d2e-45e07778a9e6/1/2) |
| Beta | | DOCKER win10-x64 ![Deploy status](https://vsrm.dev.azure.com/uping-be/_apis/public/Release/badge/60c279c7-80b3-445f-8d2e-45e07778a9e6/2/3)
| | | DOCKER ubuntu-x64 ![Deploy status](https://vsrm.dev.azure.com/uping-be/_apis/public/Release/badge/60c279c7-80b3-445f-8d2e-45e07778a9e6/2/4) |
| | | Github packages ![Deploy status](https://vsrm.dev.azure.com/uping-be/_apis/public/Release/badge/60c279c7-80b3-445f-8d2e-45e07778a9e6/2/5)

## Installation

Supported platforms as the moment are:

* Windows x64 (IIS and Kestrel)
* Windows x86 (IIS and Kestrel)
* Docker Windows [beta-win10-x64](https://hub.docker.com/r/uping/embystat/)
* Docker Linux [beta-ubuntu-x64](https://hub.docker.com/r/uping/embystat/)
* Debian package
* Ubuntu package
* Fedora package

I will try to support as many platforms as possible. More and more will come in the future!
If you have knowledge about building unix packages please feel free to contact me so I can support as many platforms as possible.

## Installation Guides

For full installation guides please see the [Wiki](https://github.com/mregni/EmbyStat/wiki) page

## Translations

Translations are managed with the Crowdin web service. Feel free to help us translate the application in your own language here: [https://crowdin.com/project/embystat](https://crowdin.com/project/embystat). If your language is not listed just create a new feature request or ping me.

## Feature request

If you have some crazy idea you want to see implemented in EmbyStat you can create a feature request  on [https://embystat.featureupvote.com/](https://embystat.featureupvote.com/)
Or just go and upvote your favorite features so I have a better idea about what I should build next.

## Technology

This project is build from scratch with the following technologies:

* .NET CORE 2.2
* LiteDB
* SignalR
* NLog
* Angular 5
* Rollbar

## Roadmap

A lot of things need to be done before I can go life with this and publish my first 1.0 release. This will happen when all features from the Emby statistics plugin are implemented in EmbyStat.

## Contributors

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<table>
  <tr>
    <td align="center"><a href="http://uping.be"><img src="https://avatars3.githubusercontent.com/u/22617019?v=4" width="100px;" alt="Mikhaël Regni"/><br /><sub><b>Mikhaël Regni</b></sub></a><br /><a href="#projectManagement-mregni" title="Project Management">📆</a> <a href="https://github.com/mregni/EmbyStat/commits?author=mregni" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/jaketame"><img src="https://avatars0.githubusercontent.com/u/1787973?v=4" width="100px;" alt="Jake Tamé"/><br /><sub><b>Jake Tamé</b></sub></a><br /><a href="#question-jaketame" title="Answering Questions">💬</a></td>
    <td align="center"><a href="https://github.com/Darkassassin07"><img src="https://avatars3.githubusercontent.com/u/6593614?v=4" width="100px;" alt="Darkassassin07"/><br /><sub><b>Darkassassin07</b></sub></a><br /><a href="#userTesting-Darkassassin07" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/commits?author=Darkassassin07" title="Tests">⚠️</a></td>
    <td align="center"><a href="https://github.com/jaycedk"><img src="https://avatars3.githubusercontent.com/u/17232146?v=4" width="100px;" alt="jaycedk"/><br /><sub><b>jaycedk</b></sub></a><br /><a href="#userTesting-jaycedk" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3Ajaycedk" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/Happy2Play"><img src="https://avatars1.githubusercontent.com/u/10268557?v=4" width="100px;" alt="Doug"/><br /><sub><b>Doug</b></sub></a><br /><a href="#userTesting-Happy2Play" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3AHappy2Play" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/diedrich"><img src="https://avatars2.githubusercontent.com/u/638793?v=4" width="100px;" alt="Diedrich"/><br /><sub><b>Diedrich</b></sub></a><br /><a href="#userTesting-Diedrich" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3ADiedrich" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/adrianwi"><img src="https://avatars1.githubusercontent.com/u/15711539?v=4" width="100px;" alt="adrianwi"/><br /><sub><b>adrianwi</b></sub></a><br /><a href="#userTesting-adrianwi" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3Aadrianwi" title="Bug reports">🐛</a></td>
  </tr>
  <tr>
    <td align="center"><a href="https://github.com/wilhelmstroker"><img src="https://avatars1.githubusercontent.com/u/45869623?v=4" width="100px;" alt="WilhelmStroker"/><br /><sub><b>WilhelmStroker</b></sub></a><br /><a href="#userTesting-WilhelmStroker" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3AWilhelmStroker" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/gerritv"><img src="https://avatars1.githubusercontent.com/u/8202594?v=4" width="100px;" alt="Gerrit Visser"/><br /><sub><b>Gerrit Visser</b></sub></a><br /><a href="#userTesting-gerritv" title="User Testing">📓</a> <a href="https://github.com/mregni/EmbyStat/issues?q=author%3Agerritv" title="Bug reports">🐛</a></td>
    <td align="center"><a href="https://github.com/diedrichg"><img src="https://avatars0.githubusercontent.com/u/9559987?v=4" width="100px;" alt="diedrichg"/><br /><sub><b>diedrichg</b></sub></a><br /><a href="#design-diedrichg" title="Design">🎨</a></td>
  </tr>
</table>

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
