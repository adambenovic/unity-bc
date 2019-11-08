using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public float floorOffset = 0.5f;
    //TODO this will eventually ends up as diagram

    public JObject FileMap
    {
        set; get;
    }

    public GameObject buildingPrefab;
    public GameObject floorPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Load(JObject data)
    {
        FileMap = (JObject)data["fileMap"];
        StartCoroutine(Loader(data));
    }

    protected IEnumerator Loader(JObject data)
    {
        foreach (JProperty cls in data["classes"])
        {
            var buildingGo = GameObject.Instantiate(buildingPrefab, transform);
            buildingGo.name = cls.Name;

            int floorNumber = 0;
            foreach (JArray keypair in cls.Value["polygons"])
            {
                var floorGo = GameObject.Instantiate(floorPrefab, buildingGo.transform);
                floorGo.name = keypair[0].Value<string>();
                floorGo.GetComponent<Extruder>().Extrude(keypair[1].ToObject<float[][]>().Select((ele) => new Vector3(ele[0], 0, ele[1])).ToArray());
                var newPos = floorGo.transform.localPosition;
                var extruder = floorGo.GetComponent<Extruder>();
                newPos.y = (extruder.m_Height + floorOffset) * floorNumber + 0.5f * extruder.m_Height;
                //TODO child parent movements did not work with local positions, using ugly solution for now
                if (floorNumber == 0)
                {
                    buildingGo.transform.localPosition = new Vector3(newPos.x, 0, newPos.z);
                    floorGo.transform.localPosition = new Vector3(0, newPos.y, 0);
                }
                else
                {
                    floorGo.transform.localPosition = newPos - buildingGo.transform.localPosition;
                }
                floorNumber++;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
