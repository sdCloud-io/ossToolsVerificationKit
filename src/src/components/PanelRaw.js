import React from "react";

export function PanelRaw(props) {
    return (<div className="row">
        <div className="col-md-12 col-lg-12 col-xs-12">
            <div className="col-md-4 col-lg-4 col-xs-4">{ props.name }</div>
            { props.values.map(value => <div className="col-md-4 col-lg-4 col-xs-4 text-center">{ value } </div>) }
        </div>
    </div>)
}