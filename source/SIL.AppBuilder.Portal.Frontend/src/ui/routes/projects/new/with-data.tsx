import * as React from 'react';
import { compose } from 'recompose';
import { withData as withOrbit, WithDataProps } from 'react-orbitjs';

import { create, idFromRecordIdentity,  ProjectResource } from '@data';
import { TYPE_NAME as ORGANIZATION } from '@data/models/organization';
import { TYPE_NAME as USER } from '@data/models/user';
import { TYPE_NAME as PROJECT, ProjectAttributes } from '@data/models/project';
import { TYPE_NAME as GROUP } from '@data/models/group';

import {
  IProvidedProps as WithCurrentUserProps
} from '@data/containers/with-current-user';

import {
  IProvidedProps as WithCurrentOrganizationProps
} from '@data/containers/with-current-organization';

import { recordIdentityFromKeys } from '@data/store-helpers';

export interface IProvidedProps {
  create: (attributes: ProjectAttributes, groupId: string, typeId: string) => ProjectResource;
}

type IProps =
  & WithCurrentUserProps
  & WithCurrentOrganizationProps
  & WithDataProps;

export function withData(WrappedComponent) {

  class DataWrapper extends React.Component<IProps> {

    create = async (attributes: ProjectAttributes, groupId: string, typeId: string) => {
      const { dataStore, currentOrganizationId, currentUser } = this.props;
      const currentUserId = idFromRecordIdentity(currentUser);
      const groupIdentity = recordIdentityFromKeys({ type: 'group', id: groupId });
      const applicationTypeIdentity = recordIdentityFromKeys({ type: 'applicationType', id: typeId});


      const project = await create(dataStore, PROJECT, {
        attributes,
        relationships: {
          owner: { id: currentUserId, type: USER },
          group: { id: groupIdentity.keys.remoteId, type: GROUP },
          organization: { id: currentOrganizationId, type: ORGANIZATION },
          type: { id: applicationTypeIdentity.keys.remoteId, type: 'applicationType'}
        }
      });

      return project;
    }

    render() {
      const props = {
        ...this.props,
        create: this.create,
      };

      return <WrappedComponent { ...props } />;
    }
  }

  return compose(
    withOrbit({})
  )(DataWrapper);
}
