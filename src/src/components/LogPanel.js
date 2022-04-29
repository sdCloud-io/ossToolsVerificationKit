import Alert from "react-bootstrap/lib/Alert";
import Button from "react-bootstrap/lib/Button";
import React, { useState } from "react";

export function LogPanel(props) {
    const [show, setShow] = useState(true);

    function hidePanel() {
        setShow(false)
    }

    function showPanel() {
        setShow(true)
    }

    if (show) {
        return (<div className="row">
            <div className="col-md-12 col-lg-12 col-xs-12">
                <Alert bsStyle="info">
                    <div className="row">
                        <div className="col-md-12 col-lg-12 col-xs-12">
                            { props.log }
                        </div>
                    </div>
                    <p>
                        <Button onClick={ hidePanel }>Hide Log</Button>
                    </p>
                </Alert>
            </div>
        </div>)
    } else {
        return <Button onClick={ showPanel }>Show Log</Button>
    }
}