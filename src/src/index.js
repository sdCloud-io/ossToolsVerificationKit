import data from './testReport.json'
import React from 'react';
import Panel from 'react-bootstrap/lib/Panel';
import Alert from 'react-bootstrap/lib/Alert'
import Button from 'react-bootstrap/lib/Button';
import ReactDOM from 'react-dom';
import './index.css';

function Success(props) {
    return <div className='fa fa-check'></div>;
}

function Fail(props) {
    return <div className='fa fa-times'></div>;
}


class LogPanel extends React.Component {
    constructor(props, context) {
        super(props, context);

        this.handleDismiss = this.handleDismiss.bind(this);
        this.handleShow = this.handleShow.bind(this);

        this.state = {
            show: true
        };
    }

    handleDismiss() {
        this.setState({ show: false });
    }

    handleShow() {
        this.setState({ show: true });
    }

    render() {
        if (this.state.show) {
            return (
                    <div className="row">
                        <div className="col-md-12 col-lg-12 col-xs-12">
                        <Alert bsStyle="info" onDismiss={this.handleDismiss}>
                <div className="row">
                            <div className="col-md-12 col-lg-12 col-xs-12">
                    {this.props.log}
                                </div>
                </div>
                <p>
                <Button onClick={this.handleDismiss}>Hide Log</Button>
                </p>
                </Alert>
                        </div>
                    </div>
   
    );
}

return <Button onClick={this.handleShow}>Show Log</Button>;
}
}

function RunResult(props) {
    const result = props.result;
    if (result === "success") {
        return <Success />;
    }
    return <Fail/>;
}

class Example extends React.Component {
    constructor(props, context) {
        super(props, context);
        this.toggleClass = this.toggleClass.bind(this);
        this.state = {
            open: false
        };
    }

    toggleClass() {
        const currentState = this.state.open;
        this.setState({ open: !currentState });
    };


    render() {
        return (
            <div>
                {this.props.resultsCollection.map((resultItem, i) =>
                    <Panel key={i} index={i} id="collapsible-panel-example-3" false>
            <Panel.Toggle onClick={this.toggleClass}>
                        <Panel.Heading>
                            <Panel.Title>
                                <div className="row">
                                    <div className="col-md-12 col-lg-12 col-xs-12">
                                        <div className="col-md-4 col-lg-4 col-xs-4">
                                            {resultItem.modelPath.replace(/^.*[\\\/]/, '')}
                                        </div>
                                        <div className="col-md-4 col-lg-4 col-xs-4">Pysd: <RunResult result={resultItem.result} /></div>
                                        <div className="col-md-4 col-lg-4 col-xs-4">SDe: <RunResult result={data.sdeResults[i].result} /></div>
                                    </div>
                                </div>
                            </Panel.Title>
    </Panel.Heading>
            </Panel.Toggle>
 
            <Panel.Collapse>
                            <Panel.Body>

                                <div className="row">
                                    <div className="col-md-12 col-lg-12 col-xs-12">
                                    <div className="col-md-4 col-lg-4 col-xs-4">Log</div>
                                        <div className="col-md-4 col-lg-4 col-xs-4">
                                            <LogPanel log={ resultItem.log }/> </div>
                                        <div className="col-md-4 col-lg-4 col-xs-4"><LogPanel log={data.sdeResults[i].log}/></div>
    </div>
    </div>
    <div className="row">
        <div className="col-md-12 col-lg-12 col-xs-12">
        <div className="col-md-4 col-lg-4 col-xs-4">
            Generation Time
        </div>
    <div className="col-md-4 col-lg-4 col-xs-4">{resultItem.codeGenerationTime}</div>
                                        <div className="col-md-4 col-lg-4 col-xs-4">{data.sdeResults[i].codeGenerationTime}</div>
    </div>
                                </div>
    <div className="row">
        <div className="col-md-12 col-lg-12 col-xs-12">
        <div className="col-md-4 col-lg-4 col-xs-4">
        Execution Time
    </div>
    <div className="col-md-4 col-lg-4 col-xs-4">{resultItem.codeExecutionTime}</div>
    <div className="col-md-4 col-lg-4 col-xs-4">{data.sdeResults[i].codeExecutionTime}</div>
    </div>
                                </div>
    <div className="row">
        <div className="col-md-12 col-lg-12 col-xs-12">
        <div className="col-md-4 col-lg-4 col-xs-4">
        Compilation Time
    </div>
    <div className="col-md-4 col-lg-4 col-xs-4">{resultItem.codeCompilationTime}</div>
    <div className="col-md-4 col-lg-4 col-xs-4">{data.sdeResults[i].codeCompilationTime}</div>
    </div>
    </div>
            </Panel.Body>
            </Panel.Collapse>
            </Panel>)}
            </div>)
    }
}


ReactDOM.render(<Example resultsCollection={data.pysdResults}/>, document.getElementById('root'));