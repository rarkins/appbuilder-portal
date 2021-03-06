import * as React from 'react';
import { compose, withProps } from 'recompose';
import { Dropdown } from 'semantic-ui-react';

import {
  attributesFor,
  GroupResource,
  UserResource,
  relationshipFor,
  idFor,
  recordsWithIdIn,
} from '@data';

import { isEmpty } from '@lib/collection';

import { OrganizationResource } from '@data';

import { withRelationships } from '@data/containers/with-relationship';
import { withTranslations, i18nProps } from '@lib/i18n';

import GroupSelect from './group-select';
import GroupListByOrganization from './group-list-by-organization';

interface INeededProps {
  user: UserResource;
  organizations: OrganizationResource[];
}

interface IOwnProps {
  groups: GroupResource[];
  currentUser: UserResource;
}

type IProps = INeededProps & i18nProps & IOwnProps;

class MultiGroupSelect extends React.Component<IProps> {
  render() {
    const { organizations, user, groups } = this.props;

    const groupList = (
      <GroupListByOrganization groups={groups} user={user} organizations={organizations} />
    );

    return (
      <>
        <Dropdown
          data-test-group-multi-select
          multiple
          trigger={groupList}
          className='w-100 multiDropdown'
        >
          <Dropdown.Menu className='groups' data-test-group-menu>
            {organizations.map((organization, index) => {
              const { name } = attributesFor(organization);

              const groupCheckboxesProps = {
                organization,
                user,
              };

              return (
                <React.Fragment key={index}>
                  <Dropdown.Header data-test-group-multi-organization-name content={name} />
                  <GroupSelect {...groupCheckboxesProps} />
                </React.Fragment>
              );
            })}
          </Dropdown.Menu>
        </Dropdown>
      </>
    );
  }
}

export default compose<IProps, INeededProps>(
  withTranslations,
  withRelationships(({ user }) => {
    return {
      allUserGroups: [user, 'groupMemberships', 'group'],
    };
  }),
  // Filter groups that are visible for the current user
  withProps(({ allUserGroups, organizations }) => {
    let groups = [];

    if (allUserGroups) {
      groups = allUserGroups.filter((group) => {
        const orgId = idFor(relationshipFor(group, 'owner'));

        return recordsWithIdIn(organizations, orgId).length > 0;
      });
    }

    return { groups };
  })
)(MultiGroupSelect);
