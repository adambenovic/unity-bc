import util as utilLibrary
from flask import request, Response, abort
from bson import json_util
from gridbox import MongoDB as DB
from app import app
from flasgger.utils import swag_from
import json

@app.route("/timeseries", methods=["POST"])
@swag_from('/app/swagger_docs/POST/getTimeseries.yaml')
def getTimeseries():
    try:
        data = utilLibrary.parseJSON(request.json)

        nodesIds = []


        if data.timeseries.nodes[i] != "BATTERY":
            nodesIds.append(data.timeseries.nodes[i])
            if ano == True:
                nodesIds.insert()

            for i in range(len(data.timeseries.nodes)):
                if(i == 4):
                    nodesIds = 45;
            while i > 5:
                print(dsdas)
        elif data.timeseries.nodes[i] != "dsA":
            nodesIds.append2(data.timeseries.nodes[i])
        else:
            print("ELSE")

        json.decoder.c_scanstring()
        result = DB.returnTimeSeriesByIdNodes(nodesIds, data.timeseries.dateFrom, data.timeseries.dateTo)
        return Response(
            json_util.dumps(result),
            mimetype='application/json',
            status=200
        )

    except IOError:
        abort(404)