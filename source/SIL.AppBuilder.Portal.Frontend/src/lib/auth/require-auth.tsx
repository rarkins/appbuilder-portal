import * as React from 'react';
import { RouterProps, Redirect } from 'react-router';
import { withCurrentUser } from '@data/containers/with-current-user';
import { isLoggedIn, hasVerifiedEmail } from '@lib/auth0';
import * as toast from '@lib/toast';

import { requireAuthHelper } from './require-auth-helper';
import { storePath } from './return-to';

export function requireAuth(opts = {}) {
  return (Component) => {
    // this.displayName = 'RequireAuth';

    const checkForAuth = (propsWithRouting: RouterProps) => {
      const authenticated = isLoggedIn();

      if (authenticated) {
        const emailVerified = hasVerifiedEmail();

        if (!emailVerified) {
          return <Redirect push={false} to={'/verify-email'} />;
        }
        const WithUser = withCurrentUser(opts)(Component);

        return <WithUser {...propsWithRouting} />;
      }

      const attemptedLocation = propsWithRouting.history.location.pathname;

      storePath(attemptedLocation);

      return <Redirect push={true} to={'/login'} />;
    };

    return requireAuthHelper(checkForAuth);
  };
}

requireAuth.displayName = 'RequireAuth';
