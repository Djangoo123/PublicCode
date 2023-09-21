import React, { Component } from 'react'
import OfficeMap from 'office-map'
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import '../style/OfficeMapping.css';
import { nullEmptyOrUndefined } from "../Shared/Validation";
import { nullOrUndefined } from "../Shared/Validation";
import { ReservationComponent } from './ReservationComponent';
import dayjs from 'dayjs';
import { DemoContainer } from '@mui/x-date-pickers/internals/demo';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';


export class OfficeMapping extends Component {

    constructor(props) {
        super(props)
        this.state = {
            desk: [],
            dataMap: [],
            deskToDuplicate: [],
            open: false,
            dateValue: [],
            userName: "",
            dataReservation: null,
            showAdminElement: false,
            isAdmin : false,
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleDuplicate = this.handleDuplicate.bind(this);
        this.handleDeleteItem = this.handleDeleteItem.bind(this);
        this.handleDesk = this.handleDesk.bind(this);
        this.handleOpenPopUp = this.handleOpenPopUp.bind(this);
        this.handleClose = this.handleClose.bind(this);
        this.handleCreateReservation = this.handleCreateReservation.bind(this);
    }

    componentDidMount() {
        fetch('/api/OfficeData/getData')
            .then(response => response.json())
            .then(data => this.setState({ dataMap: data, desk: null }));

        let login = JSON.parse(window.localStorage.getItem('login'));

        if (!nullOrUndefined(login)) {

            const checkIfAdmin = login.usersRoles.find(x => x.userRight.toLowerCase() === "admin".toLowerCase());
            if (!nullOrUndefined(checkIfAdmin)) {
                this.setState({ isAdmin: true })
            }
        }
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

        this.setState({ desk: event })

        if (!nullEmptyOrUndefined(event)) {

            let url = "/api/OfficeData/getReservationData";

            fetch(url,
                {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(event.id)
                })
                .then(async response => {
                    const isJson = response.headers.get('content-type')?.includes('application/json');
                    const data = isJson && await response.json();

                    if (!response.ok) {
                        this.setState({ dataReservation: null })
                    }

                    this.setState({ dataReservation: data })
                })
                .catch(error => {
                    console.error('There was an error!', error);
                });

        }
    }

    handleOpenPopUp() {
        this.setState({ open: true });
    }

    handleClose = () => {
        this.setState({ open: false, desk: null });
    };

    handleCreateReservation = (dateValue, userName) => {
        const { desk } = this.state;

        desk.dateReservationStart = dateValue[0];
        desk.dateReservationEnd = dateValue[1];
        desk.UserName = userName;

        let url = "/api/OfficeData/ReserveLocation";
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
            .then((data) => {

            })
    };

    handleDuplicate() {

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

    handleDeleteItem() {

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

    handleSubmit() {
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

        const { dataMap, desk, open, dataReservation, showAdminElement, isAdmin } = this.state;

        let now = dayjs();
        let dateWeek = now.add('30', 'day');

        return (
            <div style={{ width: "auto", margin: "10px auto" }}>
                <h1>Your office</h1>
                <br />
                {isAdmin ?
                    <Button variant="contained" onClick={() => { this.setState({ showAdminElement: !showAdminElement }) }}>{showAdminElement ? false : true} Admin panel</Button> : null}
                {showAdminElement && isAdmin ?   
                        <><br /><br /><Grid container>
                        <Grid item xs={4}>
                            <Button onClick={this.handleSubmit} variant="contained" type="Submit">Save my map</Button>
                        </Grid>
                        <Grid item xs={4}>
                            <Button onClick={this.handleDuplicate} variant="contained" type="Submit">Duplicate selected desk</Button>
                        </Grid>
                        <Grid item xs={4}>
                            <Button onClick={this.handleDeleteItem} variant="contained" type="Submit">Delete selected desk</Button>
                        </Grid>
                    </Grid><br /></>
                    : null}
                <div className="datepickerContainer">
                    <p>Reservations for this month</p>
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <DemoContainer components={['DatePicker', 'DatePicker']}>
                            <DatePicker
                                disabled={true}
                                defaultValue={dayjs(new Date())}
                            />
                            <DatePicker
                                disabled={true}
                                defaultValue={dateWeek}
                            />
                        </DemoContainer>
                    </LocalizationProvider>
                    <br />
                    <Grid container>
                        <Grid item xs={4}>
                            <Button disabled={nullEmptyOrUndefined(desk) ? true : false}  onClick={this.handleOpenPopUp} variant="contained" type="Submit">Reserve this location</Button>
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
                        idSelected={2} />
                    : null}

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
