import React from "react";

export function PanelRawCompare(props) {
    function conditionRender(value) {
        if (value) {
            return <div className="col-md-8 col-lg-8 col-xs-8 text-center">In the confidence interval</div>
        } else {
            return <div className="col-md-8 col-lg-8 col-xs-8 text-center">Not in the confidence interval</div>
        }
    }

    return (<div className="row">
        <div className="col-md-12 col-lg-12 col-xs-12">
            <div className="col-md-4 col-lg-4 col-xs-4">{ props.name }</div>
            { conditionRender(props.value) }
        </div>
    </div>)
}