import { Subscription } from 'rxjs';

import { animate, AUTO_STYLE, state, style, transition, trigger } from '@angular/animations';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { MenuItems } from './shared/injectables/menu-items';
import { TitleService } from './shared/services/title.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  animations: [
    trigger('mobileHeaderNavRight', [
      state('nav-off, void',
        style({
          overflow: 'hidden',
          height: '0px',
        })
      ),
      state('nav-on',
        style({
          height: AUTO_STYLE,
        })
      ),
      transition('nav-off <=> nav-on', [
        animate('400ms ease-in-out')
      ])
    ])
  ]
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'EmbyStat';
  animateSidebar = '';
  pcodedDeviceType = 'desktop';
  verticalNavType = 'expanded';
  verticalEffect = 'shrink';
  layoutType = 'dark';
  headerFixedMargin = '50px';
  pcodedHeaderPosition = 'fixed';
  headerFixedTop = 'auto';
  navRight = 'nav-on';
  toggleOn = true;
  toggleIcon = 'icon-toggle-right';
  navBarTheme = 'themelight1';
  pcodedSidebarPosition = 'fixed';
  menuTitleTheme = 'theme1';
  isSidebarChecked = true;
  isHeaderChecked = true;
  sidebarFixedHeight = 'calc(100vh - 55px)';
  perfectDisable = '';
  sidebarFixedNavHeight = '';
  windowWidth = window.innerWidth;

  pageTitleSub: Subscription;
  pageTitle: string;

  scroll = (): void => {
    const scrollPosition = window.pageYOffset;
    if (scrollPosition > 50) {
      if (this.isSidebarChecked === true) {
        this.pcodedSidebarPosition = 'fixed';
      }
      if (this.pcodedDeviceType === 'desktop') {
        this.headerFixedTop = '0';
      }
      this.sidebarFixedNavHeight = '100%';
    } else {
      if (this.pcodedDeviceType === 'desktop') {
        this.headerFixedTop = 'auto';
      }
      this.pcodedSidebarPosition = 'absolute';
      this.sidebarFixedNavHeight = '';
    }
  }

  constructor(
    private readonly router: Router,
    public readonly menuItems: MenuItems,
    private readonly translate: TranslateService,
    private readonly titleService: TitleService) {
    this.translate.setDefaultLang('en-US');
    this.translate.addLangs(['en-US', 'nl-NL', 'de-DE', 'da-DK', 'el-GR', 'es-ES',
      'fi-FI', 'fr-FR', 'hu-HU', 'it-IT', 'no-NO', 'pl-PL', 'pt-BR', 'pt-PT', 'ro-RO',
      'sv-SE', 'cs-CZ']);

    this.setMenuAttributes(this.windowWidth);
    this.setHeaderAttributes(this.windowWidth);

    this.setLayoutType('dark');

    this.pageTitleSub = this.titleService.titleSubject.subscribe((title: string) => {
      this.pageTitle = title;
    });
  }

  ngOnInit() {
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }

  onResize(event) {
    this.windowWidth = event.target.innerWidth;
    this.setHeaderAttributes(this.windowWidth);

    let reSizeFlag = true;
    if (this.pcodedDeviceType === 'tablet' && this.windowWidth >= 768 && this.windowWidth <= 992) {
      reSizeFlag = false;
    } else if (this.pcodedDeviceType === 'phone' && this.windowWidth < 768) {
      reSizeFlag = false;
    }
    /* for check device */
    if (reSizeFlag) {
      this.setMenuAttributes(this.windowWidth);
    }
  }

  setHeaderAttributes(windowWidth) {
    if (windowWidth <= 992) {
      this.navRight = 'nav-off';
    } else {
      this.navRight = 'nav-on';
    }
  }

  setMenuAttributes(windowWidth) {
    if (windowWidth >= 768 && windowWidth <= 992) {
      this.pcodedDeviceType = 'tablet';
      this.verticalNavType = 'offcanvas';
      this.verticalEffect = 'overlay';
      this.toggleIcon = 'icon-toggle-left';
      this.headerFixedTop = '50px';
      this.headerFixedMargin = '0';
    } else if (windowWidth < 768) {
      this.pcodedDeviceType = 'phone';
      this.verticalNavType = 'offcanvas';
      this.verticalEffect = 'overlay';
      this.toggleIcon = 'icon-toggle-left';
      this.headerFixedTop = '50px';
      this.headerFixedMargin = '0';
    } else {
      this.pcodedDeviceType = 'desktop';
      this.verticalNavType = 'expanded';
      this.verticalEffect = 'shrink';
      this.toggleIcon = 'icon-toggle-right';
      this.headerFixedMargin = '50px';
    }
  }

  toggleHeaderNavRight() {
    this.navRight = this.navRight === 'nav-on' ? 'nav-off' : 'nav-on';
  }

  toggleOpened(e) {
    if (this.windowWidth <= 992) {
      this.toggleOn = this.verticalNavType === 'offcanvas' ? true : this.toggleOn;
      if (this.navRight === 'nav-on') {
        this.toggleHeaderNavRight();
      }
      this.verticalNavType = this.verticalNavType === 'expanded' ? 'offcanvas' : 'expanded';
    } else {
      this.verticalNavType = this.verticalNavType === 'expanded' ? 'collapsed' : 'expanded';
    }
    this.toggleIcon = this.verticalNavType === 'expanded' ? 'icon-toggle-right' : 'icon-toggle-left';
    this.animateSidebar = 'pcoded-toggle-animate';

    if (this.verticalNavType === 'collapsed') {
      this.perfectDisable = 'disabled';
      this.sidebarFixedHeight = '100%';
    } else {
      this.perfectDisable = '';
    }

    if (this.verticalNavType === 'collapsed' && this.isHeaderChecked === false) {
      this.setSidebarPosition();
    }

    setTimeout(() => {
      this.animateSidebar = '';
    }, 500);
  }

  onClickedOutsideSidebar(e: Event) {
    if ((this.windowWidth <= 992 && this.toggleOn && this.verticalNavType !== 'offcanvas') || this.verticalEffect === 'overlay') {
      this.toggleOn = true;
      this.verticalNavType = 'offcanvas';
      this.toggleIcon = 'icon-toggle-left';
    }
  }

  setNavBarTheme(theme: string) {
    if (theme === 'themelight1') {
      this.navBarTheme = 'themelight1';
      this.menuTitleTheme = 'theme1';
    } else {
      this.menuTitleTheme = 'theme9';
      this.navBarTheme = 'theme1';
    }
  }

  setLayoutType(type: string) {
    if (type === 'dark') {
      this.navBarTheme = 'theme1';
      document.querySelector('body').classList.add('dark');
      this.setBackgroundPattern('theme1');
      this.menuTitleTheme = 'theme9';
      this.layoutType = type;
    } else if (type === 'light') {
      this.navBarTheme = 'themelight1';
      this.menuTitleTheme = 'theme1';
      document.querySelector('body').classList.remove('dark');
      this.setBackgroundPattern('theme1');
      this.layoutType = type;
    } else if (type === 'img') {
      this.navBarTheme = 'themelight1';
      this.menuTitleTheme = 'theme1';
      document.querySelector('body').classList.remove('dark');
      this.setBackgroundPattern('theme1');
    }
  }

  setBackgroundPattern(pattern: string) {
    document.querySelector('body').setAttribute('themebg-pattern', pattern);
  }

  setSidebarPosition() {
    if (this.verticalNavType !== 'collapsed') {
      this.isSidebarChecked = !this.isSidebarChecked;
      this.pcodedSidebarPosition = this.isSidebarChecked === true ? 'fixed' : 'absolute';
      this.sidebarFixedHeight = this.isSidebarChecked === true ? 'calc(100vh - 50px)' : '100%';
      if (this.isHeaderChecked === false) {
        window.addEventListener('scroll', this.scroll, true);
        window.scrollTo(0, 0);
      }
    }
  }

  setHeaderPosition() {
    this.isHeaderChecked = !this.isHeaderChecked;
    this.pcodedHeaderPosition = this.isHeaderChecked === true ? 'fixed' : 'relative';
    this.headerFixedMargin = this.isHeaderChecked === true ? '50px' : '';
    if (this.isHeaderChecked === false) {
      window.addEventListener('scroll', this.scroll, true);
      window.scrollTo(0, 0);
    } else {
      window.removeEventListener('scroll', this.scroll, true);
      if (this.pcodedDeviceType === 'desktop') {
        this.headerFixedTop = 'auto';
      }
      this.pcodedSidebarPosition = 'fixed';
      if (this.verticalNavType !== 'collapsed') {
        this.sidebarFixedHeight = this.isSidebarChecked === true ? 'calc(100vh - 50px)' : 'calc(100vh + 50px)';
      }
    }
  }

  hoverOutsideSidebar() {
    if (this.verticalNavType === 'collapsed') {
      const mainEle = document.querySelectorAll('.pcoded-trigger');
      for (let i = 0; i < mainEle.length; i++) {
        mainEle[i].classList.remove('pcoded-trigger');
      }
    }
  }

  ngOnDestroy(): void {
    if (this.pageTitleSub !== undefined) {
      this.pageTitleSub.unsubscribe();
    }
  }
}
