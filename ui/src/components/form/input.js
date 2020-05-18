import React from "react";
import { FormGroup, Label, Input as BSInput } from "reactstrap";
import classNames from "classnames";
import PropTypes from "prop-types";

const propTypes = {
    value: PropTypes.string.isRequired,
    label: PropTypes.string.isRequired,
    name: PropTypes.string,
    onChange: PropTypes.func.isRequired,
    type: PropTypes.string,
    placeholder: PropTypes.string,
};
const defaultProps = {
    type: "text",
    placeholder: null,
};

const Input = ({ value, label, type, onChange, name, placeholder }) => (
    <FormGroup className={classNames({ empty: value === "" })}>
        <Label className="small">{label}</Label>
        <BSInput
            type={type}
            name={name}
            onChange={onChange}
            placeholder={placeholder ?? label}
        />
    </FormGroup>
);

Input.propTypes = propTypes;
Input.defaultProps = defaultProps;

export default Input;
