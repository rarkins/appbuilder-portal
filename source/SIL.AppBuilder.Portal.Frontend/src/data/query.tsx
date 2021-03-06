import * as React from 'react';
import { withData as withOrbit, WithDataProps } from 'react-orbitjs';

import { defaultSourceOptions } from '@data';

import { ErrorMessage } from '@ui/components/errors';
import { isEmpty } from '@lib/collection';
import { timeoutablePromise } from '@lib/promises';
import { areCollectionsRoughlyEqual } from '@lib/collection';

interface IState {
  result: object;
  error: any;
  isLoading: boolean;
}

export interface IQueryOptions {
  passthroughError?: boolean;
  useRemoteDirectly?: boolean;
  mapResultsFn?: (props, result) => Promise<any>;
}

// Example Usage
//
// import { query } from '@data';
//
// const mapRecordsToProps = (passedProps) => {
//
//   return {
//     someKey: q => q.findRecord(...),
//     someOtherKey: [q => q.findRecord, { /* source options */ }]
//   }
// }
//
// // ......
//
// export default compose(
//    query(mapRecordsToProps)
// )(SomeComponent);
//
//
// Why does this exist?
// the react-orbitjs addon actually doesn't give is much.
// it has a "lot" of cache-related handling, but no ergonomic
// way to actually make network requests.
//
// TODO: tie in to react-orbitjs' cache handling.
// TODO: what if we just use orbit directly? do we need react-orbitjs?
export function queryApi<T>(mapRecordsToProps, options?: IQueryOptions) {
  let map;
  const opts = options || { passthroughError: false, useRemoteDirectly: false, mapResultsFn: null };
  const { passthroughError, useRemoteDirectly, mapResultsFn } = opts;

  if (typeof mapRecordsToProps !== 'function') {
    map = (/* props */) => ({
      cacheKey: 'default-cache-key',
      ...mapRecordsToProps,
    });
  } else {
    map = mapRecordsToProps;
  }

  return (InnerComponent) => {
    class DataWrapper extends React.Component<T & WithDataProps, IState> {
      state = { result: {}, error: undefined, isLoading: false };
      // tslint:disable-next-line:variable-name
      _isMounted: boolean = false;
      mapResult: any = {};

      componentDidMount() {
        this._isMounted = true;
        this.tryFetch();
      }

      componentDidUpdate() {
        this.tryFetch();
      }
      componentWillUnmount() {
        this._isMounted = false;
      }
      setState(state, callback?) {
        if (this._isMounted) {
          super.setState(state, callback);
        }
      }
      fetchData = async () => {
        const result = map(this.props);

        const {
          dataStore,
          sources: { remote },
        } = this.props;
        const querier = useRemoteDirectly ? remote : dataStore;

        const responses = {};
        const resultingKeys = Object.keys(result).filter((k) => k !== 'cacheKey');
        const requestPromises = resultingKeys.map(async (key: string) => {
          const query = result[key];
          const args = typeof query === 'function' ? [query] : query;

          try {
            const queryResult = await querier.query(...args);
            responses[key] = queryResult;

            return Promise.resolve(queryResult);
          } catch (e) {
            if (querier === remote) {
              querier.requestQueue.skip();
            }

            return Promise.reject(e);
          }
        });

        if (requestPromises.length > 0) {
          await timeoutablePromise(5000, Promise.all(requestPromises));
        }

        return responses;
      };

      tryFetch = async (force: boolean = false) => {
        if (!this.isFetchNeeded() && !force) {
          return;
        }

        this.setState({ isLoading: true }, async () => {
          try {
            let result = await this.fetchData();
            if (mapResultsFn) {
              result = await mapResultsFn(this.props, result);
            }
            this.setState({ result, isLoading: false });
          } catch (e) {
            this.setState({ error: e, isLoading: false });
          }
        });
      };

      isFetchNeeded = () => {
        const result = map(this.props);

        if (areCollectionsRoughlyEqual(result, this.mapResult)) {
          return false;
        }

        this.mapResult = result;

        return true;
      };

      render() {
        const { result, error, isLoading } = this.state;
        const dataProps = {
          ...result,
          error,
          isLoading,
          refetch: this.tryFetch.bind(this, true),
        };

        if (!passthroughError && error) {
          return <ErrorMessage error={error} />;
        }

        return <InnerComponent {...this.props} {...dataProps} />;
      }
    }

    return withOrbit({})(DataWrapper);
  };
}
