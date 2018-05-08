![# Logo!](https://github.com/mregni/EmbyStat/blob/master/EmbyStat.Web/ClientApp/src/assets/images/logo_color.png?raw=true)


![GitHub (pre-)release](https://img.shields.io/github/release/mregni/embystat/all.svg?style=flat-square)
[![GitHub issues-closed](https://img.shields.io/github/issues/mregni/EmbyStat.svg?style=flat-square)](https://GitHub.com/mregni/EmbyStat/issues?q=is%3Aissue+is%3Aopen)
![GitHub last commit](https://img.shields.io/github/last-commit/mregni/embystat.svg?style=flat-square)
![Libraries.io for GitHub](https://img.shields.io/librariesio/github/mregni/embystat.svg?style=flat-square)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/mregni/embystat.svg?style=flat-square)

EmbyStat is a personal web server that can calculate all kinds of statistics from your (local) Emby server. Just install this on your server and let him calculate all kinds of fun stuff. For more reallife statistics you do have to install the EmbyStat plugin on your Emby server as well!

This project is still in Alpha fase, but feel free to pull in on your computer and test it out yourself. When the time is right I will host a full informational website/release for common platforms and Wiki pages.

# Installation
Supported platforms as the moment are:
* Windows x64 (IIS and Kestrel)
* Windows x86 (IIS and Kestrel)
* Docker Windows [latest-win](https://hub.docker.com/r/uping/embystat/)
* Docker Linux [latest-linux](https://hub.docker.com/r/uping/embystat/)
* Debian package (coming soon)

I will try to support as many platforms as possible. More and more will come in the future!
If you have knownlegde about building unix packages please feel free to contact me so I can support as many platforms as possible.

# Installation Guides
For full installation guides please see the [Wiki](https://github.com/mregni/EmbyStat/wiki) page

# Technology
This project is build from scratch with the following technologies:
* .NET CORE 2.0
* Sqlite
* SignalR
* Serilog
* Angular 5
* SCSS styling

# Roadmap
A lot of things need to be done before I can go life with this and publish my first 1.0 release. This will happen when all features from the Emby statistics plugin are implemented in EmbyStat. 
