using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JointObject : zzEditableObject
{
    protected override void applyState()
    {
        base.applyState();
        if (play)
        {
            UpdateJoint();
        }
        else
        {
            foreach (var lJoint in JointList)
            {
                Destroy(lJoint);
            }
            JointList = new ConfigurableJoint[0];
        }
    }

    public zzRigidbodySweepDetector sweepDetector1;
    public zzRigidbodySweepDetector sweepDetector2;

    [SerializeField]
    ConfigurableJoint[] JointList = new ConfigurableJoint[0];

    [SerializeField]
    bool _freezeObjectRotation = false;

    void updateFreeze()
    {
        ConfigurableJointMotion lAngularMotion;
        if (_freezeObjectRotation)
            lAngularMotion = ConfigurableJointMotion.Locked;
        else
            lAngularMotion = ConfigurableJointMotion.Free;


        ConfigurableJointMotion lMoveMotion = ConfigurableJointMotion.Locked;
        foreach (var lJoint in JointList)
        {
            lJoint.angularXMotion = lAngularMotion;
            lJoint.angularYMotion = lAngularMotion;
            lJoint.angularZMotion = lAngularMotion;

            lJoint.xMotion = lMoveMotion;
            lJoint.yMotion = lMoveMotion;
            lJoint.zMotion = lMoveMotion;
        }

    }

    [FieldUI("冻结物体旋转")]
    public bool freezeObjectRotation
    {
        get { return _freezeObjectRotation; }

        set
        {
            _freezeObjectRotation = value;
            updateFreeze();
        }
    }

    [ContextMenu("UpdateJoint")]
    public void UpdateJoint()
    {
        float lXLength = transform.lossyScale.x;
        HashSet<Rigidbody> lBodyToConnect = new HashSet<Rigidbody>();
        sweepDetector1.distance = lXLength;
        sweepDetector2.distance = lXLength;
        foreach (var lHit in sweepDetector1.SweetTest())
        {
            lBodyToConnect.Add(lHit.rigidbody);
        }
        foreach (var lHit in sweepDetector2.SweetTest())
        {
            lBodyToConnect.Add(lHit.rigidbody);
        }

        var lNewJointList = new List<ConfigurableJoint>(lBodyToConnect.Count);
        var lAddedList = new List<Rigidbody>();
        foreach (var lJoint in JointList)
        {
            if (lBodyToConnect.Contains(lJoint.connectedBody))
            {
                //添加到新列表中,从 创建表中移除
                lNewJointList.Add(lJoint);
                lBodyToConnect.Remove(lJoint.connectedBody);
            }
            else
            {
                Destroy(lJoint);
            }
        }

        //连接未连接的刚体
        foreach (var lRigidbody in lBodyToConnect)
        {
            var lJoint = gameObject.AddComponent<ConfigurableJoint>();
            lNewJointList.Add(lJoint);
            lJoint.connectedBody = lRigidbody;
        }
        JointList = lNewJointList.ToArray();
        updateFreeze();
    }
}