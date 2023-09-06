import React, { Component } from 'react'
import OfficeMap from 'office-map'
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import '../components/style/OfficeMapping.css';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';

import dayjs from 'dayjs';
import { DemoContainer, DemoItem } from '@mui/x-date-pickers/internals/demo';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateRangePicker } from '@mui/x-date-pickers-pro/DateRangePicker';


export class OfficeMapping extends Component {

    constructor(props) {
        super(props)
        this.state = {
            desk: undefined,
            dataMap: null,
            deskToDuplicate: null,
            spaceDesignation: "",
            selectedDesk: null,
            open: false,
            dateValue: [],
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleDuplicate = this.handleDuplicate.bind(this);
        this.handleSeparator = this.handleSeparator.bind(this);
        this.handleDeleteItem = this.handleDeleteItem.bind(this);
        this.handleDesignation = this.handleDesignation.bind(this);
        this.handleDesk = this.handleDesk.bind(this);
        this.handleReservation = this.handleReservation.bind(this);
        this.handleClose = this.handleClose.bind(this);
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

    handleDesk(event) {
        this.setState({ desk: event })
        this.setState({ selectedDesk: event })
    }

    handleReservation() {
        const { selectedDesk } = this.state;
        if (selectedDesk != null) {
            this.setState({ open: true });
        }
    }

    handleClose = () => {
        this.setState({ open: false, selectedDesk : null  });
    };


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

    handleDesignation(event) {
        event.preventDefault();
        this.setState({ spaceDesignation: event.target.value })
    }

    handleSeparator(event) {
        event.preventDefault();

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

        const { dataMap, desk, spaceDesignation, open, dateValue } = this.state;

        return (
            <div style={{ width: 1200, margin: "10px auto" }}>
                <h1>Your office</h1>
                <br />
                <div id="signup">
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
                            <Button onClick={this.handleReservation} variant="contained" type="Submit">Reserve this location</Button>
                        </Grid>
                    </Grid>
                </div>
                <br />
                {desk != undefined ? null : <p>No desk selected</p>}
                <br />
                {(desk && desk.x >= 0 && desk.y >= 0) ?
                    (<h2>You have selected the desk {desk.id}</h2>) :
                    null}
                <hr />
                <br />
                {dataMap != undefined ?
                    <OfficeMap
                        data={dataMap}
                        onSelect={desk => this.handleDesk(desk)}
                        onMove={desk => this.setState({ desk })}
                        editMode={true}
                        showNavigator={true}
                        horizontalSize={5}
                        verticalSize={3}
                        idSelected={2} /> : null}
                <Modal
                    open={open}
                    onClose={this.handleClose}
                    aria-labelledby="modal-modal-title"
                    aria-describedby="modal-modal-description"
                >
                    <Box className="modalStyle">
                        <Typography id="modal-modal-title" variant="h6" component="h2">
                            Reserve this emplacement
                        </Typography>
                        <br />
                        <Typography id="modal-modal-description" sx={{ mt: 2 }}>
                            You will reserve the desk number {(desk && desk.x >= 0 && desk.y >= 0) ? desk.id : null}
                        </Typography>
                        <br />
                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                            <DemoContainer components={['DateRangePicker', 'DateRangePicker']}>
                                <DemoItem label="Uncontrolled picker" component="DateRangePicker">
                                    <DateRangePicker
                                        defaultValue={[dayjs(new Date()), dayjs(new Date())]}
                                        onChange={(newValue) => this.setState({ dateValue: newValue })}
                                    />
                                </DemoItem>
                            </DemoContainer>
                        </LocalizationProvider>
                    </Box>
                </Modal>
            </div>
        )
    }
}
