export interface Language {
  name: string;
  code: string;
}

export interface LanguageContainer {
  languages: Language[];
  isLoaded: boolean;
}
