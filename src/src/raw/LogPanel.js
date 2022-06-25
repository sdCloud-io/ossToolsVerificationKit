import React, { useState } from "react";
import { Button, Modal } from "react-bootstrap";

export function LogPanel(props) {
    const styles = {
        textAlign: 'left'
    }

    const [show, setShow] = useState(false);

    function hideLog() {
        setShow(false)
    }

    function showPanel() {
        setShow(true)
    }

    if (show) {
        return (
            <Modal show={ show } onHide={ hideLog } { ...props }
                   dialogClassName="modal-90w">
                <Modal.Header closeButton>
                    <Modal.Title>Log</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <pre style={ styles } className="log">
                        { props.log }
                    </pre>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={ hideLog }>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        )
    } else {
        return <Button onClick={ showPanel } variant="outline-dark">Show Log</Button>
    }
}