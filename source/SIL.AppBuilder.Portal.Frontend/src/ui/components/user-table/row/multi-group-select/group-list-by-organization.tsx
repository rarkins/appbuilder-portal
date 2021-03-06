import * as React from 'react';

import { attributesFor } from '@data';

import ActiveGroupsDisplay from './active-groups-display';

export default ({ organizations, user, groups }) => {
  return organizations.map((organization, index) => {
    const organizationName = attributesFor(organization).name;
    const groupProps = {
      organization,
      user,
      groups,
    };

    return (
      <div data-test-groups-active key={index}>
        <span className='bold fs-11'>{organizationName}</span>
        <br />
        <ActiveGroupsDisplay {...groupProps} />
      </div>
    );
  });
};
