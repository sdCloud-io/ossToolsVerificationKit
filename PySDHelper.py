#!/usr/bin/python3
import pysd 
import numpy 
import pandas
import sys
import json

def print_full(x, resultsModelName):
    pandas.set_option('display.max_rows', len(x))
    print(x, file=open(resultsModelName + '.result', 'w'))
    pandas.reset_option('display.max_rows')

numpy.set_printoptions(threshold=sys.maxsize)

if len(sys.argv) != 3:
    print("Error: wrong number of arguments. Expected 2 but got " + str(len(sys.argv) - 1))
    exit()

command=sys.argv[1]
modelName=sys.argv[2]

print('Command is ' + modelName)
print('Model file name is ' + modelName)

if command == 'compileMdl':
    model = pysd.read_vensim(modelName)
elif command == 'compileAndRunMdl':
    model = pysd.read_vensim(modelName)
    result=model.run()
    print_full(json.dumps(result.to_dict()), modelName)
elif command == 'compileXmile':
    model = pysd.read_xmile(modelName)
elif command == 'compileAndRunXmile':
    model = pysd.read_xmile(modelName)
    result=model.run()
    print_full(json.dumps(result.to_dict()), modelName)

exit()
