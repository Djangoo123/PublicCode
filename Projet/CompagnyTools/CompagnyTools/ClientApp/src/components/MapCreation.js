import {
    Typography,
    TextField,
    Button,
} from "@material-ui/core";
import React, { Component } from 'react';
import "../components/style/CreationMap.css";


export class MapCreation extends Component {

    render() {
        return (
            <div className="mainContainer">
                <Typography variant="h5">Create a map with your parameters</Typography>
                <br />
                <form>
                    <TextField
                        className="textfieldMainDesign"
                        type="text"
                        label="Number of desk verticaly"
                        variant="outlined"
                    />
                    <br />
                    <TextField
                        className="textfieldMainDesign"
                        type="text"
                        label="Number of desk horizontaly"
                        variant="outlined"
                    />
                    <br />
                    <TextField
                        className="textfieldMainDesign"
                        type="text"
                        label="Type of desk"
                        variant="outlined"
                    />
                    <br/>
                    <Button variant="contained" color="primary">
                        save
                    </Button>
                </form>
            </div>
        );
    }
}
