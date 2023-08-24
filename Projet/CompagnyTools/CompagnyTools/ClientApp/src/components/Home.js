import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Hello!</h1>
        <p>Welcome to your new single-page application for compagny:</p>
        <p>The first tools to your disposal is destined for the Flex-Office</p>
        <p>See the menu above to access it</p>
      </div>
    );
  }
}
