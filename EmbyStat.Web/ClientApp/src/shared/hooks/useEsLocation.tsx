import * as router from 'react-router-dom';

export interface RouteState {
  from: Location;
}

export function useEsLocation() {
  type L = router.Location & { state: RouteState };
  return router.useLocation() as L;
}
