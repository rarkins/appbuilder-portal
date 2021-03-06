import { translate } from 'react-i18next';

export { TransProps as i18nProps } from 'react-i18next';

const defaultNamespace = 'translations';
export const withTranslations = translate(defaultNamespace);
