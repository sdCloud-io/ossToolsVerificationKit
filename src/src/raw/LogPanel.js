import React, { useState } from "react";
import { Button } from "react-bootstrap";

export function LogPanel(props) {
    const styles = {
        textAlign: 'left'
    }

    const [show, setShow] = useState(true);

    function hidePanel() {
        setShow(false)
    }

    function showPanel() {
        setShow(true)
    }

    if (show) {
        return (
            <div className="card border-info">
                <div className="card-body">

                    <pre style={styles} className="log">
                        { props.log }
                    </pre>

                    <Button onClick={ hidePanel } variant="outline-dark">Hide Log</Button>
                </div>
            </div>
        )
    } else {
        return <Button onClick={ showPanel } variant="outline-dark">Show Log</Button>
    }
}