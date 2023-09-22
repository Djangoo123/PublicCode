import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './style/NavMenu.css';
import { nullOrUndefined } from "../components/Shared/Validation";


export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
            hideItemIfLog: false,
            hideItemAdmin: false,
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }



    componentDidMount() {

        let login = JSON.parse(window.localStorage.getItem('login'));

        if (!nullOrUndefined(login)) {
            this.setState({
                hideItemIfLog: true
            });

            const checkIfAdmin = login.usersRoles.find(x => x.userRight.toLowerCase() === "admin".toLowerCase());
            if (!nullOrUndefined(checkIfAdmin)) {
                this.setState({
                    hideItemAdmin: true
                });
            }
        }
    }

    render() {
        const { hideItemIfLog, hideItemAdmin } = this.state;

        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
                    <NavbarBrand tag={Link} to="/">CompagnyTools</NavbarBrand>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/Office">Office</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/Creation">Create default map</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/Login">{!hideItemIfLog ? "Login" : "Logout"}</NavLink>
                                </NavItem>
                            {hideItemAdmin ?
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/UserAdministration">Manage Users</NavLink>
                                </NavItem> : null}
                        </ul>
                    </Collapse>
                </Navbar>
            </header>
        );
    }
}
