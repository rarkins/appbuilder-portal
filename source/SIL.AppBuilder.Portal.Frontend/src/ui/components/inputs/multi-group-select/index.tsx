import { compose } from 'recompose';

import { withCurrentUser } from '@data/containers/with-current-user';
import { withData } from '../group-select/with-data';
import Display from './display';

export default compose(
  withCurrentUser(),
  withData
)(Display);
