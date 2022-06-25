import React from "react";
import { SingleInstrumentReportAccordion } from "./SingleInstrumentReportAccordion";
import { MultipleInstrumentReportAccordion } from "./MultipleInstrumentReportAccordion";

export function ReportAccordion(props) {
    if (props.reports.length === 0) return <div/>

    if (props.reports.length === 1) {
        return <SingleInstrumentReportAccordion reports={ props.reports }/>
    } else {
        return <MultipleInstrumentReportAccordion reports={ props.reports }
                                                  confidenceInterval={ props.confidenceInterval }/>
    }
}