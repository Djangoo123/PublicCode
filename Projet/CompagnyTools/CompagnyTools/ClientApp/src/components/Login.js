import React, { Component } from 'react'
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import { nullOrUndefined } from "../components/Shared/Validation";

export class Login extends Component {

    constructor(props) {
        super(props)
        this.state = {
            userName: "",
            password: "",
            hideItemIfLog: false,
        };
    }

    componentDidMount() {

        let login = JSON.parse(window.localStorage.getItem('login'));

        if (!nullOrUndefined(login)) {
            this.setState({
                hideItemIfLog: true
            });
        }
    }

    handleChangeUser = name => event => {
        this.setState({
            [name]: event.target.value
        });
    };

    handleChangePassword = name => event => {
        this.setState({
            [name]: event.target.value
        });
    };

    handleSubmit = event => {
        event.preventDefault();
        const { userName, password } = this.state;

        let data = {};
        data.Username = userName;
        data.Password = password;

        let url = "/api/Login/userLogin";
        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then((res) => res.json())
            .then((data) => {
                let login = JSON.parse(window.localStorage.getItem('login'));
                if (nullOrUndefined(login)) {
                    window.localStorage.setItem('login', JSON.stringify(data));
                }
                window.location.href = "/Office";
            })
    };

    handleLogOut = event => {
        event.preventDefault();

        let url = "/api/Login/logout";
        fetch(url,
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
            })
            .then((data) => {
                console.log(data)
                localStorage.clear();
                window.location.href = "/Home";
            })
    };

    render() {
        const { userName, password, hideItemIfLog } = this.state;

        return (
            <div style={{ width: "auto" }}>
                {!hideItemIfLog ?
                    <form
                        onSubmit={this.handleSubmit.bind(this)}
                        autoComplete="off"
                    >
                        <TextField
                            label="Name"
                            required
                            value={userName}
                            onChange={this.handleChangeUser("userName")}
                            margin="normal"
                        />
                        <br />
                        <TextField
                            label="password"
                            type={"password"}
                            required
                            value={password}
                            onChange={this.handleChangePassword("password")}
                            margin="normal"
                        />
                        <br />
                        <Button type="submit" variant="outlined">
                            Validate
                        </Button>
                    </form>
                    :
                    <form
                        onSubmit={this.handleLogOut.bind(this)}
                        autoComplete="off"
                    >
                        <Button type="submit" variant="outlined">
                            Log out
                        </Button>
                    </form>}
            </div>
        )
    }
}
