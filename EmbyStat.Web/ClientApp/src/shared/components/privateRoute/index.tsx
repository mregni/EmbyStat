import React, { useState, useEffect } from 'react';
import { Route, Redirect } from 'react-router-dom';

import { isUserLoggedIn, checkUserRoles } from '../../services/AccountService';
import { UserRoles } from '../../models/login';

interface Props {
  children: React.ReactNode;
  roles?: string[];
  path: string | string[];
  exact?: boolean;
}

const PrivateRoute = (props: Props) => {
  const {
    children,
    roles = [UserRoles.User],
    path,
    exact = false
  } = props;
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const checkUser = async () => {
      const result = await isUserLoggedIn();
      if (result) {
        const includeRole = checkUserRoles(roles);
        setIsAuthenticated(includeRole);
        setIsLoading(false);
        return;
      }
      setIsAuthenticated(false);
      setIsLoading(false);
    };

    checkUser();
  }, [roles]);

  return (
    <Route
      path={path}
      exact={exact}
      render={(props) =>
        isLoading
          ? null
          : isAuthenticated
            ? (children)
            : (<Redirect to={{ pathname: '/login', state: { referer: props.location } }} />)}
    />
  );
};

export default PrivateRoute;
