# -*- coding: utf-8 -*-
"""
Created on Thu Jul 28 22:41:24 2016
http://www.devdungeon.com/content/working-binary-data-python
@author: Daniel Claus
NOTE: use Python version 2.7
        configure output time format with 'choosenTimestampFormat'
"""

import struct
import csv
from enum import Enum

class LineFormatBySensor(Enum):
    Accelerometer = b"q f f f"
    Gyrometer = b"q f f f"
    Quaternion = b"q f f f f"
    Evaluation = b"q f f ? ? ?"
    Geolocation = b"q d d d d d"
    # more struct format characters unter: https://docs.python.org/2/library/struct.html

class csvHeaderBySensor(Enum):
    Accelerometer = ['timestamp','xCoordinate', 'yCoordinate', 'zCoordinate']
    Gyrometer = ['timestamp','xVelocity', 'yVelocity', 'zVelocity']
    Quaternion = ['timestamp','angleW','xCoordinate', 'yCoordinate','zCoordinate']
    Evaluation = ['timestamp','accelermeterVectorLength','gyrometerVectorLength','accelerometerPeak','accelerometerPeak','DetectedStep']
    Geolocation = ['timestamp','latitude','longitude','altitude','accuracyInMeter','speedInMeterPerSecond']

class timestampFormat(Enum):
    Ticks = [1]
    Milliseconds = [10000]
    Duration = [60*60*1000*10000, 60*1000*10000, 1000*10000, 10000] # hh,mm,ss,ms

#
# read and evaluate the header
#
def readHeader(binary_file):
    print('Read headerline of new data part')
    byteStruct = struct.Struct(b"i")
    # read length of the next data set
    couple_bytes = binary_file.read(4)
    rowLength = byteStruct.unpack(couple_bytes)[0]
    # read amount of rows in the next data set
    couple_bytes = binary_file.read(4)
    amountOfRows = byteStruct.unpack(couple_bytes)[0]
    # read length of the describing header part
    couple_bytes = binary_file.read(4)
    headerSize = byteStruct.unpack(couple_bytes)[0]
    # read sensor and describing header part
    header = binary_file.read(int(headerSize/float(2)))
    splittedHeader = str(header).split(':')
    sensor = splittedHeader[0] # choose the first part and truncate the frist both characters
    fieldnames = splittedHeader[1]
    # print results
    print('Daten des Sensor: '+ str(sensor))
    print('Zeilenbeschreibung: '+ str(fieldnames))
    print('Länge einer Zeile in Byte: ' + str(rowLength))
    print('Anzahl der Zeilen des Sensors: ' + str(amountOfRows))
    return (rowLength, amountOfRows, sensor, fieldnames)

#
# start method which handels the conversion
#
def convertFileIntoCsv(filename):
    print('Start converion of file: ' + str(filename))

    with open(filename, "rb") as binary_file:
        # determine the length of the file
        fileLength = determineFileLength(binary_file)
        # Seek position and read N bytes
        binary_file.seek(0) # Go to beginning of the file

        while (binary_file.tell() <= fileLength):
            rowLength, amountOfRows, sensor, fieldnames = readHeader(binary_file)

            dataList = []
            for _ in range(int(amountOfRows)):
                currentRow = binary_file.read(rowLength)
                byteStruct = struct.Struct(LineFormatBySensor[str(sensor)].value)
                rawTuple = byteStruct.unpack(currentRow)
                dataList.append(rawTuple)

            writeDataToFile(dataList, filename, sensor)
            print(' ') #empty line at the and of a completed sensor

        print('finish file conversion')
        binary_file.close

#
# Method determine the total length of the given file.
#
def determineFileLength(binary_file):
    oldPosition = binary_file.tell()
    binary_file.seek(-1, 2) # 2 – end of the stream; offset is usually negative
    fileLength = binary_file.tell()
    binary_file.seek(oldPosition)
    print('total file length: ' + str(fileLength))
    return fileLength

#
# Method writes a single csv line taking account of the sensor
#
def writeCsvLine(writer, dataTuple, sensor):
    fieldnames = csvHeaderBySensor[str(sensor)].value
    if (sensor == csvHeaderBySensor['Accelerometer'].name) or (sensor == csvHeaderBySensor['Gyrometer'].name):
        writer.writerow({fieldnames[0]:convertTimestamp(dataTuple[0]), fieldnames[1]:dataTuple[1], fieldnames[2]:dataTuple[2], fieldnames[3]:dataTuple[3]})
    elif (sensor == csvHeaderBySensor['Quaternion'].name):
        writer.writerow({fieldnames[0]:convertTimestamp(dataTuple[0]), fieldnames[1]:dataTuple[1], fieldnames[2]:dataTuple[2], fieldnames[3]:dataTuple[3], fieldnames[4]:dataTuple[4]})
    elif (sensor == csvHeaderBySensor['Evaluation'].name) or (sensor == csvHeaderBySensor['Geolocation'].name):
        writer.writerow({fieldnames[0]:convertTimestamp(dataTuple[0]), fieldnames[1]:dataTuple[1], fieldnames[2]:dataTuple[2], fieldnames[3]:dataTuple[3], fieldnames[4]:dataTuple[4], fieldnames[5]:dataTuple[5]})

#
# converts a long of ticks into a human readable format
#
def convertTimestamp(value):
    if (timestampFormat[str(choosenTimestampFormat)] == timestampFormat.Duration):
        timeTuple = decompose(value, timestampFormat[str(choosenTimestampFormat)].value)
        result = "{0[0]:02d}:{0[1]:02d}:{0[2]:02d}.{0[3]:03d}".format(timeTuple)
    elif (timestampFormat[str(choosenTimestampFormat)] == timestampFormat.Milliseconds):
        result = decompose(value, timestampFormat[str(choosenTimestampFormat)].value)[0]
    else:
        result = decompose(value, timestampFormat[str(choosenTimestampFormat)].value)[0]
    return result

#
# 
#
def decompose(value, base):
    n = value
    result = []
    for b in base:
        d, n = divmod(n, b)
        result.append(d)
    return result

#
# Method writes the given data list into a file with the given name.
#
def writeDataToFile(dataList, filename, sensor):
    csvFilename = filename.split('.')[0] + '_' + sensor + '.csv'
    print('Start data output in file: ' + csvFilename)

    with open(csvFilename, 'w') as csvfile:
        fieldnames = csvHeaderBySensor[str(sensor)].value
        writer = csv.DictWriter(csvfile, delimiter=',', lineterminator='\n', fieldnames = fieldnames)
        writer.writeheader()

        for dataTuple in dataList:
            writeCsvLine(writer, dataTuple, sensor)

        csvfile.close
    print('finish output data')


#
# Start script -----------------------------------------------------------------------------------------------------------------------------------------
#
# Define global variables
filename = raw_input('Dateiname eingeben: ')
choosenTimestampFormat = timestampFormat.Milliseconds.name
# run conversion
convertFileIntoCsv(filename)