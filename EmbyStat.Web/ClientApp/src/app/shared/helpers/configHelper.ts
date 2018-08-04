import { Configuration } from '../../configuration/models/configuration';

 export module ConfigHelper {
  export function getFullEmbyAddress(config: Configuration): string {
    const protocol = config.embyServerProtocol === 0 ? 'http://' : 'https://';
    const url = protocol + config.embyServerAddress + ':' + config.embyServerPort;
    return url;
  }
}
