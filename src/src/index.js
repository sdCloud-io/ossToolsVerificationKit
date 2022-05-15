import React from 'react';
import ReactDOM from 'react-dom';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-bootstrap-range-slider/dist/react-bootstrap-range-slider.css';
import './index.css';
import { files } from './reports/linker'
import { AppComponent } from "./AppComponent";

ReactDOM.render(<AppComponent
    reports={ files }/>, document.getElementById('root'));