import * as React from 'react';
import { Dropdown, Checkbox } from 'semantic-ui-react';

import { attributesFor } from '@data';

import { IProvidedProps as IDataProps } from '../group-select/with-data';

interface IOwnProps {
  selectedGroups: Id[];
  onChange: (groupIds: Id[]) => void;
  disableSelection?: boolean;
}

export interface IState {
  options: Array<{ id: string, value: string }>;
  selectedOptions: Array<{ id: string, value: string }>;
}

type IProps =
  & IOwnProps
  & IDataProps;

export default class GroupSelectDisplay extends React.Component<IProps, IState> {

  state = {
    options: [],
    selectedOptions: []
  }

  componentDidMount() {
    const { groups } = this.props;

    if (groups && groups.length > 0) {
      const dropdownOptions = this.groupToDropdownOptions(groups);
      this.setState({ options: dropdownOptions });
    }
  }

  componentDidUpdate(prevProps) {

    const newDropdownOptions = this.groupToDropdownOptions(this.props.groups);
    const prevDropdownOptions = this.groupToDropdownOptions(prevProps.groups);
    const newValues = newDropdownOptions.map(o => o.id).join('');
    const oldValues = prevDropdownOptions.map(o => o.id).join('');

    if(newValues != oldValues) {
      this.setState({ options: newDropdownOptions });
    }
  }

  groupToDropdownOptions = (groups) => {

    const groupOptions = groups
      .filter(group => attributesFor(group).name)
      .map(group => ({
        text: attributesFor(group).name,
        id: group.id
      }));

    return groupOptions;
  }

  onSelect = (e, { value }) => {
    e.preventDefault();

    const { onChange } = this.props;

    onChange(value);
  }

  getText = () => {

    const { selectedOptions, options } = this.state;

    if (selectedOptions && selectedOptions.length === 0) {
      return 'None';
    }

    if (selectedOptions.length === options.length) {
      return 'All groups';
    }

    const getShortName = (text) => text.split(' ').length > 1 ? `${text.split(' ')[0]}...` : text;

    return selectedOptions.map(item => getShortName(item.text)).join(', ');
  }

  updateOptionsSelected = (id) => {

    const { options, selectedOptions } = this.state;

    const optionSelected = options.find(opt => opt.id === id);

    const isOptionInSelectedOptions = (o) => selectedOptions.find(i => i.id === o.id) != undefined

    if (!optionSelected) { return; }

    //If option is in selectedOptions, remove it
    if (isOptionInSelectedOptions(optionSelected)) {

      const selectOptionsFiltered = selectedOptions.filter(i => i.id !== optionSelected.id);

      this.setState({
        selectedOptions: selectOptionsFiltered
      });

    } else {

      this.setState({
        selectedOptions: [
          ...selectedOptions,
          optionSelected
        ]
      });

    }
  }

  isItemSelected = (id) => {
    const { selectedOptions } = this.state;
    return selectedOptions.filter(i => i.id === id).length > 0;
  }

  render() {
    const { disableSelection } = this.props;
    const { options } = this.state;

    return (
      <>
        <Dropdown
          data-test-multi-group-select
          disabled={disableSelection || false}
          multiple
          text={this.getText()}
          className='w-100 groupDropdown'

        >
          <Dropdown.Menu className='groups'>
            {
              options.map((item, index) => (
                <div key={index} className="item" onClick={e => {
                  e.stopPropagation();
                  this.updateOptionsSelected(item.id);
                }}>
                  <Checkbox
                    value={item.id}
                    label={item.text}
                    checked={this.isItemSelected(item.id)}
                  />
                </div>
              ))
            }
          </Dropdown.Menu>
        </Dropdown>
      </>
    );

  }
}
