
using mytest2.Utils;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator PlayerAnimator;
    [Range(0.01f, 1)]
    public float DampTime = 0.1f;

    private bool m_IsPaused = false;
    private InterpolationData<float> m_LerpData;

    public void PlayMoveAnimation(float speed, Vector3 lastActiveMoveDir, Quaternion targetRot, bool isInAir = false, bool jumpLastFrame = false)
    {
        if (m_IsPaused)
            return;

        /*bool isStand = speed <= 0 && 
                       (Quaternion.Angle(transform.rotation, targetRot) <= 1) && 
                       !isInAir;

        PlayerAnimator.SetBool(Hash.Stand, isStand);

        if (!isStand)
        {
            Vector3 perpendicularToForward = new Vector3(transform.forward.z, 0, -transform.forward.x);
            float dot = Vector3.Dot(perpendicularToForward, lastActiveMoveDir);

            PlayerAnimator.SetFloat(Hash.Vertical, speed, DampTime, Time.deltaTime);
            PlayerAnimator.SetFloat(Hash.Horizontal, dot);

            //Прыжок
            if (isInAir)
                PlayerAnimator.SetBool(Hash._Jump, jumpLastFrame);
        }
        else
            PlayerAnimator.SetFloat(Hash.Vertical, 0);*/
    }

    public void StopPlayerMoveAnimation()
    {
        if (m_IsPaused)
            return;

       /* PlayerAnimator.SetBool(Hash.Stand, true);*/
    }

    public void PauseAnimations(bool isPaused)
    {
        m_IsPaused = isPaused;

        m_LerpData.TotalTime = PlayerController.ReduceSpeedAtLockInputTime;
        if (isPaused)
        {
            m_LerpData.From = 1;
            m_LerpData.To = 0;
        }
        else 
        {
            m_LerpData.From = 0;
			m_LerpData.To = 1;
        }

        m_LerpData.Start();
    }

    private void Update()
    {
        if (m_LerpData.IsStarted)
        {
            m_LerpData.Increment();

            if (PlayerAnimator != null)
                PlayerAnimator.speed = Mathf.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);

            if (m_LerpData.Overtime())
            {
                if (PlayerAnimator != null)
                    PlayerAnimator.speed = m_LerpData.To;

                m_LerpData.Stop();
            }
        }
    }
}
