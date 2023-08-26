import React, { Component } from 'react'
import OfficeMap from 'office-map'

export class OfficeMapping extends Component {

    constructor(props) {
        super(props)
        this.state = {
            desk: undefined,
            dataMap: null,
        };

        this.handleSubmit = this.handleSubmit.bind(this);

    }

    componentDidMount() {
        fetch('/api/officeData/getData')
            .then(response => response.json())
            .then(data => this.setState({ dataMap: data }));
    }

    componentDidUpdate() {
        const { dataMap, desk } = this.state;

        // Update our map if user decide to move some desk
        if (desk != undefined && dataMap != undefined)
        {
            let foundIndex = dataMap.findIndex(x => x.id === desk.id);
            dataMap[foundIndex] = desk;
        }
    }


    handleSubmit(event) {
        event.preventDefault();
        const { dataMap } = this.state;

        let url = "/api/officeData/updateOfficeMap";

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

        const { dataMap, desk } = this.state;

        return (
            <div style={{ width: 1200, margin: "10px auto" }}>
                <h1>OfficeMap Example</h1>
                <div id="signup">
                    <form onSubmit={this.handleSubmit}>
                        <button type="Submit">Save my map</button>
                    </form>

                </div>
                {
                    (desk && desk.x >= 0 && desk.y >= 0) ?
                        (<h2>The desk {desk.id} moved to: {desk.x}, {desk.y}</h2>) :
                        (desk && desk.id ? <h2>The desk {desk.id} was selected</h2> : '')}
                <hr />
                <br />
                {dataMap != undefined && dataMap[0] != null ?
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
