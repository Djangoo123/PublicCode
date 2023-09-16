import React, { Component } from 'react';
import "../style/CreationMap.css";
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import FormControlLabel from '@mui/material/FormControlLabel';


export class MapCreation extends Component {

    constructor(props) {
        super(props)

        this.state = {
            textFieldX: 0,
            textFieldY: 0,
            radioButton: "",
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleTextFieldXChange = this.handleTextFieldXChange.bind(this);
        this.handleTextFieldYChange = this.handleTextFieldYChange.bind(this);

    }

    handleTextFieldXChange(e) {
        this.setState({
            textFieldX: e.target.value
        });
    }

    handleTextFieldYChange(e) {
        this.setState({
            textFieldY: e.target.value
        });
    }

    handleSubmit(event) {
        event.preventDefault();
    }

    render() {
        const { textFieldX, textFieldY } = this.state;

        return (
            <div className="mainContainer">
                <Typography variant="h5">Create a map with your parameters</Typography>
                <br />
                <form>
                    <TextField
                        className="textfieldMainDesign"
                        type="number"
                        label="Number of desk verticaly"
                        variant="outlined"
                        value={textFieldX}
                        onChange={this.handleTextFieldXChange}
                    />
                    <br />
                    <TextField
                        className="textfieldMainDesign"
                        type="number"
                        label="Number of desk horizontaly"
                        variant="outlined"
                        value={textFieldY}
                        onChange={this.handleTextFieldYChange}
                    />
                    <br />
                    <div className="radioContainer">
                        <RadioGroup
                            aria-labelledby="demo-radio-buttons-group-label"
                            defaultValue="female"
                            name="radio-buttons-group"
                        >
                            <FormControlLabel value="Simple" control={<Radio />} label="Simple desk" />
                            <FormControlLabel value="Laptop" control={<Radio />} label="Laptop" />
                            <FormControlLabel value="Full" control={<Radio />} label="Full equipment" />
                        </RadioGroup>
                    </div>

                    <br />
                    <Button variant="contained" color="primary">
                        save
                    </Button>
                </form>
            </div>
        );
    }
}
