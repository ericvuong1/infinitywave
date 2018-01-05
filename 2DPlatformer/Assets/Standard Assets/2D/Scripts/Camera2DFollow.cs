using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        private Vector3 midPoint;
        public Transform player1;
        public Transform player2;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float yPosRestriction = -1;

        public float minZoom, maxZoom;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        float nextTimeToSearch = 0;

        // Use this for initialization
        private void Start()
        {
            midPoint = (player1.position + player2.position) / 2f;

            m_LastTargetPosition = midPoint;
            m_OffsetZ = (transform.position - midPoint).z;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            if (player1 == null || player2 == null)
            {
                FindPlayer();
                return;
            }
            // only update lookahead pos if accelerating or changed direction
            midPoint = (player1.position + player2.position) / 2f;

            float xMoveDelta = (midPoint - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = midPoint + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            newPos = new Vector3(newPos.x, Mathf.Clamp(newPos.y, yPosRestriction, Mathf.Infinity), newPos.z);

            float zoomSize = Mathf.Clamp(Vector2.Distance(player1.position, player2.position), minZoom, maxZoom);
            this.GetComponentInChildren<Camera>().orthographicSize = zoomSize;

            transform.position = newPos;

            m_LastTargetPosition = midPoint;
        }
        void FindPlayer() //very taxing on computer, so call only a couple of times per seconds
        {
            if (nextTimeToSearch <= Time.time)
            {
                GameObject result1 = GameObject.FindGameObjectWithTag("Player");
                GameObject result2 = GameObject.FindGameObjectWithTag("Player2");
                if (result1 != null && result2 !=null)
                {
                    player1 = result1.transform;
                    player2 = result2.transform;
                    nextTimeToSearch = Time.time + 0.5f;
                }
            }
        }
    }
}
