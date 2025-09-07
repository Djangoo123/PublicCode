
export const validatePhoneNumber = (internationalNumber, value) => {

    if (!internationalNumber) {
        if (value !== "" && value !== null && !/^0[1-9]{1}(([0-9]{2}){4})|((\s[0-9]{2}){4})|((-[0-9]{2}){4})$/i.test(value)) { //Check for french format
            return false;
        }
    }
    else {
        if (
            value !== "" && value !== null &&
            !/(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\s*[)]?[-\s.]?[(]?[0-9]{1,3}[)]?([-\s.]?[0-9]{3})([-\s.]?[0-9]{3,4})/i.test(value)
        ) {       //Check for international format
            if (!/^0[1-9]{1}(([0-9]{2}){4})|((\s[0-9]{2}){4})|((-[0-9]{2}){4})$/i.test(value)) {
                return false;
            }
        }
    }

    return true;
}

export const validateZipCode = (value) => {
    if (!nullEmptyOrUndefined(value) &&
        !/^(([0-8][0-9])|(9[0-5]))[0-9]{3}$/i.test(value)
    ) {
        return 'Postal code invalid';
    }
}

export const validateEmail = (value) => {
    if (!nullEmptyOrUndefined(value) &&
        !value.match("^(?=.{1,64}@)[A-Za-z0-9_-]+(\\.[A-Za-z0-9_-]+)*@+[^-][A-Za-z0-9-]+(\\.[A-Za-z0-9-]+)*(\\.[A-Za-z]{2,})$")
    ) {
        return false;
    }
    else {
        return true;
    }
}

export const nullOrEmpty = (value) => (value === null || value === '');

export const nullOrUndefined = (value) => (value === null || value === undefined);

export const nullEmptyOrUndefined = (value) => (value === null || value === undefined || value === '');

export const isNumeric = (value) => {
    return value === null || value === '' || (!isNaN(value) && value.toString().trim() !== '' && value.toString().indexOf(' ') === -1);
}

export const isDate = (value) => !nullEmptyOrUndefined(value) && (value instanceof Date) && (!isNaN(value.getTime()));

export const validatePositiveInteger = (value) => (new RegExp('^[0-9]+$')).test(value);

export const validateInteger = (value) => (new RegExp('^-?[0-9]+$')).test(value);