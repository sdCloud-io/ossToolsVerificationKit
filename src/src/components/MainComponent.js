import Panel from "react-bootstrap/lib/Panel";
import { LogPanel } from "./LogPanel";
import React from "react";
import { PanelRaw } from "./PanelRaw";
import { RunResult } from "./RunResult";
import { PanelRawCompare } from "./PanelRawCompare";


export function MainComponent(props) {
    function getNameFromItem(resultItem) {
        return resultItem.ModelPath.replace(/^.*[\\\/]/, '')
    }

    function getInstrumentNameFromItem(resultItem) {
        return resultItem.ModelInstrumentResults.map(instumentResult =>
            <div>
                { instumentResult.ScriptName }:
                <RunResult result={ instumentResult.Result }/>
            </div>
        )
    }

    function getLogFromItem(resultItem) {
        return resultItem.ModelInstrumentResults.map(instumentResult =>
            <LogPanel log={ instumentResult.Log }/>)
    }

    function getGenerationTimeFromItem(resultItem) {
        return resultItem.ModelInstrumentResults.map(instumentResult =>
            instumentResult.CodeGenerationTime)
    }

    function getExecutionTimeFromItem(resultItem) {
        return resultItem.ModelInstrumentResults.map(instumentResult =>
            instumentResult.CodeExecutionTime)
    }

    function getCompilationTimeFromItem(resultItem) {
        return resultItem.ModelInstrumentResults.map(instumentResult =>
            instumentResult.CodeExecutionTime)
    }

    function getComparisonParameters(dictionary) {
        if (dictionary === undefined) return <div></div>
        var dict = dictionary.ComparisonInstruments[0].Values
        return (
            Object
                .getOwnPropertyNames(dict)
                .map(x => <PanelRawCompare name={ x } value={ dict[x] }/>)
        );
    }

    return (
        <div className="mt-5">
            { props.reportExecutionsCollection.ModelResults.map((resultItem, i) =>
                <Panel key={ i } index={ i } id="collapsible-panel-example-3">
                    <Panel.Toggle>
                        <Panel.Heading>
                            <Panel.Title>
                                <PanelRaw
                                    name={ getNameFromItem(resultItem) }
                                    values={ getInstrumentNameFromItem(resultItem) }/>
                            </Panel.Title>
                        </Panel.Heading>
                    </Panel.Toggle>

                    <Panel.Collapse>
                        <Panel.Body>
                            <PanelRaw
                                name="Log"
                                values={ getLogFromItem(resultItem) }/>

                            <PanelRaw
                                name="Generation Time"
                                values={ getGenerationTimeFromItem(resultItem) }/>

                            <PanelRaw
                                name="Execution Time"
                                values={ getExecutionTimeFromItem(resultItem) }/>

                            <PanelRaw
                                name="Compilation Time"
                                values={ getCompilationTimeFromItem(resultItem) }/>

                            {
                                getComparisonParameters(props.reportComparisonCollection.ComparisonModels
                                    .find(item => item.ModelPath === resultItem.ModelPath))

                            }
                        </Panel.Body>
                    </Panel.Collapse>
                </Panel>) }
        </div>)
}