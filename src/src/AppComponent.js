import React, { useState } from "react";
import { ReportAccordion } from "./accordion/ReportAccordion";
import { ReportsService } from "./domain/ReportsService";
import { PickerComponent } from "./InstrumentPicker/PickerComponent";



export function AppComponent(props) {
    const reportService = new ReportsService(props.reports)
    const [confidenceInterval, setConfidenceInterval] = useState(5);
    const [reports, setReports] = useState([])

    function showReports(updatePickerFunction) {
        const { instruments, interval } = updatePickerFunction()
        setReports(reportService.mapInstrumentsIntoReports(instruments))
        setConfidenceInterval(interval)
    }

    return (
        <div className="mt-5">
            <PickerComponent reports={ props.reports }
                             showReports={ showReports }
                             reportService={ reportService }/>
            <ReportAccordion reports={ reports } confidenceInterval={ confidenceInterval }/>
        </div>)
}