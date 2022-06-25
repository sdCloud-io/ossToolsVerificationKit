export class CompareService {
    compareInstruments(confidenceInterval, resultItem) {
        if (resultItem.some(elem => elem.Result !== "success")) return []
        const listOfResultDictionaries = resultItem.map(item => item.ResultDictionary, confidenceInterval)

        if (listOfResultDictionaries.length <= 1) return []

        let commonParameters = this.getParameters(listOfResultDictionaries[0])

        listOfResultDictionaries.forEach(resultDictionary => {
            commonParameters = commonParameters.filter(elem => this.getParameters(resultDictionary).indexOf(elem) !== -1)
        })

        console.log(commonParameters)
        return commonParameters.map(elem => this.mapToParametersWithValues(elem, listOfResultDictionaries, confidenceInterval))
    }

    filterByCommonModelResult(listOfReports) {
        let commonModelResultNames = listOfReports[0].ModelResults.map(modelResult => modelResult.ModelPath)

        listOfReports.forEach(report => {
            const listOfNameModelResult = report.ModelResults.map(modelResult => modelResult.ModelPath)
            commonModelResultNames = commonModelResultNames.filter(modelResultName => listOfNameModelResult.indexOf(modelResultName) !== -1)
        })


        let listOfCommonModelResults = []
        const listOfModelResults = listOfReports.map(report => report.ModelResults).flat()

        commonModelResultNames.forEach(modelResultName => {
            let modelResults = {
                ModelName: modelResultName,
                Values: listOfModelResults.filter(modelResult => modelResult.ModelPath === modelResultName)
            }
            listOfCommonModelResults.push(modelResults)
        })

        return listOfCommonModelResults
    }

    mapToParametersWithValues(elem, listOfResultDictionaries, confidenceInterval) {
        const values = listOfResultDictionaries.map(resultDictionary => parseFloat(resultDictionary[elem]))
        console.log(values)
        const minValue = Math.min(...values)
        const maxValue = Math.max(...values)

        return {
            ParameterName: elem,
            IsInConfidenceInterval: this.compareParams(minValue, maxValue, confidenceInterval),
            Values: values
        };
    }

    getInstrumentInformation(reports) {
        return reports.map(report => Object.create({
            InstrumentName: report.InstrumentName,
            InstrumentVersion: report.InstrumentVersion
        }))
    }

    compareParams(value1, value2, confidenceInterval) {
        if (value1 === value2) return true
        return (((value2 - value1) / value1) * 100) <= confidenceInterval;
    }

    getParameters(resultDictionary) {
        return Object.getOwnPropertyNames(resultDictionary)
    }
}