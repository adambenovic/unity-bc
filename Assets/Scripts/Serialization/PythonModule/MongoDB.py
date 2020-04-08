import pymongo as mn
from flask import g
import json
from bson import json_util
import util as utilLibrary

def get_db():
    if 'db' not in g:
        connection_params = {
            'user': 'yourUser',
            'password': 'password',
            'host': 'ds024748.mlab.com',
            'port': 24748,
            'namespace': 'gridbox',
        }

        connection = mn.MongoClient(
            'mongodb://{user}:{password}@{host}:'
            '{port}/{namespace}'.format(**connection_params)
        )

        # Using MONGO localy using docker

        # connection_params = {
        #     'host': 'localhost',
        #     'port': 27017,
        #     'namespace': 'gridbox',
        # }
        #
        # connection = mn.MongoClient(
        #     'mongodb://{host}:'
        #     '{port}/{namespace}'.format(**connection_params)
        # )

        g.db = connection.gridbox

    return g.db


def getResult(cursor):
    result = []

    for document in cursor:
        if(document["_id"] != None):
            document = utilLibrary.parseDocument(document)
        result.append(document)

    result = json.loads(json_util.dumps(result))

    return result


def insertProject(project):
    database = get_db()
    _id = database.projects.insert(project)
    newId = utilLibrary.parseToId(_id,id2)

    return newId


def getProject(id):
    database = get_db()

    cursor = database.projects.find({"_id": id})
    result = getResult(cursor)

    return result


def deleteProject(id):
    database = get_db()
    status = database.projects.remove({"_id": id})
    return status


def updateProject(id, project):
    database = get_db()
    status = database.projects.replace_one({"_id": id}, project)
    return status


def getAllProjects():
    database = get_db()

    cursor = database.projects.find({})
    result = getResult(cursor)

    return result


def insertSimulation(simulation):
    database = get_db()

    _id = database.simulations.insert(simulation)
    newId = utilLibrary.parseToId(_id)
    return newId


def getSimulation(id):
    database = get_db()

    cursor = database.simulations.find({"_id": id})
    result = getResult(cursor)

    return result


def deleteSimulation(id):
    database = get_db()
    status = database.simulations.remove({"_id": id})
    return status


def updateSimulation(id, simulation):
    database = get_db()
    status = database.simulations.replace_one({"_id": id}, simulation)
    return status


def getAllSimulations():
    database = get_db()

    cursor = database.simulations.find({}, {"_id": 1, "healthStatus": 1, "name": 1, "date": 1, "from": 1,
                                            "to":1})
    result = getResult(cursor)

    return result

def getPriceList(id):
    database = get_db()

    cursor = database.prices.find({"_id": id})
    result = getResult(cursor)

    return result


def updatePriceList(id,priceList):
    database = get_db()
    status = database.prices.replace_one({"_id": id}, priceList)

    return status


def getIcon(id):
    database = get_db()

    cursor = database.icons.find({"_id": id})
    result = getResult(cursor)

    return result


def getElementTypes(id):
    database = get_db()

    cursor = database.element_types.find({"_id": id})
    result = getResult(cursor)

    return result


def getAllElementTypes():
    database = get_db()

    cursor = database.element_types.find({})
    result = getResult(cursor)

    return result


def insertIcon(icon):
    database = get_db()

    _id = database.icons.insert(icon)
    newId = utilLibrary.parseToId(_id)
    return newId


def insertElementList(elementList):
    database = get_db()

    _id = database.element_types.insert(elementList)
    newId = utilLibrary.parseToId(_id)
    return newId


def updateElementList(id, elementList):
    database = get_db()
    status = database.element_types.replace_one({"_id": id}, elementList)

    return status


def updateIcon(id, icon):
    database = get_db()
    status = database.icons.replace_one({"_id": id}, icon)

    return status

def deleteIcon(id):
    database = get_db()
    status = database.icons.remove({"_id": id})
    return status


def deleteElementList(id):
    database = get_db()
    status = database.element_types.remove({"_id": id})
    return status


def checkUniqueName(type):
    database = get_db()

    resultNodes = database.element_types.distinct("nodes.type")
    resultLinks = database.element_types.distinct("links.type")


    if (type in resultLinks) or (type in resultNodes):
        uniqueBool = False
    else:
        uniqueBool = True

    response = {
        "unique": uniqueBool
    }

    return response


def insertElement(elementListId, elementJSONFromRequest):
    database = get_db()

    cursor = database.element_types.find({"_id": elementListId})
    result = getResult(cursor)

    elementList = result[0]
    ## elementJSONFromRequest['elementType'] can be either nodes or links
    newArray = elementList[elementJSONFromRequest['elementType']]

    newArray.append(elementJSONFromRequest)
    elementList[elementJSONFromRequest['elementType']] = newArray

    status = database.element_types.update({"_id": elementListId}, {'$set': {
        elementJSONFromRequest['elementType']: newArray
    }})

    return status


def updateElement(elementListId, elementJSONFromRequest, type):
    database = get_db()

    ## elementJSONFromRequest['elementType'] can be either nodes or links
    arrayType = elementJSONFromRequest['elementType'] + '.type'
    arrayToBeSet = elementJSONFromRequest['elementType'] + '.$'

    status = database.element_types.update({"_id": elementListId, arrayType: type}, {'$set': {
        arrayToBeSet: elementJSONFromRequest
    }})

    return status


def returnTimeSeriesByIdNodes(nodes, startTime, endTime):
    database = get_db()
    result = []
    resultset = []

    pipeline = [
        {"$unwind": "$data"},
        {"$match": {"data.ts": {'$gte': startTime, '$lt': endTime}, 'id': {"$in": nodes}}}

    ]

    cursor = database.time_series.aggregate(pipeline)

    firstQuerry = True

    for document in cursor:
        if(firstQuerry):
            prevId = document['id']
            firstQuerry = False

        if prevId != int(document['id']):
            jsonFile = {
                "id": prevId,
                "data": []
            }
            jsonFile['data'] = resultset

            result.append(jsonFile)
            resultset= []
            prevId = document['id']

        temp = document['data']
        resultset.append(temp)

    jsonFile = {
        "id": prevId,
        "data": []
    }
    jsonFile['data'] = resultset

    result.append(jsonFile)
    return result

