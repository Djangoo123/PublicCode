import React, { Component } from 'react';
import "../style/CreationMap.css";
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';

export class MapCreation extends Component {

    constructor(props) {
        super(props)

        this.state = {
            numberOfDesk: 0,
            numberOfLine: 0,
            radioButton: '',
            location: "",
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleOffice = this.handleOffice.bind(this);
        this.handleLine = this.handleLine.bind(this);
        this.handleLocation = this.handleLocation.bind(this);
        this.handleTypeDesk = this.handleTypeDesk.bind(this);

    }

    handleOffice(e) {
        this.setState({
            numberOfDesk: e.target.value
        });
    }

    handleLine(e) {
        this.setState({
            numberOfLine: e.target.value
        });
    }

    handleLocation(e) {
        this.setState({
            location: e.target.value
        });
    }

    handleTypeDesk(event) {
        this.setState({
            radioButton: event.target.value
        });
    }

    handleSubmit(event) {
        event.preventDefault();

        const { numberOfLine, numberOfDesk, location, radioButton } = this.state;

        let data = {};
        data.numberOffice = numberOfDesk;
        data.numberLine = numberOfLine;
        data.location = location;
        data.typeDesk = radioButton;

        console.log(data)
        let url = "/api/OfficeData/createAMap";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then(async response => {
                const isJson = response.headers.get('content-type')?.includes('application/json');
                const data = isJson && await response.json();

                if (!response.ok) {

                }

            })
            .catch(error => {
                console.error('There was an error!', error);
            });
    }

    render() {
        const { numberOfDesk, numberOfLine, location, radioButton } = this.state;

        return (
            <div className="mainContainer">
                <Typography variant="h5">Create a map with your parameters</Typography>
                <br />
                <form onSubmit={this.handleSubmit.bind(this)}>
                    <TextField
                        className="textfieldMainDesign"
                        type="number"
                        label="How many desk do you want per line ?"
                        variant="outlined"
                        value={numberOfDesk}
                        onChange={this.handleOffice}
                    />
                    <br />
                    <TextField
                        className="textfieldMainDesign"
                        type="number"
                        label="On how many lines ?"
                        variant="outlined"
                        value={numberOfLine}
                        onChange={this.handleLine}
                    />
                    <br />
                    <TextField
                        className="textfieldMainDesign"
                        type="text"
                        label="What is the name of this space ?"
                        variant="outlined"
                        value={location}
                        onChange={this.handleLocation}
                    />
                    <br />
                    <div className="radioContainer">
                        <FormControl>
                            <FormLabel id="demo-controlled-radio-buttons-group">Desk type</FormLabel>
                            <RadioGroup
                                aria-labelledby="demo-controlled-radio-buttons-group"
                                name="controlled-radio-buttons-group"
                                value={radioButton}
                                onChange={this.handleTypeDesk}
                            >
                                <FormControlLabel value="Simple" control={<Radio />} label="Simple desk" />
                                <FormControlLabel value="Laptop" control={<Radio />} label="Laptop" />
                                <FormControlLabel value="Full" control={<Radio />} label="Full equipment" />
                            </RadioGroup>
                        </FormControl>
                    </div>
                    <br />
                    <Button type="submit" variant="contained" color="primary">
                        save
                    </Button>
                </form>
            </div>
        );
    }
}
