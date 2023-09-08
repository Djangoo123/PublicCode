import React, { Component } from 'react'
import OfficeMap from 'office-map'
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import '../components/style/OfficeMapping.css';
import { nullEmptyOrUndefined } from "../components/Shared/Validation";
import { ReservationComponent } from './office/ReservationComponent';

export class OfficeMapping extends Component {

    constructor(props) {
        super(props)
        this.state = {
            desk: [],
            dataMap: [],
            deskToDuplicate: [],
            spaceDesignation: "",
            selectedDesk: [],
            open: false,
            dateValue: [],
            userName: "",
            dataReservation: null,
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleDuplicate = this.handleDuplicate.bind(this);
        this.handleSeparator = this.handleSeparator.bind(this);
        this.handleDeleteItem = this.handleDeleteItem.bind(this);
        this.handleDesignation = this.handleDesignation.bind(this);
        this.handleDesk = this.handleDesk.bind(this);
        this.handleOpenPopUp = this.handleOpenPopUp.bind(this);
        this.handleClose = this.handleClose.bind(this);
        this.handleCreateReservation = this.handleCreateReservation.bind(this);
    }

    componentDidMount() {
        fetch('/api/OfficeData/getData')
            .then(response => response.json())
            .then(data => this.setState({ dataMap: data }));
    }

    componentDidUpdate() {
        const { dataMap, desk } = this.state;
        // Update our map if user decide to move some desk
        if (!nullEmptyOrUndefined(desk) && !nullEmptyOrUndefined(dataMap)) {
            let foundIndex = dataMap.findIndex(x => x.id === desk.id);
            dataMap[foundIndex] = desk;
        }
    }

    handleDesk(event) {
        this.setState({ desk: event, selectedDesk: event })
        if (!nullEmptyOrUndefined(event)) {
            let url = "/api/OfficeData/getReservationData";
            console.log(event)
            fetch(url,
                {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(event.id)
                })
                .then((data) => {
                    if (data != null) {
                        this.setState({ dataReservation: data })
                    }
                })
        }
    }

    handleOpenPopUp() {
        this.setState({ open: true });
    }

    handleClose = () => {
        this.setState({ open: false, selectedDesk: null });
    };

    handleCreateReservation = (dateValue, userName) => {
        const { selectedDesk } = this.state;
        console.log(dateValue)
        console.log(userName)

        selectedDesk.DateReservationStart = dateValue[0];
        selectedDesk.DateReservationEnd = dateValue[1];
        selectedDesk.UserName = userName;

        let url = "/api/OfficeData/ReserveLocation";
        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(selectedDesk)
            })
            .then((res) => res.json())
            .then((json) => {

            })
    };

    handleDuplicate(event) {

        const { desk, dataMap } = this.state;

        if (!nullEmptyOrUndefined(desk)) {
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

    handleDesignation(event) {
        this.setState({ spaceDesignation: event.target.value })
    }

    handleSeparator(event) {

        const { dataMap, spaceDesignation } = this.state;

        let url = "/api/OfficeData/createSeparator";

        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(spaceDesignation)
            })
            .then((res) => res.json())
            .then((json) => {
                dataMap.push(json);
                this.setState({ dataMap: dataMap })
                window.location.reload(false);
            })
    }

    handleDeleteItem(event) {

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

        const { dataMap, desk, spaceDesignation, open, dataReservation } = this.state;
        console.log(open)
        return (
            <div style={{ width: 1200, margin: "10px auto" }}>
                <h1>Your office</h1>
                <br />
                <div>
                    <Grid container >
                        <Grid item xs={4}>
                            <Button onClick={this.handleSubmit} variant="contained" type="Submit">Save my map</Button>
                        </Grid>
                        <Grid item xs={4}>
                            <Button onClick={this.handleDuplicate} variant="contained" type="Submit">Duplicate selected desk</Button>
                        </Grid>
                        <Grid item xs={4}>
                            <Button onClick={this.handleDeleteItem} variant="contained" type="Submit">Delete selected desk</Button>
                        </Grid>
                    </Grid>
                    <br />
                    <Grid container>
                        <Grid item xs={4}>
                            <TextField
                                className="designationTextfield"
                                type="text"
                                label="Office designation"
                                variant="outlined"
                                value={spaceDesignation}
                                onChange={this.handleDesignation}
                            />
                            <br />
                            <Button onClick={this.handleSeparator} variant="contained" type="Submit">Submit</Button>
                        </Grid>
                    </Grid>
                    <br />
                    <Grid container >
                        <Grid item xs={4}>
                            <Button onClick={this.handleOpenPopUp} variant="contained" type="Submit">Reserve this location</Button>
                        </Grid>
                    </Grid>
                </div>
                <br />
                {!nullEmptyOrUndefined(desk) ? null : <p>No desk selected</p>}
                <br />
                {(desk && desk.x >= 0 && desk.y >= 0) ?
                    (<h2>You have selected the desk {desk.id}</h2>) :
                    null}
                <hr />
                <br />
                {!nullEmptyOrUndefined(dataMap) ?
                    <OfficeMap
                        data={dataMap}
                        onSelect={desk => this.handleDesk(desk)}
                        onMove={desk => this.setState({ desk })}
                        editMode={true}
                        showNavigator={true}
                        horizontalSize={5}
                        verticalSize={3}
                        idSelected={2} /> : null}

                <ReservationComponent
                    open={open}
                    dataReservation={dataReservation}
                    desk={desk}
                    handleChange={this.handleCreateReservation}
                    onClose={this.handleClose} />
            </div>
        )
    }
}
