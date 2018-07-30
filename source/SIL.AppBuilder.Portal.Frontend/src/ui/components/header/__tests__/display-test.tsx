import * as React from 'react';
import { describe, beforeEach, it } from '@bigtest/mocha';
import { location, visit } from '@bigtest/react';
import { expect } from 'chai';

import {
  setupRequestInterceptor, useFakeAuthentication, setupApplicationTest
} from 'tests/helpers';


import { setToken, deleteToken, isLoggedIn } from '@lib/auth0';
import Header from '../display';

import headerHelper from 'tests/helpers/components/header';
import { TOGGLE_SIDEBAR } from '@store/user-interface/actions/toggle-sidebar';

describe('Integration | Component | Header', () => {
  setupApplicationTest();
  setupRequestInterceptor();
  useFakeAuthentication();

  beforeEach(function () {
    this.mockGet(200, '/organizations', { data: [{
      type: 'organizations',
      id: 1,
      attributes: {}
    }] });
  });

  describe('Dropdowns', () => {
    beforeEach(async () => {
      await visit('/');
    });

    describe('the dropdowns can open', () => {
      beforeEach(async () => {
        expect(headerHelper.isNotificationMenuOpen).to.be.false;

        await headerHelper.clickNotification();
      });

      it('opens the notification dropdown', () => {
        expect(headerHelper.isNotificationMenuOpen).to.be.true;
      });

      describe('only one dropdown is open at a time / dropdowns do not share the same state', () => {
        beforeEach(async () => {
          expect(headerHelper.isAvatarMenuOpen).to.be.false;
          expect(headerHelper.isNotificationMenuOpen).to.be.true;

          await headerHelper.clickAvatar();
        });

        it('closes the notification dropdown, and opens the avatar dropdown', () => {
          expect(headerHelper.isAvatarMenuOpen).to.be.true;
          expect(headerHelper.isNotificationMenuOpen).to.be.false;
        });
      });
    });
  });

  describe('Go to profile', () => {
    beforeEach(async () => {
      await visit('/');
      await headerHelper.clickProfileLink();
    });

    it('redirect to profile',() => {
      expect(location().pathname).to.eq('/profile');
    });
  });
});
