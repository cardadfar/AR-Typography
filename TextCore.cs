using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.HelloAR;

public class TextCore : MonoBehaviour {
    
    public int clickCount;  //number of screen taps
    public TextMesh textLayer;  //text layer to be changed 
    public GameObject TrackedPointPrefab;  //prefab that generates over tracked points onscreen
    public GameObject GeneratePointPrefab;  //prefab that generates over a particular tracked point when screen is tapped
    public List<Vector4> points;  //array of tracked points <x,y,z,c> s.t. c is confidence from 0 to 1

    public Camera Camera;

    List<string> content = new List<string>();  //array of words to process

    void filterPoints(float filter) {
        for(var i = 0; i < points.Count; i++) {
            if(points[i].w < filter)
                points.RemoveAt(i);
        }
    }

    //canSee (camera, point) = true if point is in view of camera, false otherwise
    bool canSee (Camera camera, Vector3 point) {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

    //minIndxFinder (minIndx, minReg, touchPos) = the index i s.t points[i] is the closest point to touchPos
    int minIndxFinder(int minIndx, float minReg, Vector3 touchPos) {
        for(int i = 0; i < points.Count; i++) {
            if(canSee(Camera, new Vector3( points[i].x, points[i].y, points[i].z))) {
                Vector3 pointTemp = Camera.WorldToViewportPoint(new Vector3( points[i].x, points[i].y, points[i].z));
                Vector3 pointTempAdj = new Vector3( (Screen.width) * pointTemp.x, (Screen.height) * pointTemp.y, 0);

                float minRegTemp = (pointTempAdj - touchPos).magnitude;
                if(minRegTemp < minReg) {
                    minReg = minRegTemp;
                    minIndx = i;
                }
            }
        }
        return minIndx;
    }

    void generateText (Vector3 target, Vector3 cross, int depthLen) {

        Vector3 camDir = Camera.transform.forward;
        float dot = Vector3.Dot(camDir, cross);
        if(dot < 0) {
            cross = Vector3.Reflect(camDir, cross);
        }

        float dist = ((Camera.transform.position) - cross).magnitude;
        textLayer.fontSize = 500 + 500 * (int)dist;

        textLayer.color = Color.black;
        Vector3 depth = target;
        for(int i = 0; i < depthLen; i++) {
            if(i > depthLen - 5) {
                textLayer.color = Color.white;
            }
            Instantiate(GeneratePointPrefab, depth, Quaternion.LookRotation(cross, new Vector3 (0,1,0)), transform);
            depth -= 0.001f*cross;
        }
    }

    void updateTouch() {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) {
                float x = touch.position.x;
                float y = touch.position.y;
                Vector3 touchPos = new Vector3(x, y, 0.0f);
                clickCount++;
                clickCount = clickCount % content.Count;
                textLayer.text = content[clickCount];

                if(points.Count > 2) {
                    int minIndx = minIndxFinder(0, 99999999999999f, touchPos);
                    int minIndx2 = 1;
                    int minIndx3 = 2;
                    float minDis1 = 99999999999999f;
                    float minDis2 = 99999999999999f;
                    for(int i = 0; i < points.Count; i++) {
                        if(i != minIndx) {
                            float minDisTemp = (points[minIndx] - points[i]).magnitude;
                            if(minDisTemp < minDis1) {
                                minDis1 = minDisTemp;
                                minIndx2 = i;
                            }
                            else if(minDisTemp < minDis2) {
                                minDis2 = minDisTemp;
                                minIndx3 = i;
                            }
                        }
                    }
                    Vector3 v1 = points[minIndx2] - points[minIndx];
                    Vector3 v2 = points[minIndx3] - points[minIndx];
                    Vector3 cross = Vector3.Cross(v1, v2);
                    cross /= cross.magnitude;
                    Vector3 target = new Vector3(points[minIndx].x, points[minIndx].y, points[minIndx].z);
                    generateText(target, cross, 75);
                }
            }
        }
    }

    void Start() {
        content.Add("How Far We've Come");
    }

    void Update() {
        points = GetComponent<HelloARController>().getPoints();

        print(Camera.transform.forward);

        //filterPoints(0.1);

        updateTouch();
    }
}
