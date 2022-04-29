import React from "react";

export function RunResult(props) {

    function Success() {
        return <div className='fa fa-check'/>;
    }

    function Fail() {
        return <div className='fa fa-times'/>;
    }

    if (props.result === "success") {
        return Success();
    }
    return Fail();
}