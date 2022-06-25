export class ReportsService {
    instruments

    constructor(reports) {
        this.reports = reports;
        this.instruments = this.reports.map(this.extractInstruments);
    }

    getDefaultInstrument() {
        const defaultReport = this.instruments[0];

        return {
            Name: defaultReport.Name,
            ModelType: defaultReport.ModelType,
            Version: defaultReport.InstrumentVersion
        }
    }

    getModelTypesByName(instrumentName) {
        return this.instruments
            .filter(instrument => instrument.Name === instrumentName)
            .map(instrument => instrument.ModelType)
    }

    getModelTypesByNameAndModelType(instrumentName, modelType) {
        return this.instruments
            .filter(instrument => instrument.Name === instrumentName)
            .filter(instrument => instrument.ModelType === modelType)
            .map(instrument => instrument.InstrumentVersion)
    }

    mapInstrumentsIntoReports(instruments) {
        return instruments
            .map(instrument => this.findReport(instrument.Name, instrument.ModelType, instrument.InstrumentVersion))
    }

    extractInstruments(report) {
        return {
            Name: report.InstrumentName,
            ModelType: report.ModelTypes,
            InstrumentVersion: report.InstrumentVersion
        }
    }

    findReport(name, modelType, version) {
        return this.reports
            .find(report => report.InstrumentName === name
                && report.ModelTypes === modelType
                && report.InstrumentVersion === version)
    }
}