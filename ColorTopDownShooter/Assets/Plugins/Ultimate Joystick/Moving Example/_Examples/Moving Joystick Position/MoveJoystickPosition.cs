using UnityEngine;
using System.Collections;

public class MoveJoystickPosition : MonoBehaviour
{
	public UltimateJoystick joystickToMove;
	public float TimeToTargetPosition = 1.0f;
    public float StartHorizontalPosition = -50;
    public AnimationCurve Curve;
    public AnimationCurve CurveTo;

    private float m_TargetHorizontalPosition;
    private float m_Dist;

    void Start()
    {
        m_TargetHorizontalPosition = joystickToMove.customSpacing_X;
		m_Dist = m_TargetHorizontalPosition - StartHorizontalPosition;

		ApplyPosition(StartHorizontalPosition);
    }

	public void StartMoveFrom ()
	{
		StartCoroutine(MoveFrom());
	}

    public void StartMoveTo()
    {
        StartCoroutine(MoveTo());
    }

	IEnumerator MoveFrom ()
	{
		float speed = 1.0f / TimeToTargetPosition;

		for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed )
		{
			//joystickToMove.customSpacing_X = Mathf.Lerp( startHorizontalPosition, targetHorizontalPosiotion, t );

            ApplyPosition(StartHorizontalPosition + Curve.Evaluate(t) * m_Dist);
			yield return null;
		}

        ApplyPosition(m_TargetHorizontalPosition);
	}

	IEnumerator MoveTo()
	{
		float speed = 1.0f / TimeToTargetPosition;

		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed)
		{
			//joystickToMove.customSpacing_X = Mathf.Lerp( startHorizontalPosition, targetHorizontalPosiotion, t );

            ApplyPosition(m_TargetHorizontalPosition - CurveTo.Evaluate(t) * m_Dist);
			yield return null;
		}

        ApplyPosition(StartHorizontalPosition);
	}


	void ApplyPosition(float pos)
    {
		joystickToMove.customSpacing_X = pos;
		joystickToMove.UpdatePositioning();
    }
}