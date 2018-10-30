using System;
using UnityEngine;

namespace mytest2.Character.Dodging
{
    public class Dodging_CharacterController : MonoBehaviour, iDodging
    {
        public event Action OnDodgeStarted;
        public event Action OnDodgeFinished;

        public float DodgeDist = 5;
        public float DodgeSpeed = 5;

        private Vector3 m_DodgeDir;
        private CharacterController m_CharacterController;
        private Utils.InterpolationData<float> m_DodgeTimeLerpData;

        public bool IsDodging
        {
            get { return m_DodgeTimeLerpData.IsStarted; }
        }


        public void Init()
        {
            m_CharacterController = transform.GetComponent<CharacterController>();
        }

        public void Dodge(Vector2 dir)
        {
            if (m_DodgeTimeLerpData.IsStarted)
                return;

            m_DodgeDir = new Vector3(dir.x, 0, dir.y);

            if (OnDodgeStarted != null)
                OnDodgeStarted();

            float dodgeTime = DodgeDist / DodgeSpeed;
            m_DodgeTimeLerpData = new Utils.InterpolationData<float>(dodgeTime); 
            m_DodgeTimeLerpData.Start();
        }


        void Update()
        {
            if (m_DodgeTimeLerpData.IsStarted)
            {
                m_DodgeTimeLerpData.Increment();
                m_CharacterController.SimpleMove(m_DodgeDir * DodgeSpeed);

                if(m_DodgeTimeLerpData.Overtime())
                {
                    m_DodgeTimeLerpData.Stop();

                    if (OnDodgeFinished != null)
                        OnDodgeFinished();
                }
            }
        }
    }
}
