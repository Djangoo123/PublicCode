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

        //const dataTest = [
        //    {
        //        id: 1,
        //        chairDirection: 'south', x: 0, y: 0,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'laptop', specification: 'Laptop Dell Inspiron 15 5000' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 2,
        //        chairDirection: 'south', x: 1, y: 0,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'cpu', specification: 'Dual core 2.4 GHz, 16 GB RAM, 256 GB HD' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'keyboard', specification: 'HP Ultrathin Wireless Keyboard' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'mouse', specification: 'HP USB 2 Button Optical Mouse' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 3, chairDirection: 'south', x: 2, y: 0,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'cpu', specification: 'Dual core 2.4 GHz, 16 GB RAM, 256 GB HD' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'keyboard', specification: 'HP Ultrathin Wireless Keyboard' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'mouse', specification: 'HP USB 2 Button Optical Mouse' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 4, chairDirection: 'south', x: 3, y: 0,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'laptop', specification: 'Laptop Dell Inspiron 15 5000' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 5, chairDirection: 'west', x: 0, y: 1,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'laptop', specification: 'Laptop Dell Inspiron 15 5000' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 6, chairDirection: 'east', x: 1, y: 1,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ],
        //    },
        //    {
        //        id: 7, chairDirection: 'north-west', x: 2, y: 1,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'cpu', specification: 'Dual core 2.4 GHz, 8 GB RAM, 512 GB HD' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'keyboard', specification: 'HP Ultrathin Wireless Keyboard' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ]
        //    },
        //    {
        //        id: 8, chairDirection: 'north-east', x: 3, y: 1,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'laptop', specification: 'Laptop Dell Inspiron 15 5000' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //        ]
        //    },
        //    {
        //        id: 9, chairDirection: 'west', x: 0, y: 2,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ]
        //    },
        //    {
        //        id: 10, chairDirection: 'east', x: 1, y: 2,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'desktop', specification: 'HP CPU, keyboard and mouse' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ]
        //    },
        //    {
        //        id: 11, chairDirection: 'south-west', x: 2, y: 2,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'cpu', specification: 'Dual core 2.4 GHz, 16 GB RAM, 256 GB HD' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'keyboard', specification: 'HP Ultrathin Wireless Keyboard' },
        //            { type: 'phone', specification: 'Cisco Phone IP 7960G/7940G' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'mouse', specification: 'HP USB 2 Button Optical Mouse' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ]
        //    },
        //    {
        //        id: 12, chairDirection: 'south-east', x: 3, y: 2,
        //        equipments: [
        //            { type: 'desk', specification: 'Simple desk' },
        //            { type: 'cpu', specification: 'Dual core 2.4 GHz, 16 GB RAM, 256 GB HD' },
        //            { type: 'monitor', specification: 'HP V197 18.5-inch' },
        //            { type: 'keyboard', specification: 'HP Ultrathin Wireless Keyboard' },
        //            { type: 'chair', specification: '817L Kare Ergonomic Office Chair' },
        //            { type: 'mouse', specification: 'HP USB 2 Button Optical Mouse' },
        //            { type: 'drawer', specification: 'Simple drawer' },
        //        ]
        //    }
        //]


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
