using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonManager : MonoBehaviour
{
    //The list of joints used for visualization
    private List<GameObject> listOfJoints = new List<GameObject>();

    //The prefab that will be used for visualization of the joints 
    [SerializeField]
    private GameObject jointPrefab;

    private float speed = 20f;
    private float step;

    private bool hasConfidence;
    private float skeletonConfidenceThreshold = 0.0001f;

    [SerializeField]
    private Material[] jointsMaterial;

    //Use this to make the depth values smaler to fit the depth of the hand for each joints when using 3D joint positions. 
    private int depthDivider = 8;

    // The number of Joints the skeleton is made of
    private int jointsLength = 21;

    private void Start()
    {
        Inititialize();
        SkeletonModel();
    }

    /// <summary>
    /// Create the hand model depending if you use 3D or 2D joints.
    /// The model need to have 21 joints.
    /// </summary>
    /// <param name="currentModel"></param>
    /// <param name="previousModel"></param>
    private void SkeletonModel()
    {
        Debug.Log("Skeleton joints model loaded");

        if (jointPrefab.transform.childCount == jointsLength)
        {
            listOfJoints.Clear();

            for (int i = 0; i < jointPrefab.transform.childCount; i++)
            {
                listOfJoints.Add(jointPrefab.transform.GetChild(i).gameObject);
            }
        }

        else
        {
            Debug.LogFormat("Current model have {0} joints, need to have 21 joints", jointPrefab.transform.childCount);
        }
    }

    void Inititialize()
    {
        for (int i = 0; i < jointsMaterial.Length; i++)
        {
            Color tempColor = jointsMaterial[i].color;
            tempColor.a = 0f;
            jointsMaterial[i].color = tempColor;
        }
    }

    void Update()
    {
        hasConfidence = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.confidence > skeletonConfidenceThreshold;
        UpdateJointPositions();
    }

    /// <summary>
    /// Updates the position of the joints according to the positions given by the SDK.
    /// If confidence is to low, the joints will fade out.
    /// </summary>
    private void UpdateJointPositions()
    {
        if (hasConfidence)
        {
            if (jointsMaterial[jointsMaterial.Length - 1].color.a < 1)
            {
                for (int i = 0; i < jointsMaterial.Length; i++)
                {
                    Color tempColor = jointsMaterial[i].color;
                    tempColor.a += 0.1f;
                    jointsMaterial[i].color = tempColor;
                }

                jointPrefab.SetActive(true);
            }

            for (int i = 0; i < ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.joints.Length; i++)
            {
                float depthEstimation = Mathf.Clamp(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation + 0.3f, 0.4f, 1);
                float jointDepthValue = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.joints[i].z / depthDivider;

                Vector3 newPosition = ManoUtils.Instance.CalculateNewPosition(new Vector3(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.joints[i].x, ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.joints[i].y, jointDepthValue), depthEstimation + 0.25f);

                step = speed * Time.deltaTime * Vector3.Distance(listOfJoints[i].transform.position, newPosition);
                listOfJoints[i].transform.position = Vector3.MoveTowards(listOfJoints[i].transform.position, newPosition, step);
            }
        }

        else
        {
            if (jointsMaterial[0].color.a > 0)
            {
                for (int i = 0; i < jointsMaterial.Length; i++)
                {
                    Color tempColor = jointsMaterial[i].color;
                    tempColor.a -= 0.1f;
                    jointsMaterial[i].color = tempColor;
                }

                if (jointsMaterial[0].color.a == 0)
                {
                    jointPrefab.SetActive(false);
                }
            }
        }
    }
}
