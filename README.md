![# Logo!](https://github.com/mregni/EmbyStat/blob/master/EmbyStat.Web/ClientApp/src/assets/images/logo_color.png?raw=true)

[![Gitter](https://img.shields.io/gitter/room/embystat/EmbyStat.js.svg)](https://gitter.im/EmbyStat/Lobby)
![GitHub (pre-)release](https://img.shields.io/github/release/mregni/embystat/all.svg)
![Github All Releases](https://img.shields.io/github/downloads/mregni/embystat/total.svg)
[![Docker Pulls](https://img.shields.io/docker/pulls/uping/embystat.svg)](https://hub.docker.com/r/uping/embystat/)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/mregni/embystat.svg)
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/embystat/localized.svg)](https://crowdin.com/project/embystat)
![Codacy](https://api.codacy.com/project/badge/Grade/92431e9931574cf2a663242fde86c47f)
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)

# Intro

EmbyStat is a personal web server that can calculate all kinds of statistics from your (local) Emby server. Just install this on your server and let him calculate all kinds of fun stuff.

This project is still in Alpha fase, but feel free to pull in on your computer and test it out yourself. When the time is right I will host a full informational website/release for common platforms and Wiki pages.

## Installation

Supported platforms as the moment are:

* Windows x64 (IIS and Kestrel)
* Windows x86 (IIS and Kestrel)
* Docker Windows [latest-win](https://hub.docker.com/r/uping/embystat/)
* Docker Linux [latest-linux](https://hub.docker.com/r/uping/embystat/)
* Debian package (coming soon)

I will try to support as many platforms as possible. More and more will come in the future!
If you have knowledge about building unix packages please feel free to contact me so I can support as many platforms as possible.

## Installation Guides

For full installation guides please see the [Wiki](https://github.com/mregni/EmbyStat/wiki) page

## Translations

Translations are managed with the Crowdin web service. Feel free to help us translate the application in your own language here: [https://crowdin.com/project/embystat](https://crowdin.com/project/embystat). If your language is not listed just create a new feature request or ping me.

## Technology

This project is build from scratch with the following technologies:

* .NET CORE 2.0
* Sqlite
* SignalR
* Serilog
* Angular 5
* SCSS styling

## Roadmap

A lot of things need to be done before I can go life with this and publish my first 1.0 release. This will happen when all features from the Emby statistics plugin are implemented in EmbyStat.

## Contributors

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
<table><tr><td align="center"><a href="http://uping.be"><img src="https://avatars3.githubusercontent.com/u/22617019?v=4" width="100px;" alt="Mikhaël Regni"/><br /><sub><b>Mikhaël Regni</b></sub></a><br /><a href="#projectManagement-mregni" title="Project Management">📆</a> <a href="https://github.com/mregni/EmbyStat/commits?author=mregni" title="Code">💻</a></td></tr></table>

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
