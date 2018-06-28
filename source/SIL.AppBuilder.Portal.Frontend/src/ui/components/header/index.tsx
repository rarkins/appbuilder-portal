import { connect } from 'react-redux';

import HeaderDisplay from './display';

import {
  toggleSidebar
} from '@store/user-interface';

const mapStateToProps = ({ ui }) => ({
  isSidebarVisible: ui.isSidebarVisible
});

const mapDispatchToProps = {
  toggleSidebar
}

export default connect(
  connect(mapStateToProps, mapDispatchToProps)
)(HeaderDisplay);