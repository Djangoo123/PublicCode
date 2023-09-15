import React, { Component } from 'react'
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import '../../components/style/OfficeMapping.css';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import dayjs from 'dayjs';
import { DemoContainer, DemoItem } from '@mui/x-date-pickers/internals/demo';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateRangePicker } from '@mui/x-date-pickers-pro/DateRangePicker';
import { nullEmptyOrUndefined } from "../../components/Shared/Validation";
import '../style/Reservation.css'

export class ReservationComponent extends Component {

    constructor(props) {
        super(props)
        this.state = {
            dateValue: [],
            userName: "",
            showErrorMessage: false,
        };
        this.handleReservation = this.handleReservation.bind(this);
        this.handleUserName = this.handleUserName.bind(this);

    }

    handleUserName(event) {
        this.setState({ userName: event.target.value })
    }

    handleReservation() {
        const { userName, dateValue } = this.state;
        if (nullEmptyOrUndefined(userName) || nullEmptyOrUndefined(dateValue)) {
            this.setState({ showErrorMessage : true })
        }
        else {
            this.setState({ showErrorMessage: false })
            this.props.handleChange(dateValue, userName);
        }
    }

    handleClosePopUp = () => {
        this.props.onClose();
    };

    render() {

        const { open, dataReservation } = this.props;
        const { userName, showErrorMessage } = this.state;

        let listReservations;
        if ((!nullEmptyOrUndefined(dataReservation))){
            listReservations = dataReservation.map((item, i) =>
                <Typography id="modal-modal-description" sx={{ mt: 2 }}>
                    {"This desk is already reserved by " + item.username + " for the dates " + item.dateReservationStart + " to " + item.dateReservationEnd}
                </Typography>)
        }

        return (
            <div>
                <Modal
                    open={open}
                    onClose={this.handleClosePopUp}
                    aria-labelledby="modal-modal-title"
                    aria-describedby="modal-modal-description"
                >
                    <Box className="modalStyle">
                        <div className="bodyPopUp">
                            <Typography id="modal-modal-title" variant="h6" component="h2">
                                Reserve this emplacement
                            </Typography>
                            <br />
                            <Typography sx={{ mt: 2 }}>
                                Warning : Only dates of the current month are accepted
                            </Typography>
                            <br />
                            {listReservations}
                            <br />
                            <LocalizationProvider dateAdapter={AdapterDayjs}>
                                <DemoContainer components={['DateRangePicker', 'DateRangePicker']}>
                                    <DemoItem component="DateRangePicker">
                                        <DateRangePicker
                                            defaultValue={[dayjs(new Date()), dayjs(new Date())]}
                                            onChange={(newValue) => this.setState({ dateValue: newValue })}
                                            required
                                        />
                                    </DemoItem>
                                </DemoContainer>
                            </LocalizationProvider>
                            <br />
                            <TextField
                                className="username"
                                required
                                type="text"
                                label="Enter your user name"
                                variant="outlined"
                                value={!nullEmptyOrUndefined(dataReservation) ? dataReservation.userName : userName}
                                onChange={this.handleUserName}
                            />
                            <br />
                            <br />
                            <Button
                                onClick={this.handleReservation} variant="contained" type="Submit">
                                Reserve this location
                            </Button>
                            <br />
                            {showErrorMessage ? <Typography sx={{ mt: 2 }}>
                                Warning : One of the required field is not completed.
                            </Typography> : null}
                        </div>
                    </Box>
                </Modal>
            </div>
        )
    }
}
