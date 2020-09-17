using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    [Serializable]
    public class MULTI_FOLLOW_AXIS
    {
        public bool x = true;
        public bool y = true;
        public bool z = true;
    }

    [ExecuteInEditMode]
    public class MultiTransformFollower : MonoBehaviour
    {
        [Header("Lerp Setting")]
        public GameObject target_A;
        public GameObject target_B;
        [Range(0.0f, 1.0f)]
        public float lerp = 0.0f;

        [Header("Use Axis")]
        public MULTI_FOLLOW_AXIS followPosition;
        public MULTI_FOLLOW_AXIS followRotation;
        public MULTI_FOLLOW_AXIS followScale;


        void Awake()
        {
            SetFollow();
        }

        void Start()
        {

        }

        void Update()
        {
            SetFollow();
        }


        private void SetFollow()
        {
            if(target_A != null && target_B != null)
            {
                if(followPosition.x || followPosition.y || followPosition.z)
                    SetFollowPosition();

                if(followRotation.x || followRotation.y || followRotation.z)
                    SetFollowRotation();

                if(followScale.x || followScale.y || followScale.z)
                    SetFollowScale();
            }
        }

        // 位置を設定
        private void SetFollowPosition()
        {
            Vector3 lerped = Vector3.Lerp(target_A.transform.position, target_B.transform.position, lerp);
            float x = followPosition.x ? lerped.x : this.gameObject.transform.position.x;
            float y = followPosition.y ? lerped.y : this.gameObject.transform.position.y;
            float z = followPosition.z ? lerped.z : this.gameObject.transform.position.z;

            this.gameObject.transform.position = new Vector3(x, y, z);
        }

        // 回転を設定
        private void SetFollowRotation()
        {
            Quaternion lerped = Quaternion.Lerp(target_A.transform.rotation, target_B.transform.rotation, lerp);
            float x = followRotation.x ? lerped.eulerAngles.x : this.gameObject.transform.rotation.eulerAngles.x;
            float y = followRotation.y ? lerped.eulerAngles.y : this.gameObject.transform.rotation.eulerAngles.y;
            float z = followRotation.z ? lerped.eulerAngles.z : this.gameObject.transform.rotation.eulerAngles.z;

            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
        }

        // スケールを設定
        private void SetFollowScale()
        {
            Vector3 lerped = Vector3.Lerp(target_A.transform.localScale, target_B.transform.localScale, lerp);
            float x = followScale.x ? lerped.x : this.gameObject.transform.localScale.x;
            float y = followScale.y ? lerped.y : this.gameObject.transform.localScale.y;
            float z = followScale.z ? lerped.z : this.gameObject.transform.localScale.z;

            this.gameObject.transform.localScale = new Vector3(x, y, z);
        }
    }
}
