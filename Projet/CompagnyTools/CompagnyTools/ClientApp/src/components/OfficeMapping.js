import React, { Component } from 'react'
import OfficeMap from 'office-map'
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';

export class OfficeMapping extends Component {

    constructor(props) {
        super(props)
        this.state = {
            desk: undefined,
            dataMap: null,
            deskToDuplicate: null,
            spaceDesignation: "",
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleDuplicate = this.handleDuplicate.bind(this);
        this.handleSeparator = this.handleSeparator.bind(this);
        this.handleDeleteItem = this.handleDeleteItem.bind(this);
    }

    componentDidMount() {
        fetch('/api/OfficeData/getData')
            .then(response => response.json())
            .then(data => this.setState({ dataMap: data }));
    }

    componentDidUpdate() {
        const { dataMap, desk } = this.state;

        // Update our map if user decide to move some desk
        if (desk != undefined && dataMap != undefined) {
            let foundIndex = dataMap.findIndex(x => x.id === desk.id);
            dataMap[foundIndex] = desk;
        }
    }

    handleDuplicate(event) {

        event.preventDefault();
        const { desk, dataMap } = this.state;

        if (desk != undefined) {
            this.setState({ deskToDuplicate: desk })
        }

        let url = "/api/OfficeData/duplicateDesk";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(desk)
            })
            .then((res) => res.json())
            .then((json) => {
                dataMap.push(json);
                this.setState({ dataMap: dataMap })
            })
    }

    handleSeparator(event) {
        event.preventDefault();

        const { dataMap } = this.state;

        let url = "/api/OfficeData/createSeparator";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(event.target.value)
            })
            .then((res) => res.json())
            .then((json) => {
                dataMap.push(json);
                this.setState({ dataMap: dataMap })
            })
    }

    handleDeleteItem(event) {
        event.preventDefault();

        const { desk, dataMap } = this.state;
        let foundItem = dataMap.find(x => x.id === desk.id);
        let url = "/api/OfficeData/deleteDesk";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(foundItem)
            })
            .then((res) => res.json())
            .then((id) => {
                window.location.reload(false);
            })
    }

    handleSubmit(event) {
        event.preventDefault();
        const { dataMap } = this.state;

        let url = "/api/OfficeData/updateOfficeMap";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(dataMap)
            })
            .then(
                response => {

                })
    }

    render() {

        const { dataMap, desk, spaceDesignation } = this.state;

        return (
            <div style={{ width: 1200, margin: "10px auto" }}>
                <h1>Your office</h1>
                <br />
                <div id="signup">
                    <Grid container >
                        <Grid item xs={4}>
                            <form onSubmit={this.handleSubmit}>
                                <Button variant="contained" type="Submit">Save my map</Button>
                            </form>
                        </Grid>
                        <Grid item xs={4}>
                            <form onSubmit={this.handleDuplicate}>
                                <Button variant="contained" type="Submit">Duplicate selected desk</Button>
                            </form>
                        </Grid>
                        <Grid item xs={4}>
                            <form onSubmit={this.handleDeleteItem}>
                                <Button onChange={this.handleDeleteItem} variant="contained" type="Submit">Delete selected desk</Button>
                            </form>
                        </Grid>
                    </Grid>
                    <br />
                    <Grid container>
                        <Grid item xs={4}>
                            <TextField
                                className=""
                                type="text"
                                label="Office espace designation"
                                variant="outlined"
                                value={spaceDesignation}
                                onChange={this.handleSeparator}
                            />
                        </Grid>
                    </Grid>
                </div>
                <br />
                {desk != undefined ? null : <p>No desk selected</p>}
                <br />
                {(desk && desk.x >= 0 && desk.y >= 0) ?
                    (<h2>The desk {desk.id} moved to: {desk.x}, {desk.y}</h2>) :
                    (desk && desk.id ? <h2>The desk {desk.id} was selected</h2> : '')}
                <hr />
                <br />
                {dataMap != undefined ?
                    <OfficeMap
                        data={dataMap}
                        onSelect={desk => this.setState({ desk })}
                        onMove={desk => this.setState({ desk })}
                        editMode={true}
                        showNavigator={true}
                        horizontalSize={5}
                        verticalSize={3}
                        idSelected={2} /> : null}
            </div>



        )
    }
}
