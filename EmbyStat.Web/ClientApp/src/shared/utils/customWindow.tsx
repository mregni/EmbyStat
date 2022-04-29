export interface CustomWindow extends Window {
  runConfig: Config
}

export interface Config {
  featureupvoteUrl: string;
  githubUrl: string;
  crowdinUrl: string;
  githubReleaseUrl: string;
}
