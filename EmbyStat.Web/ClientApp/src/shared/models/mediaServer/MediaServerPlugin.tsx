export interface MediaServerPlugin {
  name: string;
  configurationDateLastModified: Date | string;
  version: string;
  assemblyFileName: string;
  configurationFileName: string;
  description: string;
  id: string;
  imageUrl: string;
}