import React from "react";
import { Badge } from "react-bootstrap";

export function ResultBadge(props) {

    function Success() {
        return <Badge className="ms-2" bg="success">Success</Badge>;
    }

    function Fail() {
        return <Badge className="ms-2" bg="danger">Fail</Badge>;
    }

    if (props.result === "success") {
        return Success();
    }
    return Fail();
}


