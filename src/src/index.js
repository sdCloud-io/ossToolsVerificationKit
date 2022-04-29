import reportExecutionData from './build/report.json'
import reportComparisonData from './build/comparisonReport.json'
import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import { MainComponent } from "./components/MainComponent";

ReactDOM.render(<MainComponent reportExecutionsCollection={ reportExecutionData }
                               reportComparisonCollection={ reportComparisonData }/>, document.getElementById('root'));