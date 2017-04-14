using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandControl : MonoBehaviour {
    const int HAND_DATA_LEN = 60;

    const int SPHERES = 20;
    const int CYLINERS = 15;
    private Controller leapMotion;
    private GameObject[] spheres = new GameObject[SPHERES];
    private GameObject[] cylinders = new GameObject[CYLINERS];

    private float[] getHandDataFromLeapMotion()
    {
        float[] handData = null;

        foreach (Hand hand in leapMotion.Frame().Hands)
        {
            if (handData == null)
            {
                handData = new float[HAND_DATA_LEN];
            }
            for (int i = 0; i < hand.Fingers.Count; i++)
            {
                Leap.Finger finger = hand.Fingers[i];
                Bone bone1 = finger.Bone(Bone.BoneType.TYPE_DISTAL);
                Bone bone2 = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
                Bone bone3 = finger.Bone(Bone.BoneType.TYPE_PROXIMAL);
                Bone bone4 = finger.Bone(Bone.BoneType.TYPE_METACARPAL);
                handData[i * 12 + 0] = bone1.PrevJoint.x - hand.PalmPosition.x;
                handData[i * 12 + 1] = bone1.PrevJoint.y - hand.PalmPosition.y;
                handData[i * 12 + 2] = bone1.PrevJoint.z - hand.PalmPosition.z;
                handData[i * 12 + 3] = bone2.PrevJoint.x - hand.PalmPosition.x;
                handData[i * 12 + 4] = bone2.PrevJoint.y - hand.PalmPosition.y;
                handData[i * 12 + 5] = bone2.PrevJoint.z - hand.PalmPosition.z;
                handData[i * 12 + 6] = bone3.PrevJoint.x - hand.PalmPosition.x;
                handData[i * 12 + 7] = bone3.PrevJoint.y - hand.PalmPosition.y;
                handData[i * 12 + 8] = bone3.PrevJoint.z - hand.PalmPosition.z;
                handData[i * 12 + 9] = bone4.PrevJoint.x - hand.PalmPosition.x;
                handData[i * 12 + 10] = bone4.PrevJoint.y - hand.PalmPosition.y;
                handData[i * 12 + 11] = bone4.PrevJoint.z - hand.PalmPosition.z;
            }
        }

        return handData;
    }

    private float[] getHandDataFromLeapWrist()
    {
        float[] handData = null;
        return handData;
    }

    void updateHand(float[] handData)
    {
        if (handData == null)
        {
            for (int i = 0; i < SPHERES; i++)
            {
                if (spheres[i] != null)
                {
                    Destroy(spheres[i]);
                    spheres[i] = null;
                }
            }
            for (int i = 0; i < CYLINERS; i++)
            {
                if (cylinders[i] != null)
                {
                    Destroy(cylinders[i]);
                    cylinders[i] = null;
                }
            }
        } else
        {
            for (int i = 0; i < SPHERES; i++)
            {
                if (spheres[i] == null)
                {
                    spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    spheres[i].transform.parent = transform;
                }
                spheres[i].transform.localPosition = new Vector3(handData[i * 3], handData[i * 3 + 1], handData[i * 3 + 2]);
                spheres[i].transform.localScale = new Vector3(15.0f, 15.0f, 15.0f);
                spheres[i].GetComponent<Renderer>().material.color = Color.red;
            }
            for (int i = 0; i < CYLINERS; i++)
            {
                if (cylinders[i] == null)
                {
                    cylinders[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cylinders[i].transform.localScale = new Vector3(5.0f, 15.0f, 5.0f);
                    cylinders[i].transform.parent = transform;
                }
            }
            for (int i = 0; i < SPHERES; i++)
            {
                if (i % 4 != 3)
                {
                    int id = i / 4 * 3 + i % 4;
                    int u = i;
                    int v = i + 1;

                    Vector3 p0 = spheres[u].transform.localPosition;
                    Vector3 p1 = spheres[v].transform.localPosition;

                    cylinders[id].transform.localPosition = (p0 + p1) / 2.0f;
                    cylinders[id].transform.localScale = new Vector3(5f, (p1 - p0).magnitude / 2.0f, 5f);
                    cylinders[id].transform.rotation = Quaternion.FromToRotation(Vector3.up, p1 - p0);
                }
            }
        }
    }

    void Start()
    {
        leapMotion = new Controller();
    }

    void Update()
    {
        updateHand(getHandDataFromLeapMotion());
    }
}
